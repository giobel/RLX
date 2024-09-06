#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RG = Rhino.Geometry;

#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class FindChainage : IExternalCommand
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


            Reference alignmentsRef = uidoc.Selection.PickObject(ObjectType.Element, "Pick Chainages");

            Element alignment = doc.GetElement(alignmentsRef);

            

            LocationCurve alignmentLoc = alignment.Location as LocationCurve;

            Autodesk.Revit.DB.Curve alignmentCrv = alignmentLoc.Curve;

            List<XYZ> alignmentPts = alignmentCrv.Tessellate().ToList();

            List<Point3d> rgPts = new List<Point3d>();

            foreach (XYZ point in alignmentPts)
            {
                rgPts.Add(new Point3d(point.X, point.Y, point.Z));
            }

            RG.Polyline pl1 = new RG.Polyline(rgPts);

            TaskDialog.Show("Original length", (pl1.Length*0.3048).ToString());

            PolyLine pl = PolyLine.Create(alignmentPts); 
                       

            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
            builtInCats.Add(BuiltInCategory.OST_PipeAccessory);
            builtInCats.Add(BuiltInCategory.OST_Sprinklers);
            builtInCats.Add(BuiltInCategory.OST_GenericModel);
            builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);

            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);

            IList<Element> mepObjects = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();

            using (Transaction t = new Transaction(doc, "Place circles"))
            {

                t.Start();

                foreach (Element element in mepObjects)
                {
                    LocationPoint lp = element.Location as LocationPoint;
                    XYZ projectedPt = new XYZ(lp.Point.X, lp.Point.Y, 0);
                    int closestPtIndex = Helpers.FindClosestPointIndex(projectedPt, alignmentPts);
                    XYZ curveClosestVertex = alignmentPts[closestPtIndex];


                    XYZ pointOnCurve = alignmentCrv.Project(projectedPt).XYZPoint;


                    Helpers.CreateCircle(doc, alignmentPts[closestPtIndex], 4);


                    //check if point is before or after the closest point index


                    List<XYZ> splitLits = alignmentPts.GetRange(0, closestPtIndex);

                    List<Point3d> splitPoints = new List<Point3d>();

                    foreach (XYZ point in splitLits)
                    {
                        splitPoints.Add(new Point3d(point.X, point.Y, point.Z));

                        Helpers.CreateCircle(doc, point, 1);
                    }


                    RG.Polyline plSplit = new RG.Polyline(splitPoints);

                    

                    double l = plSplit.Length * 0.3048 + 680.543;
                
                    TaskDialog.Show("Split length", "Length " +  l.ToString());
                    


                    Helpers.CreateCircle(doc, pointOnCurve, 1);

                    TaskDialog.Show("PL Length", (Helpers.ComputePolylineLength(splitLits) + 680.543).ToString());
                }


                t.Commit();
            }
                return Result.Succeeded;
        }
    }
}
