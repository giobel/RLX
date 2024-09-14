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
    public class FillEmptyDescriptions : IExternalCommand
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





            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);

            IList<Element> velements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();

            using (Transaction t = new Transaction(doc, "Fill empty descriptions"))
            {
                t.Start();

                foreach (var element in velements)
                {

                    string descr = element.LookupParameter("RLX_Description").AsValueString();

                    if (descr == null || descr.Length < 3)
                    {

                        string title = element.LookupParameter("RLX_Title").AsValueString();
                        string location = element.LookupParameter("RLX_Location").AsValueString();
                        string zone = element.LookupParameter("RLX_Zone").AsValueString();
                        string space = element.LookupParameter("RLX_Space").AsValueString();

                        element.LookupParameter("RLX_Description").Set(String.Format("{0} {1} {2} {3}", title, location, zone, space));
                    }

                }

                t.Commit();
            }

            return Result.Succeeded;
            
        }
    }
}
