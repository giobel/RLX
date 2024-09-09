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
    public class CopyOLDids : IExternalCommand
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


            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


            IList<Element> visibleElements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();


            using (Transaction t = new Transaction(doc, "Copy old uniqueIDS"))
            {

                t.Start();

                foreach (Element element in visibleElements)
                {

                    if (element.Name != "Reference Point")
                    {

                        Parameter p = element.LookupParameter("RLX_UniqueIdentifier");

                        if (p.AsValueString() != null)
                        {
                            string oldId = p.AsValueString();

                            if (oldId.Length == 20)
                            {

                                string axis = oldId.Substring(0, 1);
                                string location = oldId.Substring(5, 3);
                                string lane = oldId.Substring(8, 1);
                                string assetType = oldId.Substring(9, 3);

                                Parameter ds_oldId = element.LookupParameter("DS_OLD UID");

                                if (ds_oldId.AsValueString() == null || ds_oldId.AsValueString().Length == 0)
                                    ds_oldId.Set(oldId);

                                Parameter ds_axis = element.LookupParameter("DS_Axis");

                                if (ds_axis.AsValueString() == null || ds_axis.AsValueString().Length == 0)
                                {
                                    ds_axis.Set(axis);
                                }

                                Parameter ds_location = element.LookupParameter("DS_Location");

                                if (ds_location.AsValueString() == null || ds_location.AsValueString().Length == 0)
                                    ds_location.Set(location);

                                Parameter ds_assetType = element.LookupParameter("DS_AssetType");

                                if (ds_assetType.AsValueString() == null || ds_assetType.AsValueString().Length == 0)
                                    ds_assetType.Set(assetType);

                                Parameter ds_lane = element.LookupParameter("DS_Lane");

                                if (ds_lane.AsValueString() == null || ds_lane.AsValueString().Length == 0)
                                    ds_lane.Set(lane);
                            }
                        }
                    }

                }

                t.Commit();
            }

            return Result.Succeeded;
            
        }
    }
}
