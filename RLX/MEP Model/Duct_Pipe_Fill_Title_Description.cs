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
    public class Duct_Pipe_Fill_Title_Description: IExternalCommand
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
            builtInCats.Add(BuiltInCategory.OST_DuctAccessory);
            builtInCats.Add(BuiltInCategory.OST_DuctCurves);
            builtInCats.Add(BuiltInCategory.OST_PipeCurves);



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

                        string categoryName = element.Category.Name;    

                        //better to use the element Pr Title. Parameter is empyt in some element types
                        //Parameter prTitle = et.LookupParameter("Identity_Classification_Uniclass 2015_Pr_Title");
                        Parameter size = element.LookupParameter("Size");
                        Parameter material = element.LookupParameter("RLX_MainMaterial");

                        string titleString = "";

                        if (size != null && size.AsValueString() != null && material != null && material.AsValueString() != null)
                        {
                            titleString += $"{categoryName} {size.AsValueString()} {material.AsValueString()}";
                        }

                            title.Set(titleString);

                        
                        Parameter description = element.LookupParameter("RLX_Description");

                        Level level = doc.GetElement(element.LevelId) as Level;
                        Parameter descriptionParam = et.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    string descriptionValue = descriptionParam.AsValueString();

                        if (!descriptionParam.HasValue)
                        {
                            Parameter elePr = et.LookupParameter("Identity_Classification_Uniclass 2015_Pr_Title");
                            if (elePr != null && elePr.HasValue)
                        {
                            descriptionValue = elePr.AsString();
                        }
                        }


                    //Parameter location = element.LookupParameter("RLX_Location");

                    string levelSentenceCase = char.ToUpper(level.Name[0]) + level.Name.Substring(1).ToLower();

                        string descrString = $"{descriptionValue} {Helpers.LocationforDescription()} { levelSentenceCase}";
                        
                            description.Set(descrString);

                        counterModified++;

                }


                t.Commit();

                    
                }

            TaskDialog.Show("R", $"{counterModified} modified of {countVisible}");

            



            return Result.Succeeded;
        }
    }
}
