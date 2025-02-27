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
    public class Util_FillCommonParam : IExternalCommand
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


            IList<Element> visibleElements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(Helpers.RLXcatFilter()).WhereElementIsNotElementType().ToElements();

            int countElements = visibleElements.Count;
            int counterModified = 0;


            using (Transaction t = new Transaction(doc, "Fill XYZ"))
            {

                t.Start();

                foreach (var e in visibleElements)
                    {
                    Parameter system = e.LookupParameter("RLX_System");
                    Parameter facility = e.LookupParameter("RLX_Facility");
                    Parameter type = e.LookupParameter("RLX_Type");
                    Parameter zone = e.LookupParameter("RLX_Zone");
                    Parameter location = e.LookupParameter("RLX_Location");
                    Parameter space = e.LookupParameter("RLX_Space");
                    Parameter grid = e.LookupParameter("RLX_GridReferenceSystem");
                    Parameter acost = e.LookupParameter("RLX_ActualCost");
                    Parameter maincost = e.LookupParameter("RLX_MaintenanceCost");

                    //Silvertown Bld:
                    system.Set("TunnelServiceBuildings_MEP");
                    facility.Set("Silvertown Tunnel");
                    type.Set("Project Facilities");
                    zone.Set("Compound North");
                    location.Set("Silvertown");
                    space.Set("n/a");
                    grid.Set("BNG");
                    acost.Set("n/a");
                    maincost.Set("n/a");

                    counterModified++;

                    }
                    
                

                t.Commit();
            }
            TaskDialog.Show("R", $"{counterModified} modified of {countElements}");
            return Result.Succeeded;
        }
    }
}
