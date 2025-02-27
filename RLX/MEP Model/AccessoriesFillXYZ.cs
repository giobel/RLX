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
    public class AccessoriesFillXYZ : IExternalCommand
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


            IList<Element> visibleElements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(Helpers.RLXcatFilterAccessories()).WhereElementIsNotElementType().ToElements();

            int countElements = visibleElements.Count;
            int counterModified = 0;


            XYZ pbp = BasePoint.GetProjectBasePoint(doc).Position;
            XYZ sp = BasePoint.GetSurveyPoint(doc).Position;

            // Calculate the offset from project base point to survey point
            XYZ offset = sp - pbp;


            Transform surveyTransf = doc.ActiveProjectLocation.GetTotalTransform();

            ProjectLocation pl = doc.ActiveProjectLocation;
            Transform ttr = pl.GetTotalTransform().Inverse;


            using (Transaction t = new Transaction(doc, "Fill XYZ"))
            {

                t.Start();

                foreach (var element in visibleElements)
                {


                    if (element.LookupParameter("RLX_UniqueIdentifier").AsValueString() != null)
                    {



                        XYZ centroid = Helpers.GetElementCentroid(element);
                        if (centroid != null)
                        {
                            centroid = ttr.OfPoint(centroid);

                        }
                        else
                        {
                            uidoc.Selection.SetElementIds(new List<ElementId> { element.Id });
                            TaskDialog.Show("R", "Failed");
                            return Result.Failed;
                        }

                        double metricX = UnitUtils.ConvertFromInternalUnits(centroid.X, UnitTypeId.Meters);
                        double metricY = UnitUtils.ConvertFromInternalUnits(centroid.Y, UnitTypeId.Meters);
                        double metricZ = UnitUtils.ConvertFromInternalUnits(centroid.Z, UnitTypeId.Meters);

                        //TaskDialog.Show("R", String.Format("{0} {1} {2}", metricX, metricY, metricZ));

                        Helpers.FillXYZParam(element, metricX, metricY, metricZ);

                        counterModified++;
                    }
                    else
                    {
                        TaskDialog.Show("R", "Unique Id is null. Ignore");
                    }
                }

                t.Commit();
            }
            TaskDialog.Show("R", $"{counterModified} modified of {countElements}");
            return Result.Succeeded;
        }
    }
}
