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
    public class FillUniqueId : IExternalCommand
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
            builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);
            builtInCats.Add(BuiltInCategory.OST_PipeAccessory);
            builtInCats.Add(BuiltInCategory.OST_Sprinklers);
            builtInCats.Add(BuiltInCategory.OST_GenericModel);
            builtInCats.Add(BuiltInCategory.OST_PipeFitting);
            builtInCats.Add(BuiltInCategory.OST_Furniture);





            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


            IList<Element> visibleElements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();


            using (Transaction t = new Transaction(doc, "Fill uniqueIDS"))
            {

                t.Start();

                foreach (Element element in visibleElements)
                {

                    string axis = element.LookupParameter("DS_Axis").AsValueString();

                    string chainageParam = element.LookupParameter("DS_Chainage").AsValueString();

                    if (chainageParam == null)
                    {
                        IList<ElementId> selection = new List<ElementId>();
                        selection.Add(element.Id);
                        uidoc.Selection.SetElementIds(selection);
                        //TaskDialog.Show("R", element.Id.ToString());
                        return Result.Failed;
                    }

                    double ch = double.Parse(element.LookupParameter("DS_Chainage").AsValueString());

                    

                    // client instruction, no rounding to nearest 10
                    //					string chainage = (Math.Round(ch / 10.0) * 10).ToString();
                    string chainage = Math.Round(ch).ToString();
                    string location = element.LookupParameter("DS_Location").AsValueString();
                    string lane = element.LookupParameter("DS_Lane").AsValueString();
                    string assetType = element.LookupParameter("DS_AssetType").AsValueString();
                    string id = element.LookupParameter("DS_AssetID").AsValueString();



                    Parameter p = element.LookupParameter("RLX_UniqueIdentifier");

                    //						if (p.AsValueString() == null || p.AsValueString().Length < 3){

                    p.Set(axis + chainage + location + lane + assetType + id);

                    //						}
                }


                t.Commit();
            }



            return Result.Succeeded;
        }
    }
}
