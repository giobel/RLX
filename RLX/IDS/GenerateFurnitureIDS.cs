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
    public class GenerateFurnitureIDS : IExternalCommand
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


            IList<Element> velements = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_Furniture).WhereElementIsNotElementType().ToElements();

            //			TaskDialog.Show("R", elements.Count().ToString());

            int counter = 3700;

            using (Transaction t = new Transaction(doc, "Generate Furnitures Ids"))
            {

                t.Start();

                foreach (Element element in velements)
                {

                    try
                    {



                        Parameter assetId = element.LookupParameter("DS_AssetID");

                        string assetIdvalue = assetId.AsValueString();

                        //					TaskDialog.Show("R", assetIdvalue.Length.ToString());

                        if (assetIdvalue == null || assetIdvalue.Length < 3)
                        {

                            assetId.Set(counter.ToString());

                            counter++;

                        }



                    }
                    catch (Exception ex)
                    {
                        //TaskDialog.Show("R", ex.Message);
                    }

                }


                t.Commit();
            }


            return Result.Succeeded;
            
        }
    }
}
