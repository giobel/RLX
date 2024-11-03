#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using System.Numerics;
using Autodesk.Revit.DB.Structure;
#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class FindCenterlines : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;


            FamilySymbol fs = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).First(q => q.Name.Equals("(BR01a) CHS114.3x4")) as FamilySymbol;

            Level level = new FilteredElementCollector(doc).OfClass(typeof(Level)).ToElements().First() as Level;


            Options opt = new Options();

            IList<Reference> linkModelRefs = uidoc.Selection.PickObjects(ObjectType.LinkedElement, "Select Elements");

            using (Transaction t = new Transaction(doc, "Copy Linked Elements"))
            {

                t.Start();

                foreach (var linkModelRef in linkModelRefs)
                {

                    var e = doc.GetElement(linkModelRef.ElementId);
                    RevitLinkInstance revitLinkInst = e as RevitLinkInstance;
                    Document linkRvtDoc = (e as RevitLinkInstance).GetLinkDocument();
                    Transform transf = revitLinkInst.GetTransform();

                    Element eLinked = linkRvtDoc.GetElement(linkModelRef.LinkedElementId);

                    GeometryElement fiGeometry = eLinked.get_Geometry(opt);

                    List<XYZ> vertexes = new List<XYZ>();

                    foreach (GeometryObject geoObj in fiGeometry)

                        if (geoObj is Solid)
                        {
                            //TaskDialog.Show("R", geoObj.ToString());

                            List<Face> allFaces = new List<Face>();

                            Solid solid = geoObj as Solid;
                            foreach (Face face in solid.Faces)
                            {
                                allFaces.Add(face);

                                foreach (var v in face.Triangulate().Vertices)
                                {
                                    vertexes.Add(v);
                                }

                            }

                            //TaskDialog.Show("R", vertexes.Count.ToString());
                        }
                    //geoObjects.Add(geoObj);


                    /*
                    XYZ centroid = new XYZ(vertexes.Average(p => p.X), vertexes.Average(p => p.Y), vertexes.Average(p => p.Z));

                    double sumXX = 0, sumYY = 0, sumZZ = 0, sumXY = 0, sumXZ = 0, sumYZ = 0;

                    foreach (var point in vertexes)
                    {
                        sumXX += (point.X - centroid.X) * (point.X - centroid.X);
                        sumYY += (point.Y - centroid.Y) * (point.Y - centroid.Y);
                        sumZZ += (point.Z - centroid.Z) * (point.Z - centroid.Z);
                        sumXY += (point.X - centroid.X) * (point.Y - centroid.Y);
                        sumXZ += (point.X - centroid.X) * (point.Z - centroid.Z);
                        sumYZ += (point.Y - centroid.Y) * (point.Z - centroid.Z);
                    }

                    //double[,] covarianceMatrix = new double[3, 3]
                    var covarianceMatrix = DenseMatrix.OfArray(new double[,]
                                            {
                            { sumXX, sumXY, sumXZ },
                            { sumXY, sumYY, sumYZ },
                            { sumXZ, sumYZ, sumZZ }
                                            };


                    // Eigen decomposition

                    var evd = covarianceMatrix.Evd(Symmetricity.Symmetric);
                    var eigenvectors = evd.EigenVectors;
                    var eigenvalues = evd.EigenValues;

                    // Get the eigenvector corresponding to the largest eigenvalue

                    int maxIndex = eigenvalues.MaximumIndex();
                    var direction = eigenvectors.Column(maxIndex).ToArray();
                    XYZ directionVector = new XYZ(direction[0], direction[1], direction[2]).Normalize();
                    */

                    var (slope, intercept) = CalculateBestFitLine(vertexes);

                    // Determine the start and end points of the line based on the slope
                    double xMin = double.MaxValue, xMax = double.MinValue;

                    foreach (var point in vertexes)
                    {
                        if (point.X < xMin) xMin = point.X;
                        if (point.X > xMax) xMax = point.X;
                    }

                    XYZ start = new XYZ(xMin, slope * xMin + intercept, 0);
                    XYZ end = new XYZ(xMax, slope * xMax + intercept, 0);



                    Line line = Line.CreateBound(start, end);
                    //doc.Create.NewModelCurve(line, SketchPlane.Create(doc, Autodesk.Revit.DB.Plane.CreateByNormalAndOrigin(XYZ.BasisZ, XYZ.Zero)));
                    XYZ newSt = transf.OfPoint(line.GetEndPoint(0));
                    XYZ newEnd = transf.OfPoint(line.GetEndPoint(1));

                    Line newLine = Line.CreateBound(newSt, newEnd);

                    FamilyInstance fa = doc.Create.NewFamilyInstance(newLine, fs,level, StructuralType.Beam);

                    

                    // Define start and end points of the line
                    //XYZ startPoint = centroid - directionVector * 10; // Extend backward
                    //XYZ endPoint = centroid + directionVector * 10;   // Extend forward

                    //        Line line = Line.CreateBound(startPoint, endPoint);
                    //        // Ensure you have a valid sketch plane; for example:
                    //        SketchPlane sketchPlane = SketchPlane.Create(doc, Autodesk.Revit.DB.Plane.CreateByNormalAndOrigin(XYZ.BasisZ, centroid));

                    //        //Create the model curve
                    //        doc.Create.NewModelCurve(line, sketchPlane);
                    //}

                }

                    t.Commit();


                return Result.Succeeded;

            }//close using


        }//close execute

        public static (double slope, double intercept) CalculateBestFitLine(IList<XYZ> points)
        {
            int n = points.Count;
            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;

            foreach (var point in points)
            {
                sumX += point.X;
                sumY += point.Y;
                sumXY += point.X * point.Y;
                sumX2 += point.X * point.X;
            }

            double meanX = sumX / n;
            double meanY = sumY / n;

            double slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            double intercept = meanY - slope * meanX;

            return (slope, intercept);
        }

    }//close class


}//close namespace
