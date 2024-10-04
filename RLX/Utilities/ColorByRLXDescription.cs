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
    public class ColorByRLXDescription : IExternalCommand
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


            FilteredElementCollector elementsInView = new FilteredElementCollector(doc);
            FillPatternElement solidFillPattern = elementsInView.OfClass(typeof(FillPatternElement)).Cast<FillPatternElement>().First(a => a.GetFillPattern().IsSolidFill);


            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
            builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);
            builtInCats.Add(BuiltInCategory.OST_GenericModel);
            builtInCats.Add(BuiltInCategory.OST_PipeCurves);
            builtInCats.Add(BuiltInCategory.OST_PipeInsulations);
            builtInCats.Add(BuiltInCategory.OST_PipeFitting);
            builtInCats.Add(BuiltInCategory.OST_PipeAccessory);
            builtInCats.Add(BuiltInCategory.OST_Furniture);



            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


            IList<Element> visibleElements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();

            var grouped = visibleElements.GroupBy(x => x.LookupParameter("RLX_Description").AsValueString());

            Random pRand = new Random();
            var md5 = MD5.Create();
            OverrideGraphicSettings ogs = new OverrideGraphicSettings();
            ogs.SetSurfaceForegroundPatternId(solidFillPattern.Id);

            string error = "";
            using (Transaction t = new Transaction(doc, "Override Colors"))
            {
                t.Start();
                foreach (var element in grouped)
                {


                    var firstElement = element.First();
                    string colorName = firstElement.LookupParameter("RLX_Description").AsValueString();

                    if (colorName == null)
                    {
                        colorName = "null";
                    }
                    var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(colorName));

                    byte iR, iG, iB;
                    iR = Convert.ToByte(pRand.Next(0, 255));
                    iG = Convert.ToByte(pRand.Next(0, 255));
                    iB = Convert.ToByte(pRand.Next(0, 255));

                    Autodesk.Revit.DB.Color pcolor = new Autodesk.Revit.DB.Color(hash[0], hash[1], hash[2]);



                    ogs.SetSurfaceForegroundPatternColor(pcolor);

                    try
                    {
                        //foreach (FamilyInstance item in element)
                        foreach (var item in element)
                        {
                            doc.ActiveView.SetElementOverrides(item.Id, ogs);

                        }

                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                    }
                }

                t.Commit();
            }

            if (error != "")
            {
                TaskDialog.Show("Error", error);
            }


            return Result.Succeeded;
            
        }
    }
}