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
    public class FindChainagePipes : IExternalCommand
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

            string alignmentName = doc.GetElement(alignmentCrv.GraphicsStyleId).Name;

            //TaskDialog.Show("R", alignmentName);

            double chainageStart = 0;

            if (alignmentName == "SOUTH BOUND")
            {
                chainageStart = 680;
            }
            else if (alignmentName == "NORTH BOUND")
            {
                chainageStart = 680.543;
            }

            List<XYZ> tessellation = alignmentCrv.Tessellate().ToList();

            List<RG.Point3d> rhinoPts = new List<RG.Point3d>();

            foreach (var item in tessellation)
            {
                rhinoPts.Add(Helpers.RevitToRhinoPt(item));
            }

            RG.Polyline rhinoAlignment = new RG.Polyline(rhinoPts);


            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
            builtInCats.Add(BuiltInCategory.OST_PipeCurves);
            


            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);

            IList<Element> allPipes = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();

            var grouped = allPipes.GroupBy(x => x.LookupParameter("DS_AssetID").AsValueString());


            using (Transaction t = new Transaction(doc, "Find closest chainage"))
            {

                t.Start();

                foreach (var items in grouped)
                {
                    List<double> chainages = new List<double>();

                    //all pipes that have the same id
                    foreach (Element element in items)
                    {

                        LocationCurve locationCurve = element.Location as LocationCurve;

                        if (locationCurve == null)
                        {
                            TaskDialog.Show("Error", "Some pipes do not have a location curve. Please hide them from the view");
                            return Result.Failed;
                        }

                        XYZ stPt = locationCurve.Curve.GetEndPoint(0);
                        XYZ endPt = locationCurve.Curve.GetEndPoint(1);


                        chainages.Add(Helpers.FindChainage(stPt, rhinoAlignment, chainageStart));

                        chainages.Add(Helpers.FindChainage(endPt, rhinoAlignment, chainageStart));

                    }

                    double chainage = 0;

                    if (alignmentName == "SOUTH BOUND")
                    {
                        chainage = chainages.Max();
                    }
                    else if (alignmentName == "NORTH BOUND")
                    {
                        chainage = chainages.Min();
                    }

                    foreach (Element element in items)
                    {
                        element.LookupParameter("DS_Chainage").Set(chainage.ToString());
                    }
                }

                t.Commit();

                return Result.Succeeded;
            }
        }
    }
}
