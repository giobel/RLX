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
    public class HangerFittingsCopyClosestPipeParamsBBox : IExternalCommand
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

            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>() { BuiltInCategory.OST_MechanicalEquipment, BuiltInCategory.OST_PipeFitting};

            ElementMulticategoryFilter catFilter = new ElementMulticategoryFilter(builtInCats);

            IList<Element> mechEquipments = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(catFilter).WhereElementIsNotElementType().ToElements();


                List<string> paramsToSet = new List<string>(){"RLX_ActualCost","RLX_ClassificationUniclassEF_Description",
                "RLX_ClassificationUniclassEF_Number","RLX_ClassificationUniclassPr_Description",
                "RLX_ClassificationUniclassPr_Number","RLX_ClassificationUniclassSs_Description",
                "RLX_ClassificationUniclassSs_Number","RLX_Component","RLX_CoordinatesX","RLX_CoordinatesY",
                "RLX_CoordinatesZ","RLX_Facility","RLX_GridReferenceSystem","RLX_Location","RLX_MaintenanceCost",
                "RLX_Space","RLX_Specification","RLX_System","RLX_Type","RLX_UniqueIdentifier","RLX_Zone","RLX_MainMaterial",
                "DS_AssetID",
                "DS_AssetType",
                "DS_Axis",
                "DS_Lane",
                "DS_Chainage",
                "DS_Location"};

                using (Transaction t = new Transaction(doc, "Pipe fittings copy pipe parameters"))
                {
                    t.Start();

                    foreach (Element mecEquip in mechEquipments)
                    {

                        LocationPoint point = mecEquip.Location as LocationPoint;

                        BoundingBoxXYZ mecBbox = mecEquip.get_BoundingBox(doc.ActiveView);

                        Outline outline = new Outline(mecBbox.Min, mecBbox.Max);

                        BoundingBoxIntersectsFilter filter = new BoundingBoxIntersectsFilter(outline);

                    ElementSlowFilter intersectElement = new ElementIntersectsElementFilter(mecEquip);

                    //first element is the mech equipment itself
                    IList<Element> intersectedPipe = new FilteredElementCollector(doc, doc.ActiveView.Id)
                                                            .OfCategory(BuiltInCategory.OST_PipeCurves)
                                                            .WherePasses(intersectElement).ToElements();

                        

                        if (intersectedPipe.Count > 0)
                        {
                        Element pipe = intersectedPipe.Where(x => x.Id != mecEquip.Id).First();

                        //update only if the uniqueId is empty
                        string mechEquipUniqueId = mecEquip.LookupParameter("RLX_UniqueIdentifier").AsValueString();
                        if (mechEquipUniqueId == null || mechEquipUniqueId.Length < 3)
                        {

                            foreach (string paramName in paramsToSet)
                            {
                                //					TaskDialog.Show("R", paramName);
                                Parameter p = pipe.LookupParameter(paramName);

                                if (p.StorageType == StorageType.String)
                                    mecEquip.LookupParameter(paramName).Set(p.AsValueString());

                                if (p.StorageType == StorageType.ElementId)
                                    mecEquip.LookupParameter(paramName).Set(p.AsElementId());

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
