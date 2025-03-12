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
    public class FireAlarmDev_Fill_Title_Description : IExternalCommand
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
                BuiltInCategory.OST_FireAlarmDevices

            };

            
            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


            IList<Element> visibleElements= new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();
                   int countVisible = visibleElements.Count();
            int counterModified = 0;

                using (Transaction t = new Transaction(doc, "Fill params"))
                {

                    t.Start();

                    foreach (Element element in visibleElements)
                    {

                        


                    Element et = doc.GetElement(element.GetTypeId());
                        Parameter title = element.LookupParameter("RLX_Title");


                        


                    Parameter material = element.LookupParameter("RLX_MainMaterial");

                    
                    
                    string width = et.LookupParameter("Dimensions_Width")?.AsValueString();
                    string length = et.LookupParameter("Dimensions_Length")?.AsValueString();
                    
                    if (length == null || length == "")
                    {
                        length = et.LookupParameter("Dimensions_Depth_Faceplate")?.AsValueString() + "thk";
                    }
                    else
                    {
                        length = length + "L";
                    }

                    string height = et.LookupParameter("Dimensions_Height")?.AsValueString();
                    if (height == null || height == "")
                    {
                        height = et.LookupParameter("Dimensions_Height_Nominal")?.AsValueString();
                    }
                    et.LookupParameter("Dimensions_Height_Nominal")?.AsValueString();

                    string diam = et.LookupParameter("Dimensions_Diameter")?.AsValueString();

                    string size = "";

                    if (diam == null || diam == "")
                    {
                        size = $"{width}W x {length} x {height}H";
                    }
                    else
                    {
                        size = $"{diam}Dia";
                    }



                    string titleString = $"{element.Category.Name} {material.AsValueString()}";

                        string cleanedTitle = Regex.Replace(titleString, @"\s{2,}", " "); // Replaces 2+ spaces with 1


                        title.Set(cleanedTitle);
                            counterModified++;


                        Parameter description = element.LookupParameter("RLX_Description");

                        Level level = doc.GetElement(element.LevelId) as Level;

                        Parameter descriptionParam = et.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    string name = Helpers.ConvertToSentenceCase(descriptionParam.AsValueString());



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


                }


                    t.Commit();

                    
                }

            TaskDialog.Show("R", $"{counterModified} modified of {countVisible}");

            



            return Result.Succeeded;
        }
    }
}
