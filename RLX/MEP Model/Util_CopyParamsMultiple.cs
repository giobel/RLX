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
    public class Util_CopyParamsMultiple : IExternalCommand
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

            Reference sourcePiperef = uidoc.Selection.PickObject(ObjectType.Element, "Select source");

            Element sourcePipe = doc.GetElement(sourcePiperef);

            IList<Reference> destinationPipesRef = uidoc.Selection.PickObjects(ObjectType.Element, "Select destination pipes");

                List<string> paramsToSet = new List<string>(){"RLX_ActualCost",
                    "RLX_ClassificationUniclassEF_Description","RLX_Title","RLX_Description",
                "RLX_ClassificationUniclassEF_Number","RLX_ClassificationUniclassPr_Description",
                "RLX_ClassificationUniclassPr_Number","RLX_ClassificationUniclassSs_Description",
                "RLX_ClassificationUniclassSs_Number","RLX_CoordinatesX","RLX_CoordinatesY",
                "RLX_CoordinatesZ","RLX_Facility","RLX_GridReferenceSystem","RLX_Location","RLX_MaintenanceCost",
                "RLX_Space","RLX_Specification","RLX_System","RLX_Type","RLX_UniqueIdentifier","RLX_Zone",
                "DS_AssetID",
                "DS_AssetType","RLX_MainMaterial"};

            //if (doc.GetElement(destinationPipesRef).Category.BuiltInCategory == BuiltInCategory.OST_PipeFitting)
            //{
            //    paramsToSet.Remove("RLX_Title");
            //}

                using (Transaction t = new Transaction(doc, "Copy parameters"))
                {
                    t.Start();

                foreach (var item in destinationPipesRef)
                {




                           Element pipe = doc.GetElement(item);
                            foreach (string paramName in paramsToSet)
                            {
                        Parameter p = sourcePipe.LookupParameter(paramName);

                        if (p.StorageType == StorageType.String)
                            pipe.LookupParameter(paramName).Set(p.AsValueString());

                        if (p.StorageType == StorageType.ElementId)
                            pipe.LookupParameter(paramName).Set(p.AsElementId());
                    }

                }
                
                    t.Commit();
                }//close transaction


            


            return Result.Succeeded;
        }
    }
}
