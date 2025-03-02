#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
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
    public class Util_CheckLocation : IExternalCommand
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

            var grouped = visibleElements.GroupBy(x => x.LookupParameter("RLX_UniqueIdentifier").AsValueString());
            FamilySymbol positionFamily = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfType<FamilySymbol>().Where(x => x.FamilyName == "Position").First();


            ProjectLocation projectLocation = doc.ActiveProjectLocation;
            Transform transform = projectLocation.GetTotalTransform();


            string error = "";
            using (Transaction t = new Transaction(doc, "Check Location"))
            {
                t.Start();
                foreach (var group in grouped)
                {
                    foreach (Element item in group)
                    {
                        double x = double.Parse(item.LookupParameter("RLX_CoordinatesX").AsValueString());
                        double y = double.Parse(item.LookupParameter("RLX_CoordinatesY").AsValueString());
                        double z = double.Parse(item.LookupParameter("RLX_CoordinatesZ").AsValueString());

                        XYZ loc = new XYZ(UnitUtils.ConvertToInternalUnits(x,UnitTypeId.Meters),
                                            UnitUtils.ConvertToInternalUnits(y, UnitTypeId.Meters), 
                                            UnitUtils.ConvertToInternalUnits(z, UnitTypeId.Meters));

                        loc = transform.OfPoint(loc);


                        FamilyInstance instance = doc.Create.NewFamilyInstance(loc, positionFamily, StructuralType.NonStructural);
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
