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
using System.Text.RegularExpressions;
using static Autodesk.Revit.DB.SpecTypeId;

#endregion 

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class ElectricalEquip_Dependant_FixParam : IExternalCommand
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


            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>
            {
                BuiltInCategory.OST_ElectricalEquipment

            };

            
            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


            IList<Element> visibleElements= new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();
                   int countVisible = visibleElements.Count();
            int counterModified = 0;

                using (Transaction t = new Transaction(doc, "Fill air terminals params"))
                {

                    t.Start();

                    foreach (Element element in visibleElements)
                    {

                    List<string> paramsToSet = new List<string>(){"RLX_ActualCost",
                    "RLX_ClassificationUniclassEF_Description","RLX_Title",
                "RLX_ClassificationUniclassEF_Number","RLX_ClassificationUniclassPr_Description",
                "RLX_ClassificationUniclassPr_Number","RLX_ClassificationUniclassSs_Description",
                "RLX_ClassificationUniclassSs_Number","RLX_CoordinatesX","RLX_CoordinatesY",
                "RLX_CoordinatesZ","RLX_Facility","RLX_GridReferenceSystem","RLX_Location","RLX_MaintenanceCost",
                "RLX_Space","RLX_Specification","RLX_System","RLX_Type","RLX_UniqueIdentifier","RLX_Zone",
                "DS_AssetID",
                "DS_AssetType"};




                    IList<ElementId> dependents =   element.GetDependentElements(null);

                    foreach (var item in dependents)
                    {
                        Element dep = doc.GetElement(item);


                        foreach (string paramName in paramsToSet)
                        {
                            //					TaskDialog.Show("R", paramName);
                            string p = element.LookupParameter(paramName).AsValueString();
                            dep.LookupParameter(paramName).Set(p);


                        }
                        counterModified++;

                    }

                }


                    t.Commit();

                    
                }

            TaskDialog.Show("R", $"{counterModified} modified of {countVisible}");

            



            return Result.Succeeded;
        }
    }
}
