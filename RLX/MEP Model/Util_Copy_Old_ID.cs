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
    public class Util_Copy_Old_ID : IExternalCommand
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


            using (Transaction t = new Transaction(doc, "Fill Unique Ids"))
            {

                t.Start();

                foreach (var element in visibleElements)
                {

                    Parameter p = element.LookupParameter("RLX_UniqueIdentifier");

                    //Silvertown Bld
                    string old_code = "M260013B0";

                    if (p != null && p.AsValueString() != null && p.AsValueString().Contains(old_code))
                    {
                        string oldId = p.AsValueString();

                        string assetType = oldId.Substring(9, 3);

                        Parameter ds_oldId = element.LookupParameter("DS_OLD UID");

                        if (ds_oldId.AsValueString() == null || ds_oldId.AsValueString().Length == 0)
                            ds_oldId.Set(oldId);

                        Parameter ds_assetType = element.LookupParameter("DS_AssetType");

                        ds_assetType.Set(assetType);

                        counterModified++;
                    }



                }
                    
                

                t.Commit();
            }
            TaskDialog.Show("R", $"{counterModified} modified of {countElements}");
            return Result.Succeeded;
        }
    }
}
