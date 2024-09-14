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
using RG = Rhino.Geometry;
#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class FillPipesAndHangersUniclassParams : IExternalCommand
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
            builtInCats.Add(BuiltInCategory.OST_PipeInsulations);
            builtInCats.Add(BuiltInCategory.OST_PipeCurves);
            builtInCats.Add(BuiltInCategory.OST_PipeInsulations);
            builtInCats.Add(BuiltInCategory.OST_PipeFitting);
            builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);

            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


            IList<Element> velements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();


            List<Tuple<string, string>> data = new List<Tuple<string, string>>();
            data.Add(new Tuple<string, string>("RLX_ClassificationUniclassEF_Description", "Fire-extinguishing supply"));
            data.Add(new Tuple<string, string>("RLX_ClassificationUniclassEF_Number", "EF_55_30"));
            data.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Description", "Pipes and fittings"));
            data.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Number", "Pr_65_52_63"));
            data.Add(new Tuple<string, string>("RLX_ClassificationUniclassSs_Description", "Fire-stopping systems"));
            data.Add(new Tuple<string, string>("RLX_ClassificationUniclassSs_Number", "Ss_25_60_30"));
            data.Add(new Tuple<string, string>("DS_AssetType", "PPW"));


            using (Transaction t = new Transaction(doc, "Fill uniclasses"))
            {

                t.Start();

                foreach (Element element in velements)
                {

                    foreach (var info in data)
                    {

                        Parameter p = element.LookupParameter(info.Item1);

                        p.Set(info.Item2);
                    }

                }


                t.Commit();
            }
            return Result.Succeeded;
            
        }
    }
}
