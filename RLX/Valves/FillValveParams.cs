using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLX.Valves
{
    internal class FillValveParams
    {
        public void FillValvesParams()
        {
            UIDocument uidoc = this.ActiveUIDocument;
            Document doc = uidoc.Document;


            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
            builtInCats.Add(BuiltInCategory.OST_PipeAccessory);

            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


            IList<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();


            using (Transaction t = new Transaction(doc, "Fill valves params"))
            {

                t.Start();

                foreach (Element element in elements)
                {

                    Parameter title = element.LookupParameter("RLX_Title");



                    if (title.AsValueString() == null || title.AsValueString().Length < 3)
                    {

                        //						TaskDialog.Show("T", title.AsValueString());

                        title.Set(element.LookupParameter("Family").AsValueString());

                    }



                }


                t.Commit();
            }

        }
    }
}
