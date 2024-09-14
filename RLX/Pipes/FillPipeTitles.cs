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
    public class FillPipeTitles : IExternalCommand
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


            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
            builtInCats.Add(BuiltInCategory.OST_PipeCurves);

            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


            IList<Element> visibleElements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();


            using (Transaction t = new Transaction(doc, "Fill pipes title"))
            {

                t.Start();

                foreach (Element element in visibleElements)
                {

                    Parameter title = element.LookupParameter("RLX_Title");

                    if (title.AsValueString() == null || title.AsValueString().Length < 3)
                    {

                        Parameter typeNameParam = element.LookupParameter("Type");
                        if (typeNameParam != null && typeNameParam.AsValueString() != null)
                        {
                            title.Set(typeNameParam.AsValueString());
                        }
                        else
                        {
                            //title.Set("705 Flexible Coupling Painted");
                        }
                    }



                }


                t.Commit();
            }

            return Result.Succeeded;
            
        }
    }
}
