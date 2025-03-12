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
    public class Pipe_TagNextAssetID : IExternalCommand
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


            //ADD OTHER CATEGORIES HERE!!!

            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>
            {

                BuiltInCategory.OST_PipeCurves,
                BuiltInCategory.OST_PipeFitting,
                BuiltInCategory.OST_DuctCurves,
                BuiltInCategory.OST_DuctFitting,
                BuiltInCategory.OST_Conduit,
                BuiltInCategory.OST_ConduitFitting,
            };



            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


            IList<Element> visibleElements= new FilteredElementCollector(doc).WherePasses(filter1).WhereElementIsNotElementType().ToElements();


            int latestId = 0;

            foreach (Element element in visibleElements) {

                Parameter assetId = element.LookupParameter("DS_AssetID");

                if (assetId != null && assetId.HasValue)
                {
                    int assetIdValue = int.Parse(assetId.AsString());

                    if (assetIdValue > latestId)
                    {
                        latestId = assetIdValue;
                    }
                }

            }

            IList<Element> selected = uidoc.Selection.GetElementIds().Select(x => doc.GetElement(x)).ToList();

            int counterModified = 0;

                using (Transaction t = new Transaction(doc, "Fill params"))
                {

                t.Start();

                foreach (Element item in selected)
                {
                    item.LookupParameter("DS_AssetID").Set((latestId + 1).ToString());
                    counterModified++;
                }


                t.Commit();

                    
                }


            



            return Result.Succeeded;
        }
    }
}
