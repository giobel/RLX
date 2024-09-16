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
    public class FillCommonParams : IExternalCommand
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



            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


            IList<Element> velements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();

            List<Tuple<string, string>> data = new List<Tuple<string, string>>();
            data.Add(new Tuple<string, string>("RLX_ClassificationUniclassEF_Description", "Fire-extinguishing supply"));
            data.Add(new Tuple<string, string>("RLX_ClassificationUniclassEF_Number", "EF_55_30"));
            data.Add(new Tuple<string, string>("RLX_ClassificationUniclassSs_Description", "Fire-stopping systems"));
            data.Add(new Tuple<string, string>("RLX_ClassificationUniclassSs_Number", "Ss_25_60_30"));


            using (Transaction t = new Transaction(doc, "Fill params"))
            {

                t.Start();

                foreach (Element element in velements)
                {

                    Parameter fac = element.LookupParameter("RLX_Facility");
                    fac.Set("Silvertown Tunnel");

                    Parameter typ = element.LookupParameter("RLX_Type");
                    typ.Set("Project Facilites");

                    Parameter grid = element.LookupParameter("RLX_GridReferenceSystem");
                    grid.Set("BNG");

                    Parameter cost = element.LookupParameter("RLX_MaintenanceCost");
                    cost.Set("N/A");

                    Parameter actualCost = element.LookupParameter("RLX_ActualCost");
                    actualCost.Set("N/A");

                    Parameter system = element.LookupParameter("RLX_System");
                    system.Set("Fire Fighting");

                    //					Parameter comp = element.LookupParameter("RLX_Component");
                    //					comp.Set("ST150030-COW-FRS-40-ZZ-REQ-FE-0006");


                    //					Parameter sys = element.LookupParameter("RLX_System");
                    //					sys.Set("Fire System_Fixed fire-fighting system");

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
