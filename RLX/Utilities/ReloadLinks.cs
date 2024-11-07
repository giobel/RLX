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
    public class ReloadLinks : IExternalCommand
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


            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<Element> linkInstances = collector.OfClass(typeof(RevitLinkType)).ToElements();

            List<string> linkNames = new List<string>();
            string result = "";
            using (Transaction t = new Transaction(doc, "Updating Link from IFC"))
            {

                foreach (Element eli in linkInstances)
                {
                    RevitLinkType rlt = eli as RevitLinkType;
                    linkNames.Add(eli.Name);
                    result += eli.Name + " ";
                    ExternalFileReference efr = eli.GetExternalFileReference();

                    if (eli.Name.Contains(".ifc"))
                    {

                    string userVisiblePath = "C:\\Digital Sapiens\\Digital Sapiens\\Sergio Ros - 01 - Incoming\\02_Fabrication Design Model (For reference)\\Current\\" + eli.Name + ".RVT";
                    
                    ModelPath mpForReload = ModelPathUtils.ConvertUserVisiblePathToModelPath(userVisiblePath);
                    
                    t.Start();
                    
                    rlt.UpdateFromIFC(doc, userVisiblePath.Remove(userVisiblePath.Length - 4), userVisiblePath, false);
                   
                    t.Commit();

                    LinkLoadResult res = rlt.LoadFrom(mpForReload, new WorksetConfiguration());

                    result += res.LoadResult.ToString() + "\n";
                        //TaskDialog.Show("R", string.Format("Result = {0}", res.LoadResult));
                    }
                    else if (eli.Name.ToUpper().Contains(".RVT"))
                    {
                        string userVisiblePath = "C:\\Digital Sapiens\\Digital Sapiens\\Sergio Ros - 01 - Incoming\\02_Fabrication Design Model (For reference)\\Current\\" + eli.Name;
                        ModelPath mpForReload = ModelPathUtils.ConvertUserVisiblePathToModelPath(userVisiblePath);
                        LinkLoadResult res = rlt.LoadFrom(mpForReload, new WorksetConfiguration());

                        res = rlt.LoadFrom(mpForReload, new WorksetConfiguration());
                        result += res.LoadResult.ToString() + "\n";
                    }
                }
               
            }

            TaskDialog.Show("R", result);

            return Result.Succeeded;
            
        }
    }
}
