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
    public class HydrantExplode : IExternalCommand
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

            Reference familyRef = uidoc.Selection.PickObject(ObjectType.Element, "Pick family");

            Options opt = new Options();

            opt.DetailLevel = ViewDetailLevel.Fine;


            Element familyInstance = doc.GetElement(familyRef) as Element;



            using (Transaction t = new Transaction(doc, "Explode hydrant family"))
            {


                t.Start();


                GeometryElement geometryElement = familyInstance.get_Geometry(opt);


                List<GeometryObject> geoObjects = new List<GeometryObject>();

                List<int> cabinetIds = new List<int>() {3638, 3673};

                List<GeometryObject> cabinetObjects = new List<GeometryObject>();

                // Iterate over the geometry and add it to the list
                foreach (GeometryObject geoObj in geometryElement)
                {
                    if (cabinetIds.Contains(geoObj.Id))
                    {
                        cabinetObjects.Add(geoObj);
                    }
                }

                try
                {
                    DirectShape directShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));

                    directShape.SetShape(cabinetObjects);

                }
                catch { }







                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
