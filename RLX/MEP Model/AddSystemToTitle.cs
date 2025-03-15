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
    public class AddSystemToTitle : IExternalCommand
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



            IList<Element> visibleElements= new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(Helpers.RLXcatFilter()).WhereElementIsNotElementType().ToElements();
            
            int countVisible = visibleElements.Count();
            int counterModified = 0;

                using (Transaction t = new Transaction(doc, "Fill air terminals params"))
                {

                    t.Start();

                    foreach (Element element in visibleElements)
                    {

                    string system = element.LookupParameter("RLXSystem")?.AsValueString();

                    if (system == null || system == "")
                    {
                        system = element.LookupParameter("System Type")?.AsValueString();
                    }

                    Parameter titleparam = element.LookupParameter("RLX_Title");

                    string title = titleparam?.AsString();

                    if (title != null && system !=null && !title.Contains(system))
                    {
                        title += $" {system}";
                        
                        titleparam.Set(title);

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
