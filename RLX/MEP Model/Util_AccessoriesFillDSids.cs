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
            int lightinModified = 0;
            int airTerminalsModified = 0;
            int electrFixturesModified = 0;
            int datadevicesModified = 0;
            int lightinDevicesModified = 0;
            int electrEquipModified = 0;
            int telephonesModified = 0;

            int ligtinDevicesCounter = 1001;
            int datadevicesCounter = 2001;
            int electrFixturesCounter = 3001;
            int electrEquipCounter = 3501;
            int airtTerminalsCounter = 4001;
            int lightingFixturesCounter = 5001;
            int ductAccessoriesCounter = 6001;
            int mecAccessoriesCounter = 7001;
            int fireAlarmCounter = 8001;
            int telephonesCounter = 8501;

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

                            case "Lighting Fixtures":
                            assetId.Set(lightingFixturesCounter.ToString());
                            lightingFixturesCounter++;
                            lightinModified++;
                            break;

                        case "Air Terminals":
                            assetId.Set(airtTerminalsCounter.ToString());
                            airtTerminalsCounter++;
                            airtTerminalsCounter++;
                            break;

                        case "Electrical Fixtures":
                            assetId.Set(electrFixturesCounter.ToString());
                            electrFixturesCounter++;
                            electrFixturesModified++;
                            break;

                        case "Data Devices":
                        assetId.Set(datadevicesCounter.ToString());
                        datadevicesCounter++;
                        datadevicesModified++;
                        break;

                        case "Lighting Devices":
                            assetId.Set(ligtinDevicesCounter.ToString());
                            ligtinDevicesCounter++;
                            lightinDevicesModified++;
                            break;

                            case "Electrical Equipment":
                            assetId.Set(electrEquipCounter.ToString());
                            electrEquipCounter++;
                            electrEquipModified++;
                            break;

                        case "Telephone Devices":
                            assetId.Set(telephonesCounter.ToString());
                            telephonesCounter++;
                            telephonesModified++;
                            break;

                    }



                }
                    
                

                t.Commit();
            }

            TaskDialog.Show("R", $"{ductModified} ducts {mecModified} mech " +
                $"fire alarm {fireAlarmModified} lighting fixtures { lightinModified} air terminals {airTerminalsModified} electr fixtures {electrFixturesModified}" +
            
                $"data devices {datadevicesModified} lighting dev {lightinDevicesModified} elect equip {electrEquipModified} telephone {telephonesModified} \nof {countElements}");

            return Result.Succeeded;
        }
    }
}
