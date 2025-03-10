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
    public class Util_FillUniqueID : IExternalCommand
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

                foreach (var e in visibleElements)
                    {

                    //Silvertown Bld
                    //string code = "L252013B0";

                    //Silvertown Service:
                    string code = "X013013S0";

                    //Greenwich Bld
                    //string code = "L114014B0";



                    string assetType = e.LookupParameter("DS_AssetType").AsValueString();
                    string id = e.LookupParameter("DS_AssetID").AsValueString();


                    Parameter p = e.LookupParameter("RLX_UniqueIdentifier");
                    
                    if (assetType?.Count() == 3 && id?.Count() == 4)
                    {
                        p.Set(code + assetType + id);
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
