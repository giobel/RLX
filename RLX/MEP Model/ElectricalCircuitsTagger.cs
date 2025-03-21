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
    public class ElectricalCircuitsTagger : IExternalCommand
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


            IList<Element> allCircuits = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_ElectricalCircuit).WhereElementIsNotElementType().ToElements();

            int countElements = allCircuits.Count;
            int counterModified = 0;


            using (Transaction t = new Transaction(doc, "Fill Electrical Circuits"))
            {

                t.Start();

                foreach (var e in allCircuits)
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
                    Parameter spec = e.LookupParameter("RLX_Specification");



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
                    spec.Set("ST Portal Building: ST150030-ARU-MAC-17-Z13-REQ-CS-0001 & General: ST150030-ARU-MAC-ZZ-ZZ-REQ-CS-0001/2/3/4");
                    counterModified++;

                    }
                    
                

                t.Commit();
            }
            TaskDialog.Show("R", $"{counterModified} modified of {countElements}");
            return Result.Succeeded;
        }
    }
}
