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
using RG = Rhino.Geometry;
#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class CopyUniclassFromType : IExternalCommand
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
            builtInCats.Add(BuiltInCategory.OST_DuctCurves);
            builtInCats.Add(BuiltInCategory.OST_DuctFitting);
            builtInCats.Add(BuiltInCategory.OST_DuctAccessory);




            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


            IList<Element> visibleElements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();

            using (Transaction t = new Transaction(doc, "Copy old uniqueIDS"))
            {

                t.Start();

                foreach (Element element in visibleElements)
                {

                    Element et = doc.GetElement(element.GetTypeId());

                    Parameter uniEFtypePar = et.LookupParameter("Identity_Classification_Uniclass 2015_Ef_Code");
                    Parameter uniEFtypeDescr = et.LookupParameter("Identity_Classification_Uniclass 2015_Ef");

                    Parameter uniPrtypePar = et.LookupParameter("Identity_Classification_Uniclass 2015_Pr_Code");
                    Parameter uniPrTypeDescr = et.LookupParameter("Identity_Classification_Uniclass 2015_Pr");

                    if (uniEFtypePar != null && uniEFtypePar.AsValueString() != null &&
                        uniEFtypeDescr != null && uniEFtypeDescr.AsValueString() != null &&
                        uniPrtypePar != null && uniPrtypePar.AsValueString() != null &&
                        uniPrTypeDescr != null && uniPrTypeDescr.AsValueString() != null)
                    {

                        Parameter uniEF = element.LookupParameter("RLX_ClassificationUniclassEF_Number");
                        uniEF.Set(uniEFtypePar.AsValueString());

                        Parameter uniEFdescr = element.LookupParameter("RLX_ClassificationUniclassEF_Description");
                        uniEFdescr.Set(uniEFtypeDescr.AsValueString().Split(':')[1].Trim());

                        Parameter uniPr = element.LookupParameter("RLX_ClassificationUniclassPr_Number");
                        uniPr.Set(uniPrtypePar.AsValueString());

                        Parameter uniPrDescr = element.LookupParameter("RLX_ClassificationUniclassPr_Description");
                        uniPrDescr.Set(uniPrTypeDescr.AsValueString().Split(':')[1].Trim());

                    }

                }

                t.Commit();
            }
                
                return Result.Succeeded;
            
        }
    }
}
