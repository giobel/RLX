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
    public class MechEquipFindClosestPipe : IExternalCommand
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


                IList<Element> allPipes = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_PipeCurves).WhereElementIsNotElementType().ToElements();

                List<Curve> pipeCurves = new List<Curve>();

                foreach (Element pipe in allPipes)
                {

                    LocationCurve pipeCurve = pipe.Location as LocationCurve;
                    pipeCurves.Add(pipeCurve.Curve);
                }

                if (allPipes.Count() != pipeCurves.Count())
                {

                    TaskDialog.Show("Error", "Cannot find a curve for some pipes");
                    return Result.Failed;
                }

                IList<Element> mechEquipments = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_MechanicalEquipment).WhereElementIsNotElementType().ToElements();

                Element closestPipe = null;
                double distance = 100 / 304.8;


                List<string> paramsToSet = new List<string>(){"RLX_ActualCost","RLX_ClassificationUniclassEF_Description",
                "RLX_ClassificationUniclassEF_Number","RLX_ClassificationUniclassPr_Description",
                "RLX_ClassificationUniclassPr_Number","RLX_ClassificationUniclassSs_Description",
                "RLX_ClassificationUniclassSs_Number","RLX_Component","RLX_CoordinatesX","RLX_CoordinatesY",
                "RLX_CoordinatesZ","RLX_Facility","RLX_GridReferenceSystem","RLX_Location","RLX_MaintenanceCost",
                "RLX_Space","RLX_Specification","RLX_System","RLX_Type","RLX_UniqueIdentifier","RLX_Zone","DS_AssetID"};

                using (Transaction t = new Transaction(doc, "Pipe fittings copy pipe parameters"))
                {
                    t.Start();

                    foreach (Element mecEquip in mechEquipments)
                    {

                        LocationPoint point = mecEquip.Location as LocationPoint;

                        for (int i = 0; i < allPipes.Count(); i++)
                        {
                            double currentDistance = pipeCurves[i].Distance(point.Point);
                            if (currentDistance < distance)
                            {

                                distance = currentDistance;
                                closestPipe = allPipes[i];

                            }
                        }

                        if (closestPipe != null)
                        {
                            foreach (string paramName in paramsToSet)
                            {
                                //					TaskDialog.Show("R", paramName);
                                string p = closestPipe.LookupParameter(paramName).AsValueString();
                                mecEquip.LookupParameter(paramName).Set(p);
                            }
                        }

                        closestPipe = null;
                        distance = 100 / 304.8;

                }
                    t.Commit();
                }//close transaction


            


            return Result.Succeeded;
        }
    }
}
