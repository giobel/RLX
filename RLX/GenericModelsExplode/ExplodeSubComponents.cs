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
    public class ExplodeSubComponents : IExternalCommand
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

            // Reference familyRef = uidoc.Selection.PickObject(ObjectType.Element, "Pick family");

            IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element, "Pick family");

            Options opt = new Options();

            opt.DetailLevel = ViewDetailLevel.Fine;

            foreach (Reference familyRef in references)
            {


                FamilyInstance familyInstance = doc.GetElement(familyRef) as FamilyInstance;

                var subElements = familyInstance.GetSubComponentIds();

                using (Transaction t = new Transaction(doc, "Explode subcomponents"))
                {


                    t.Start();


                    if (subElements != null)
                    {

                        foreach (ElementId eid in subElements)
                        {
                            FamilyInstance fi = doc.GetElement(eid) as FamilyInstance;

                            GeometryElement fiGeometry = fi.get_Geometry(opt);

                            foreach (GeometryObject geoObj in fiGeometry)
                            {

                                GeometryInstance gi = geoObj as GeometryInstance;

                                GeometryElement ge = gi.GetInstanceGeometry();

                                int counter = 0;

                                List<GeometryObject> solids = new List<GeometryObject>();

                                foreach (var item in ge)
                                {
                                    solids.Add(item);
                                }

                                try
                                {
                                    DirectShape directShape = DirectShape.CreateElement(doc, fi.Category.Id);
                                    directShape.SetShape(solids);
                                    counter++;
                                }
                                catch { }
                                //}

                            }
                        }


                    }




                    t.Commit();
                }
            }
            return Result.Succeeded;
        }
    }
}
