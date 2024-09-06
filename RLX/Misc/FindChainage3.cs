#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RG = Rhino.Geometry;
#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class FindChainage3 : IExternalCommand
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

            Curve alignmentCrv = alignmentLoc.Curve;


            List<XYZ> tessellation = alignmentCrv.Tessellate().ToList();

            List<RG.Point3d> rhinoPts = new List<RG.Point3d>();

            foreach (var item in tessellation)
            {
                rhinoPts.Add(Helpers.RevitToRhinoPt(item));
            }

            RG.Polyline rhinoAlignment = new RG.Polyline(rhinoPts);


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
                    LocationPoint MEPLp = element.Location as LocationPoint;
                    XYZ MEPprojectedPt = new XYZ(MEPLp.Point.X, MEPLp.Point.Y, 0);

                    IntersectionResult intersection = alignmentCrv.Project(MEPprojectedPt);

                    XYZ pointOnAlign = intersection.XYZPoint;

                    //Helpers.CreateCircle(doc, pointOnAlign, 2);

                    //RHINO
                    RG.Point3d closestPt = rhinoAlignment.ClosestPoint(Helpers.RevitToRhinoPt(MEPprojectedPt));

                    double rhinoPar = rhinoAlignment.ClosestParameter(closestPt);
                    RG.Curve[] plcurve = rhinoAlignment.ToPolylineCurve().Split(rhinoPar);

                    RG.Polyline splitAlignment = null;

                    plcurve[0].TryGetPolyline(out splitAlignment);

                    

                    TaskDialog.Show("Length", (splitAlignment.Length * 0.3048 + 680.543).ToString());


                    TaskDialog.Show("Rhino", $"{closestPt.X * 304.8} {closestPt.Y * 304.8} ");



                    //
                    //TaskDialog.Show("R", $"{pointOnAlign.X * 304.8} {pointOnAlign.Y * 304.8} ");
                    //TaskDialog.Show("R", parameter.ToString());

                    //foreach (var item in tessellation)
                    //{
                    //    Helpers.CreateCircle(doc, item, 2);
                    //}
                }

                t.Commit();

                return Result.Succeeded;
            }
        }
    }
}
