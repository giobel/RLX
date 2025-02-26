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
using System.Security.Cryptography;
using System.Text;
using RG = Rhino.Geometry;
#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class FillCommonParams : IExternalCommand
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


            IList<Element> velements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(Helpers.RLXcatFilter()).WhereElementIsNotElementType().ToElements();
           
            int countElements = velements.Count;
            int counterModified = 0;


            List<Tuple<string, string>> data = new List<Tuple<string, string>>();
            data.Add(new Tuple<string, string>("RLX_System", "TunnelServiceBuildings_MEP"));
            data.Add(new Tuple<string, string>("RLX_Facility", "Silvertown Tunnel"));
            data.Add(new Tuple<string, string>("RLX_Type", "Project Facilities"));
            data.Add(new Tuple<string, string>("RLX_Zone", "Compound North"));
            data.Add(new Tuple<string, string>("RLX_Location", "Silvertown"));
            data.Add(new Tuple<string, string>("RLX_Space", "n/a"));
            data.Add(new Tuple<string, string>("RLX_ActualCost", "n/a"));
            data.Add(new Tuple<string, string>("RLX_MaintenanceCost", "n/a"));


            //Silvertown Bld:



            using (Transaction t = new Transaction(doc, "Fill params"))
            {

                t.Start();

                foreach (Element element in velements)
                {


                    foreach (var info in data)
                    {

                        Parameter p = element.LookupParameter(info.Item1);

                        p.Set(info.Item2);
                    }

                    counterModified++;

                }


                t.Commit();
            }

            TaskDialog.Show("R", $"{counterModified} modified of {countElements}");

            return Result.Succeeded;
            
        }
    }
}
