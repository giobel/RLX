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
    public class CabinetsSupportsCopyParamsBBox : IExternalCommand
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

            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>() { BuiltInCategory.OST_GenericModel};

            ElementMulticategoryFilter catFilter = new ElementMulticategoryFilter(builtInCats);

            IList<Element> cabinetsSupport = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(catFilter).WhereElementIsNotElementType().ToElements();


                List<string> paramsToSet = new List<string>(){"RLX_ActualCost","RLX_ClassificationUniclassEF_Description",
                "RLX_ClassificationUniclassEF_Number","RLX_ClassificationUniclassPr_Description",
                "RLX_ClassificationUniclassPr_Number","RLX_ClassificationUniclassSs_Description",
                "RLX_ClassificationUniclassSs_Number","RLX_Component","RLX_CoordinatesX","RLX_CoordinatesY",
                "RLX_CoordinatesZ","RLX_Facility","RLX_GridReferenceSystem","RLX_Location","RLX_MaintenanceCost",
                "RLX_Space","RLX_Specification","RLX_System","RLX_Type","RLX_UniqueIdentifier","RLX_Zone","RLX_MainMaterial",
                "RLX_Title",
                "DS_AssetID",
                "DS_AssetType",
                "DS_Axis",
                "DS_Lane",
                "DS_Chainage",
                "DS_Location"};

                using (Transaction t = new Transaction(doc, "Cabinets supports copy parameters"))
                {
                    t.Start();

                    foreach (Element support in cabinetsSupport)
                    {
                        BoundingBoxXYZ supportBbox = support.get_BoundingBox(doc.ActiveView);

                        Outline outline = new Outline(supportBbox.Min, supportBbox.Max);

                        BoundingBoxIntersectsFilter filter = new BoundingBoxIntersectsFilter(outline);

                    ElementSlowFilter intersectElement = new ElementIntersectsElementFilter(support);

                    //first element is the mech equipment itself
                    IList<Element> intersectedCabinet = new FilteredElementCollector(doc, doc.ActiveView.Id)
                                                            .OfCategory(BuiltInCategory.OST_Furniture)
                                                            .WherePasses(filter).ToElements();

                        

                        if (intersectedCabinet.Count > 0)
                        {
                        Element pipe = intersectedCabinet.Where(x => x.Id != support.Id).First();

                        //update only if the uniqueId is old id
                        string mechEquipUniqueId = support.LookupParameter("RLX_UniqueIdentifier").AsValueString();
                        if (mechEquipUniqueId.Length == 20)
                        {

                            foreach (string paramName in paramsToSet)
                            {
                                //					TaskDialog.Show("R", paramName);
                                Parameter p = pipe.LookupParameter(paramName);

                                if (p.StorageType == StorageType.String)
                                    support.LookupParameter(paramName).Set(p.AsValueString());

                                if (p.StorageType == StorageType.ElementId)
                                    support.LookupParameter(paramName).Set(p.AsElementId());

                            }
                        }
                        }
                    }

                
                    t.Commit();
                }//close transaction


            


            return Result.Succeeded;
        }
    }
}
