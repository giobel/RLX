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
using System.Windows;


#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class Duct_DuctFittings_Pipe_Conduit_Fill_Title_Description: IExternalCommand
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
                BuiltInCategory.OST_DuctFitting,
                BuiltInCategory.OST_DuctCurves,
                BuiltInCategory.OST_PipeCurves,
                BuiltInCategory.OST_PipeFitting,
                BuiltInCategory.OST_Conduit,
                BuiltInCategory.OST_ConduitFitting
            };



            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


            IList<Element> visibleElements= new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();

            var grouped = visibleElements.GroupBy(x => x.LookupParameter("RLX_UniqueIdentifier").AsValueString());

            int countVisible = visibleElements.Count();
            int counterModified = 0;

                using (Transaction t = new Transaction(doc, "Fill params"))
                {

                    t.Start();


                foreach (var group in grouped)
                {

                    Element source = null;

                    foreach (Element item in group)
                    {
                        if (!item.Category.Name.Contains("Fitting"))
                        {
                            source = item;
                            break;
                        }
                    }


                    Element et = doc.GetElement(source.GetTypeId());
                    

                    string categoryName = source.Category.Name;

                    //better to use the element Pr Title. Parameter is empyt in some element types
                    //Parameter prTitle = et.LookupParameter("Identity_Classification_Uniclass 2015_Pr_Title");
                    Parameter size = source.LookupParameter("Size");
                    Parameter material = source.LookupParameter("RLX_MainMaterial");

                    string titleString = "";

                    if (size != null && size.AsValueString() != null && material != null && material.AsValueString() != null)
                    {
                        titleString += $"{categoryName} {size.AsValueString()} {material.AsValueString()}";
                    }

                   

                    

                    Level level = doc.GetElement(source.LevelId) as Level;
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
                        
                    string descrString = $"{descriptionValue} {Helpers.LocationforDescription(uidoc)} {levelSentenceCase}";


                    string uniclassDescr = source.LookupParameter("RLX_ClassificationUniclassSs_Description").AsValueString();
                    string uniclassNumber = source.LookupParameter("RLX_ClassificationUniclassSs_Number").AsValueString();
                    string uniclassPrDescr = source.LookupParameter("RLX_ClassificationUniclassPr_Description").AsValueString();
                    string uniclassPrNumber = source.LookupParameter("RLX_ClassificationUniclassPr_Number").AsValueString();


                    foreach (Element element in group)
                    {

                        Parameter title = element.LookupParameter("RLX_Title");
                        Parameter description = element.LookupParameter("RLX_Description");

                        Parameter eleUniclassDescr = element.LookupParameter("RLX_ClassificationUniclassSs_Description");
                        Parameter eleUniclassNumber = element.LookupParameter("RLX_ClassificationUniclassSs_Number");

                        Parameter eleUniclassPrDescr = element.LookupParameter("RLX_ClassificationUniclassPr_Description");
                        Parameter eleUniclassPrNumber = element.LookupParameter("RLX_ClassificationUniclassPr_Number");


                        title.Set(titleString);

                        description.Set(descrString);
                        
                        eleUniclassDescr.Set(uniclassDescr);
                        eleUniclassNumber.Set(uniclassNumber);

                        eleUniclassPrDescr.Set(uniclassPrDescr);
                        eleUniclassPrNumber.Set(uniclassPrNumber);

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
