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
using System.Security.Cryptography;
using System.Text;
using System.Windows;
#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class SelectFromExcel : IExternalCommand
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
            builtInCats.Add(BuiltInCategory.OST_PipeCurves);
            builtInCats.Add(BuiltInCategory.OST_PipeFitting);
            builtInCats.Add(BuiltInCategory.OST_PipeAccessory);
            builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);
            builtInCats.Add(BuiltInCategory.OST_Sprinklers);
            builtInCats.Add(BuiltInCategory.OST_PlaceHolderPipes);
            builtInCats.Add(BuiltInCategory.OST_GenericModel);
            builtInCats.Add(BuiltInCategory.OST_Furniture);


            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text))
            {

                string content = Clipboard.GetDataObject().GetData(DataFormats.Text).ToString();

                string formatted = content.Replace("\r\n", "");

                Element e =  new FilteredElementCollector(doc).WhereElementIsNotElementType().WherePasses(filter1)
                    .Where(x => x.LookupParameter("RLX_UniqueIdentifier").AsValueString() == formatted).ToList().First();

                ICollection<ElementId> eids = new List<ElementId>() { e.Id };

                uidoc.ShowElements(eids);

            }

            return Result.Succeeded;
            
        }
    }
}
