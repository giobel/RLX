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
using MathNet.Numerics.Distributions;
using System;
using MathNet.Numerics;
#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class FindCenterlines2 : IExternalCommand
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

            Reference linkModelRef = uidoc.Selection.PickObject(ObjectType.LinkedElement, "Select Linked Beam");

            using (Transaction t = new Transaction(doc, "Move beam"))
            {

                t.Start();

                //foreach (var linkModelRef in linkModelRefs)
                //{

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

                            List<Face> planarFaces = new List<Face>();

                            Solid solid = geoObj as Solid;

                            foreach (Face face in solid.Faces)
                            {
                                if (face.GetType().ToString().Contains("Planar"))
                                {
                                    planarFaces.Add(face);
                                foreach (XYZ pt in face.Triangulate().Vertices)
                                {
                                    vertexes.Add(transf.OfPoint(pt));
                                }

                                }

                            }
                        XYZ centroid = XYZ.Zero;

                        foreach (XYZ point in vertexes)
                        {
                            centroid += point;
                        }

                        centroid /= vertexes.Count;

                        XYZ direction = vertexes[1] - vertexes[0];

                        for (int i = 0; i < 100; i++)
                        {

                            XYZ nextDirection = XYZ.Zero;

                            foreach (XYZ point in vertexes)
                            {

                                XYZ centeredPoint = point - centroid;

                                double dp = centeredPoint.DotProduct(direction);

                                nextDirection += dp * centeredPoint;
                            }
                            //nextDirection.Normalize();
                            direction = nextDirection.Normalize();
                        }

                        XYZ endPoint = centroid + direction * 8;

                        Line line = Line.CreateBound(centroid, endPoint);

                        FamilyInstance fa = doc.Create.NewFamilyInstance(line, fs, level, StructuralType.Beam);
                        //(ele.Location as LocationCurve).Curve = line;

                        //ele.LookupParameter("z Justification").Set(2);

                        //TaskDialog.Show("R", vertexes.Count.ToString());
                    }




                //}//close foreach

                t.Commit();


                return Result.Succeeded;

            }//close using


        }//close execute

     

    }//close class


}//close namespace
