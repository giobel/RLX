#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
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
    public class CopyParamsToSubcomponents : IExternalCommand
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
            builtInCats.Add(BuiltInCategory.OST_PipeAccessory);
            builtInCats.Add(BuiltInCategory.OST_Sprinklers);
            builtInCats.Add(BuiltInCategory.OST_GenericModel);
            builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);
            builtInCats.Add(BuiltInCategory.OST_PipeFitting);
            builtInCats.Add(BuiltInCategory.OST_PipeCurves);
            builtInCats.Add(BuiltInCategory.OST_Furniture);

            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);

            ICollection<ElementId> visibleElements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElementIds();

                List<string> paramsToSet = new List<string>(){"RLX_ActualCost",
                    "RLX_ClassificationUniclassEF_Description","RLX_Title","RLX_Description",
                "RLX_ClassificationUniclassEF_Number","RLX_ClassificationUniclassPr_Description",
                "RLX_ClassificationUniclassPr_Number","RLX_ClassificationUniclassSs_Description",
                "RLX_ClassificationUniclassSs_Number","RLX_CoordinatesX","RLX_CoordinatesY",
                "RLX_CoordinatesZ","RLX_Facility","RLX_GridReferenceSystem","RLX_Location","RLX_MaintenanceCost",
                "RLX_Space","RLX_Specification","RLX_System","RLX_Type","RLX_UniqueIdentifier","RLX_Zone",
                "DS_AssetID",
                "DS_AssetType"};

            //if (doc.GetElement(destinationPipesRef).Category.BuiltInCategory == BuiltInCategory.OST_PipeFitting)
            //{
            //    paramsToSet.Remove("RLX_Title");
            //}

                using (Transaction t = new Transaction(doc, "Copy pipe parameters"))
                {
                    t.Start();

                foreach (ElementId eid in visibleElements)
                {
                    FamilyInstance familyInstance = doc.GetElement(eid) as FamilyInstance;
                    ICollection<ElementId> subElements = familyInstance.GetSubComponentIds();

                    if (subElements != null)
                    {
                        try
                        {
                            foreach (ElementId subElement in subElements)
                            {
                                Element subelement = doc.GetElement(subElement);
                                foreach (string paramName in paramsToSet)
                                {
                                    //					TaskDialog.Show("R", paramName);
                                    string p = familyInstance.LookupParameter(paramName).AsValueString();
                                    subelement.LookupParameter(paramName).Set(p);
                                }

                                var sourceMaterial = familyInstance.LookupParameter("RLX_MainMaterial").AsElementId();
                                subelement.LookupParameter("RLX_MainMaterial").Set(sourceMaterial);
                            }

                        }
                        catch
                        {
                        }
                    }

                }

                
                    t.Commit();
                }//close transaction


            


            return Result.Succeeded;
        }
    }
}
