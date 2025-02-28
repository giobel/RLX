#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
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
    public class Util_Fittings_CopyAllParameters : IExternalCommand
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

            List<BuiltInCategory> categories = new List<BuiltInCategory>()
            {
                BuiltInCategory.OST_PipeCurves,
                BuiltInCategory.OST_PipeFitting,
                BuiltInCategory.OST_DuctCurves,
                BuiltInCategory.OST_DuctFitting
            };

            ElementMulticategoryFilter filter = new ElementMulticategoryFilter(categories);

            IList<Element> visibleElements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter).WhereElementIsNotElementType().ToElements();

            var grouped = visibleElements.GroupBy(x => x.LookupParameter("RLX_UniqueIdentifier").AsValueString());

            int countElements = visibleElements.Count;
            int counterModified = 0;


            List<string> paramsToSet = new List<string>(){"RLX_ActualCost",
                    "RLX_ClassificationUniclassEF_Description","RLX_Title","RLX_Description",
                "RLX_ClassificationUniclassEF_Number","RLX_ClassificationUniclassPr_Description",
                "RLX_ClassificationUniclassPr_Number","RLX_ClassificationUniclassSs_Description",
                "RLX_ClassificationUniclassSs_Number","RLX_CoordinatesX","RLX_CoordinatesY",
                "RLX_CoordinatesZ","RLX_Facility","RLX_GridReferenceSystem","RLX_Location","RLX_MaintenanceCost",
                "RLX_Space","RLX_Specification","RLX_System","RLX_Type","RLX_UniqueIdentifier","RLX_Zone",
                "RLX_MainMaterial",
                "DS_AssetID",
                "DS_AssetType"};


            using (Transaction t = new Transaction(doc, "Copy all parameters"))
            {

                t.Start();

                foreach (var group in grouped)
                {
                    Element source = group.FirstOrDefault(e=>e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_PipeCurves || e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_DuctCurves);

                    foreach (Element e in group)
                    {

                        foreach (string paramName in paramsToSet)
                        {
                        
                                string p = source.LookupParameter(paramName).AsValueString();
                                e.LookupParameter(paramName).Set(p);
                            
                        }

                        counterModified++;
                    }

                }
                {





                    t.Commit();
                }
                TaskDialog.Show("R", $"{counterModified} modified of {countElements}");
                return Result.Succeeded;
            }
        }
    }
}
