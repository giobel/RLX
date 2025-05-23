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


            Reference sourceBeam = uidoc.Selection.PickObject(ObjectType.Element, "Select Beam");
            Element ele = doc.GetElement(sourceBeam.ElementId);

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

                                }

                            }

                            var facesByArea = planarFaces
                                .GroupBy(x => Math.Round(x.Area, 3));

                            var smallestArea = facesByArea
                                .OrderBy(item => item.Key)
                                .FirstOrDefault().ToList();

                            PlanarFace face1 = smallestArea[0] as PlanarFace;
                            XYZ stPt = transf.OfPoint( face1.Origin );

                            PlanarFace face2 = smallestArea[1] as PlanarFace;
                            XYZ endPt = transf.OfPoint( face2.Origin );

                            Line line = Line.CreateBound(stPt, endPt);


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
