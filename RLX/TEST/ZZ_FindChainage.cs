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
    public class ZZ_FindChainage : IExternalCommand
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

            /*
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
            */

            IList<Reference> alignmentsRef = uidoc.Selection.PickObjects(ObjectType.Element, "Pick curves");


            List<XYZ> alignmentPts = new List<XYZ>();

            List<RG.Point3d> rhinoPts = new List<RG.Point3d>();

            foreach (var item in alignmentsRef)
            {
                Element alignment = doc.GetElement(item);

                LocationCurve locationCurve = alignment.Location as LocationCurve;
  
                foreach (XYZ pt in locationCurve.Curve.Tessellate())
                {
                    alignmentPts.Add(pt);
                    rhinoPts.Add(new RG.Point3d(pt.X, pt.Y, pt.Z));
                }
            }


            RG.Polyline pl1 = new RG.Polyline(rhinoPts);

            RG.NurbsCurve alignmentNurbs = NurbsCurve.Create(false, 3, rhinoPts);


            TaskDialog.Show("Original length", (pl1.Length*0.3048).ToString());

            
                       

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

                    RG.Point3d rhinoMepPt = new RG.Point3d(projectedPt.X, projectedPt.Y, 0);

                    int closestPtIndex = Helpers.FindClosestPointIndex(projectedPt, alignmentPts);
                    XYZ curveClosestVertex = alignmentPts[closestPtIndex];

                    RG.Point3d polPt = pl1.ClosestPoint(rhinoMepPt);

                    double param = pl1.ClosestParameter(rhinoMepPt);

                    NurbsCurve ns = pl1.ToNurbsCurve();
                    RG.Curve[] splits = ns.Split(param);

               
                    //XYZ pointOnCurve = alignmentCrv.Project(projectedPt).XYZPoint;

                    Helpers.CreateCircle(doc, Helpers.RhinoToRevitPt(polPt), 4);

                    Helpers.CreateCircle(doc, alignmentPts[closestPtIndex], 4);

                    TaskDialog.Show("PL Length", (680.543).ToString());

                    //check if point is before or after the closest point index


                    //List<XYZ> splitLits = alignmentPts.GetRange(0, closestPtIndex);

                    //List<Point3d> splitPoints = new List<Point3d>();

                    //foreach (XYZ point in splitLits)
                    //{
                    //    splitPoints.Add(new Point3d(point.X, point.Y, point.Z));

                    //    Helpers.CreateCircle(doc, point, 1);
                    //}


                    //RG.Polyline plSplit = new RG.Polyline(splitPoints);



                    //double l = plSplit.Length * 0.3048 + 680.543;


                    //TaskDialog.Show("PL Length", (Helpers.ComputePolylineLength(splitLits) + 680.543).ToString());
                }


                t.Commit();
            }
                return Result.Succeeded;
        }
    }
}
