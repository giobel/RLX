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

                    XYZ direction = new XYZ(0, 0, 0);

                    direction.Normalize();

                    direction = vertexes[1] - vertexes[0];

                    XYZ origin = new XYZ(0, 0, 0);

                    for (int i = 2; i < vertexes.Count; i++)
                    {
                        origin += vertexes[i];
                    }
                    
                    origin /= vertexes.Count;


                    for (int iter = 0; iter < 100; iter++)
                    {
                        XYZ newDirection = new XYZ(0,0,0);
                        foreach (XYZ v in vertexes)
                        {
                            XYZ point = v - origin;
                            newDirection += direction.DotProduct(v) * v;
                        }
                        direction = newDirection.Normalize();
                    }

                    XYZ startPoint = origin + direction*3;
                    XYZ endPoint= origin - direction * 3;



                    Line line = Line.CreateBound(transf.OfPoint(startPoint), transf.OfPoint(endPoint));

                    XYZ dir = new XYZ(0, 0, 1);
                    XYZ cross = direction.CrossProduct(dir);
                    // Ensure you have a valid sketch plane; for example:
                    SketchPlane sketchPlane = SketchPlane.Create(doc, Autodesk.Revit.DB.Plane.CreateByNormalAndOrigin(cross, transf.OfPoint(origin)));

                    //Create the model curve
                    doc.Create.NewModelCurve(line, sketchPlane);


                }

                t.Commit();


                return Result.Succeeded;

            }//close using


        }//close execute

     

    }//close class


}//close namespace
