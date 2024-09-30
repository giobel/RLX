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

#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class FillXYZ : IExternalCommand
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


            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
            builtInCats.Add(BuiltInCategory.OST_PipeCurves);
            builtInCats.Add(BuiltInCategory.OST_PipeAccessory);
            builtInCats.Add(BuiltInCategory.OST_Sprinklers);
            builtInCats.Add(BuiltInCategory.OST_GenericModel);
            builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);
            builtInCats.Add(BuiltInCategory.OST_Furniture);
            builtInCats.Add(BuiltInCategory.OST_PipeFitting);


            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);

            IList<Element> visibleElements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();

            var grouped = visibleElements.GroupBy(x => x.LookupParameter("DS_AssetID").AsValueString());

            XYZ pbp = BasePoint.GetProjectBasePoint(doc).Position;
            XYZ sp = BasePoint.GetSurveyPoint(doc).Position;

            // Calculate the offset from project base point to survey point
            XYZ offset = sp - pbp;


            Transform surveyTransf = doc.ActiveProjectLocation.GetTotalTransform();

            ProjectLocation pl = doc.ActiveProjectLocation;
            Transform ttr = pl.GetTotalTransform().Inverse;

            using (Transaction t = new Transaction(doc, "Fill XYZ"))
            {

                t.Start();

                foreach (var element in grouped)
                {

                    //XYZ cen = Helpers.GetElementCentroid(element.First());

                    //XYZ newC = surveyTransf.OfPoint(cen);
                    //					TaskDialog.Show("R", String.Format("{0} {1} {2}", newC.X, newC.Y, newC.Z));

                    //					XYZ centroid = CalculateCentroidOfElements(element) - offset;

                    XYZ centroid = ttr.OfPoint(Helpers.CalculateCentroidOfElements(element));

                    double metricX = UnitUtils.ConvertFromInternalUnits(centroid.X, UnitTypeId.Meters);
                    double metricY = UnitUtils.ConvertFromInternalUnits(centroid.Y, UnitTypeId.Meters);
                    double metricZ = UnitUtils.ConvertFromInternalUnits(centroid.Z, UnitTypeId.Meters);

                    //TaskDialog.Show("R", String.Format("{0} {1} {2}", metricX, metricY, metricZ));

                    Helpers.FillXYZParam(element, metricX, metricY, metricZ);

                }

                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
