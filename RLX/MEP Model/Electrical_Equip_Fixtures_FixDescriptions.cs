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
    public class Electrical_Equip_Fixtures_FixDescriptions : IExternalCommand
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
                BuiltInCategory.OST_ElectricalEquipment,
                BuiltInCategory.OST_ElectricalFixtures,

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


                    Element et = doc.GetElement(element.GetTypeId());

                    Parameter description = element.LookupParameter("RLX_Description");

                    Level level = doc.GetElement(element.LevelId) as Level;

                    Parameter descriptionParam = et.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    string name = Helpers.ConvertToSentenceCase(descriptionParam.AsValueString()).Replace("X", "x");



                    if (name == "")
                    {
                        string famName = element.LookupParameter("Family").AsValueString();
                        name = famName.Replace("Arup", "").Replace("_", " ").Replace("GB", "");


                    }
                    else
                    {
                        try
                        {
                            name = name.Split(new string[] { "mm " }, StringSplitOptions.None)[1].Replace("diameter", "");
                        }
                        catch
                        {
                            //do nothing
                        }
                    }


                    string levelSentenceCase = char.ToUpper(level.Name[0]) + level.Name.Substring(1).ToLower();

                    string descriptionString = $"{name} {Helpers.LocationforDescription()} {levelSentenceCase}";

                    //string cleanedDescr = Regex.Replace(descriptionString, @"\s{2,}", " "); // Replaces 2+ spaces with 1


                    //cleanedDescr = Helpers.ConvertToSentenceCase(cleanedDescr);

                    description.Set(descriptionString);


                    counterModified++;



                }
                


                    t.Commit();

                    
                }

            TaskDialog.Show("R", $"{counterModified} modified of {countVisible}");

            



            return Result.Succeeded;
        }
    }
}
