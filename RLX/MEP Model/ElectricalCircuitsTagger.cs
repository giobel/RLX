#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Rhino.Geometry;
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


            //IList<Element> allCircuits = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_ElectricalCircuit).WhereElementIsNotElementType().ToElements();
            //IList<Element> allCircuits = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipingSystem).WhereElementIsNotElementType().ToElements();
            IList<Element> allCircuits = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctSystem).WhereElementIsNotElementType().ToElements();




            UIDocument uIDocument = uiapp.ActiveUIDocument;
            string activeDocTitle = uIDocument.Document.Title;


            int elecModified = 0;
            int elecCounter = 2001;


            using (Transaction t = new Transaction(doc, "Fill Electrical Circuits"))
            {

               

                t.Start();

                foreach (var e in allCircuits)
                    {

                    Parameter acost = e.LookupParameter("RLX_ActualCost");

                    Parameter typeSS = e.LookupParameter("RLX_ClassificationUniclassSs_Description");
                    Parameter typeSScode = e.LookupParameter("RLX_ClassificationUniclassSs_Number");

                    Parameter eleEf = e.LookupParameter("RLX_ClassificationUniclassEF_Description");
                    Parameter eleEfcode = e.LookupParameter("RLX_ClassificationUniclassEF_Number");

                    Parameter elePr = e.LookupParameter("RLX_ClassificationUniclassPr_Description");
                    Parameter elePrcode = e.LookupParameter("RLX_ClassificationUniclassPr_Number");

                    Parameter xcoord = e.LookupParameter("RLX_CoordinatesX");
                    Parameter ycoord = e.LookupParameter("RLX_CoordinatesY");
                    Parameter zcoord = e.LookupParameter("RLX_CoordinatesZ");

                    Parameter description = e.LookupParameter("RLX_Description");

                    Parameter facility = e.LookupParameter("RLX_Facility");
                    Parameter grid = e.LookupParameter("RLX_GridReferenceSystem");
                    Parameter location = e.LookupParameter("RLX_Location");
                    Parameter maincost = e.LookupParameter("RLX_MaintenanceCost");
                    Parameter space = e.LookupParameter("RLX_Space");
                    Parameter spec = e.LookupParameter("RLX_Specification");
                    Parameter system = e.LookupParameter("RLX_System");

                    Parameter title = e.LookupParameter("RLX_Title");

                    Parameter type = e.LookupParameter("RLX_Type");

                    Parameter uniqueId = e.LookupParameter("RLX_UniqueIdentifier");

                    Parameter zone = e.LookupParameter("RLX_Zone");

                    //VALUES


                    //electrical circuits

                    //string lc = e.LookupParameter("Load Classification").AsValueString();
                    //title.Set($"Electrical Circuit {lc}");

                    //piping systems
                    //string lc = e.LookupParameter("Type").AsValueString();
                    //title.Set($"Piping System {lc}");


                    //piping systems
                    string lc = e.LookupParameter("Type").AsValueString();
                    title.Set($"Duct System {lc}");


                    //electrical circuits
                    //string loadName = e.LookupParameter("Load Name").AsValueString();
                    //string panel = e.LookupParameter("Panel").AsValueString();

                    //piping systems
                    //PipingSystem pipingSystem = e as PipingSystem;

                    //string loadName = pipingSystem.SystemType.ToString();


                    MechanicalSystem mechanicalSystem = e as MechanicalSystem;

                    string loadName = mechanicalSystem.SystemType.ToString().Replace("air"," air");

                    //COMMON PARAMS
                    system.Set("TunnelServiceBuildings_MEP");
                    facility.Set("Silvertown Tunnel");
                    type.Set("Project Facilities");
                    space.Set("n/a");
                    grid.Set("BNG");
                    acost.Set("n/a");
                    maincost.Set("n/a");

                    xcoord.Set("n/a");
                    ycoord.Set("n/a");
                    zcoord.Set("n/a");

                    //ELECTRICAL CIRCUITS

                    //typeSS.Set("Electricity distribution systems");
                    //typeSScode.Set("Ss_70_30");

                    //eleEf.Set("Electricity distribution");
                    //eleEfcode.Set("EF_70_30");

                    //elePr.Set("Electrical power products and wiring accessories");
                    //elePrcode.Set("Pr_65_72");


                    //PIPING SYSTEM

                    //typeSS.Set("Piped supply systems");
                    //typeSScode.Set("Ss_55");

                    //eleEf.Set("Piped supply functions");
                    //eleEfcode.Set("EF_55");

                    //elePr.Set("Pipe accessories");
                    //elePrcode.Set("Pr_65_52_61");

                    //DUCT SYSTEM

                    typeSS.Set("Mechanical and whole building ventilation systems");
                    typeSScode.Set("Ss_65_40_33_52");

                    eleEf.Set("Ventilation and air conditioning functions");
                    eleEfcode.Set("EF_65");

                    elePr.Set("Ductwork and fittings");
                    elePrcode.Set("Pr_65_65_25");



                    //string elecCode = "ELC"; electrical systems

                    //string elecCode = "PPS"; //piping systems

                    string elecCode = "DCS"; //Ducts systems

                    if (activeDocTitle.Contains("Z13-M3S-CS-0001"))
                    {
                        //Silvertown Bld
                        //code = "L252013B0";
                        //code = "Newham Portal Building";
                        spec.Set("ST Portal Building: ST150030-ARU-MAC-17-Z13-REQ-CS-0001 & General: ST150030-ARU-MAC-ZZ-ZZ-REQ-CS-0001/2/3/4");
                        uniqueId.Set($"L252013B0{elecCode}{elecCounter}");

                        //description.Set($"{Helpers.ConvertToSentenceCase(loadName)} Newham Portal Building Panel: {panel}"); //electrical systems
                        description.Set($"{Helpers.ConvertToSentenceCase(loadName)} Newham Portal Building");

                        zone.Set("Compound North");
                        location.Set("Silvertown");
                    }
                
                    else if (activeDocTitle.Contains("Z13-M3S-CS-0002"))
                    {
                        //Silvertown Service:
                        spec.Set("ST Service Building: ST150030-ARU-MAC-17-Z13-REQ-CS-0002 & General: ST150030-ARU-MAC-ZZ-ZZ-REQ-CS-0001/2/3/4");
                        uniqueId.Set($"X013013S0{elecCode}{elecCounter}");
                        //description.Set($"{Helpers.ConvertToSentenceCase(loadName)} Newham Services Building Panel: {panel}"); //electrical systems
                        description.Set($"{Helpers.ConvertToSentenceCase(loadName)} Newham Services Building");

                        zone.Set("Compound North");
                        location.Set("Silvertown");
                    }
                    else if (activeDocTitle.Contains("Z14-M3S-CS-0001"))
                    {
                        //Greenwich Bld
                        spec.Set("GW Portal Building: ST150030-ARU-MAC-ZZ-ZZ-REQ-CS-0001 & General: ST150030-ARU-MAC-ZZ-ZZ-REQ-CS-0001/2/3/4");
                        uniqueId.Set($"L114014B0{elecCode}{elecCounter}");
                        //description.Set($"{Helpers.ConvertToSentenceCase(loadName)} Greenwich Portal Building Panel: {panel}"); //electrical systems
                        description.Set($"{Helpers.ConvertToSentenceCase(loadName)} Greenwich Portal Building");
                        zone.Set("Compound South");
                        location.Set("Greenwich");
                    }

                    elecCounter++;
                    elecModified++;


                    }
                    
                

                t.Commit();
            }
            TaskDialog.Show("R", $"{elecModified} modified of {elecCounter}");
            return Result.Succeeded;
        }
    }
}
