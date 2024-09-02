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
    public class ObejctsToDS : IExternalCommand
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

            IList<Reference> familyRef = uidoc.Selection.PickObjects(ObjectType.Element, "Pick families");
            IList<ElementId> familiesIds = new List<ElementId>();

            Options opt = new Options();

            opt.DetailLevel = ViewDetailLevel.Fine;


            List<GeometryObject> geoObjects = new List<GeometryObject>();

            foreach (var item in familyRef)
            {
                Element fi = doc.GetElement(item);
                familiesIds.Add(fi.Id);
                GeometryElement fiGeometry = fi.get_Geometry(opt);

                foreach (GeometryObject geoObj in fiGeometry)
                {
                    geoObjects.Add(geoObj);
                }


            }

            


            using (Transaction t = new Transaction(doc, "AA"))
            {


                t.Start();

                DirectShape directShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
                directShape.SetShape(geoObjects);

                doc.Delete(familiesIds);
                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
