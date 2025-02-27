#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;

#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class Util_AccessoriesFillDSids : IExternalCommand
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

            int countElements = visibleElements.Count;
            int ductModified = 0;
            int mecModified = 0;
            int fireAlarmModified= 0;


            int ductAccessoriesCounter = 6001;
            int mecAccessoriesCounter = 7001;
            int fireAlarmCounter = 8001;

            using (Transaction t = new Transaction(doc, "Fill DS Ids"))
            {

                t.Start();

                foreach (var e in visibleElements)
                    {

                    Parameter assetId = e.LookupParameter("DS_AssetID");

                    string assetIdvalue = assetId.AsValueString();

                    switch (e.Category.Name ){

                        case "Duct Accessories":
                            assetId.Set(ductAccessoriesCounter.ToString());
                            ductAccessoriesCounter++;
                            ductModified++;
                            break;

                        case "Mechanical Equipment":
                            assetId.Set(mecAccessoriesCounter.ToString());
                            mecAccessoriesCounter++;
                            mecModified++;
                            break;

                        case "Fire Alarm Devices":
                            assetId.Set(fireAlarmCounter.ToString());
                            fireAlarmCounter++;
                            fireAlarmModified++;
                            break;



                    }



                }
                    
                

                t.Commit();
            }

            TaskDialog.Show("R", $"{ductModified} ducts {mecModified} mech " +
                $"fire alarm {fireAlarmModified} modified" +
                $"\nof {countElements}");

            return Result.Succeeded;
        }
    }
}
