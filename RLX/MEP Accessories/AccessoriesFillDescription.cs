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
    public class AccessoriesFillDescription: IExternalCommand
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




            IList<Element> visibleElements= new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(Helpers.RLXcatFilterAccessories()).WhereElementIsNotElementType().ToElements();
            int countVisible = visibleElements.Count();
            int counterModified = 0;

                using (Transaction t = new Transaction(doc, "Fill params"))
                {

                    t.Start();

                    foreach (Element element in visibleElements)
                    {

                        Element et = doc.GetElement(element.GetTypeId());
                        Parameter title = element.LookupParameter("RLX_Description");

                        Level level = doc.GetElement(element.LevelId) as Level;

                        Parameter descriptionParam = et.LookupParameter("Description");
                        Parameter location = element.LookupParameter("Location");

                        string titleString = "";

                        
                        
                            titleString += $"{descriptionParam.AsValueString()} {location.AsValueString()} {level.Name}";
                        

                            title.Set(titleString);
                            counterModified++;
                        



                    }


                    t.Commit();

                    
                }

            TaskDialog.Show("R", $"{counterModified} modified of {countVisible}");

            



            return Result.Succeeded;
        }
    }
}
