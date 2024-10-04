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
using RG = Rhino.Geometry;
#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class FixTitleDescription : IExternalCommand
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
            builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);
            builtInCats.Add(BuiltInCategory.OST_GenericModel);
            builtInCats.Add(BuiltInCategory.OST_PipeCurves);
            builtInCats.Add(BuiltInCategory.OST_PipeInsulations);
            builtInCats.Add(BuiltInCategory.OST_PipeFitting);
            builtInCats.Add(BuiltInCategory.OST_PipeAccessory);
            builtInCats.Add(BuiltInCategory.OST_Sprinklers);
            builtInCats.Add(BuiltInCategory.OST_Furniture);
            builtInCats.Add(BuiltInCategory.OST_FlexPipeCurves);




            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


            IList<Element> velements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();


            using (Transaction t = new Transaction(doc, "Fill params"))
            {

                t.Start();

                foreach (Element element in velements)
                {

                    Parameter spec = element.LookupParameter("RLX_Component");

                    spec.Set("");

                    Parameter titleP = element.LookupParameter("RLX_Title");

                    if (titleP.AsValueString() == null)
                    {
                        Clipboard.SetText(element.Id.ToString());
                        TaskDialog.Show("R", element.Id.ToString());
                        return Result.Failed;
                    }
                    string title = titleP.AsValueString().Replace("CW_","").Replace("Cw_","");
                    titleP.Set(title);


                    Parameter descriptionP = element.LookupParameter("RLX_Description");
                    string description = descriptionP.AsValueString().Replace("N/A", "").Replace("CW_","").Replace("Cw_", "");
                    descriptionP.Set(description);
                    




                }


                t.Commit();
            }

            return Result.Succeeded;
            
        }
    }
}
