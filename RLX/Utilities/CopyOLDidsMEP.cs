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
using RG = Rhino.Geometry;
#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class CopyOLDidsMEP : IExternalCommand
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
            builtInCats.Add(BuiltInCategory.OST_DuctCurves);
            builtInCats.Add(BuiltInCategory.OST_DuctFitting);
            builtInCats.Add(BuiltInCategory.OST_DuctAccessory);




            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


            IList<Element> visibleElements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();

            using (Transaction t = new Transaction(doc, "Copy old uniqueIDS"))
            {

                t.Start();

                foreach (Element element in visibleElements)
                {


                    Parameter p = element.LookupParameter("RLX_UniqueIdentifier");

                    if (p.AsValueString() != null)
                    {
                        string oldId = p.AsValueString();

                        string assetType = oldId.Substring(9, 3);

                        Parameter ds_oldId = element.LookupParameter("DS_OLD UID");

                        if (ds_oldId.AsValueString() == null || ds_oldId.AsValueString().Length == 0)
                            ds_oldId.Set(oldId);

                        Parameter ds_assetType = element.LookupParameter("DS_AssetType");

                        ds_assetType.Set(assetType);
                    }

                }

                t.Commit();
            }
                
                return Result.Succeeded;
            
        }
    }
}
