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
    public class Util_Dependents_CopyAllParameters : IExternalCommand
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




            IList<Element> visibleElements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(Helpers.RLXcatFilter()).WhereElementIsNotElementType().ToElements();

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


            using (Transaction t = new Transaction(doc, "Dependents copy all parameters"))
            {

                t.Start();

                foreach (var group in grouped)
                {
                    Element source = group.FirstOrDefault(e=>e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_PipeCurves || e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_DuctCurves);

                    foreach (Element e in group)

                        foreach (ElementId subElementId in e.GetDependentElements(null))
                        {
                            Element subElement = doc.GetElement(subElementId);
                            foreach (string paramName in paramsToSet)
                            {
                                try
                                {
                                    string p = source.LookupParameter(paramName).AsValueString();
                                    subElement.LookupParameter(paramName).Set(p);
                                }
                                catch { }
                            }

                            counterModified++;
                        }



                }

                    t.Commit();
                }
                TaskDialog.Show("R", $"{counterModified} modified of {countElements}");
                return Result.Succeeded;
            
            
        }
    }
}
