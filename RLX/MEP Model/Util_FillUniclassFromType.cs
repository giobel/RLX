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
using System.Windows;

#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class Util_FillUniclassFromType : IExternalCommand
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
            int counterModified = 0;


            using (Transaction t = new Transaction(doc, "Fill Uniclass"))
            {

                t.Start();

                foreach (var e in visibleElements)
                    {
                    try
                    {
                        Helpers.CopyUniclassFromType(doc, e);

                        counterModified++;
                    }
                    catch { }

                    }
                    
                

                t.Commit();
            }
            TaskDialog.Show("R", $"{counterModified} modified of {countElements}");
            return Result.Succeeded;
        }
    }
}
