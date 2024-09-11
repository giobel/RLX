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
    public class PipeFittCopyClosestPipeParams : IExternalCommand
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

            List<Element> pipesToRemove = new List<Element>();

            foreach (Element pipe in allPipes)
            {

                LocationCurve pipeCurve = pipe.Location as LocationCurve;

                if (pipeCurve != null)
                {
                    pipeCurves.Add(pipeCurve.Curve);
                }
                else
                {
                    pipesToRemove.Add(pipe);
                }
            }

            foreach (var item in pipesToRemove)
            {
                allPipes.Remove(item);
            }

            //if (allPipes.Count() != pipeCurves.Count())
            //{

            //    TaskDialog.Show("Error", "Cannot find a curve for some pipes");
            //    return Result.Failed;
            //}

            IList<Element> allPipeFittings = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_PipeFitting).WhereElementIsNotElementType().ToElements();

            Element closestPipe = null;
            double distance = 1000;

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
                "DS_Location"

            };

            using (Transaction t = new Transaction(doc, "Pipe fittings copy pipe parameters"))
            {
                t.Start();

                foreach (Element fitting in allPipeFittings)
                {

                    LocationPoint point = fitting.Location as LocationPoint;

                    if (point != null)
                    {
                        for (int i = 0; i < allPipes.Count(); i++)
                        {
                            double currentDistance = pipeCurves[i].Distance(point.Point);
                            if (currentDistance < distance)
                            {

                                distance = currentDistance;
                                closestPipe = allPipes[i];

                            }
                        }

                    foreach (string paramName in paramsToSet)
                    {
                        //					TaskDialog.Show("R", paramName);
                        Parameter p = closestPipe.LookupParameter(paramName);

                        if (p.StorageType == StorageType.String)
                            fitting.LookupParameter(paramName).Set(p.AsValueString());

                        if (p.StorageType == StorageType.ElementId)
                            fitting.LookupParameter(paramName).Set(p.AsElementId());

                    }

                    }

                    closestPipe = null;
                    distance = 1000;

                }
                t.Commit();
            }//close transaction





            return Result.Succeeded;
        }
    }
}
