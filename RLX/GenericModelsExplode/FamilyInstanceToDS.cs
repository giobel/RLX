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
    public class FamilyInstanceToDS : IExternalCommand
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

            FamilyInstance familyInstance = doc.GetElement(familyRef) as FamilyInstance;

            var subElements = familyInstance.GetSubComponentIds();

            using (Transaction t = new Transaction(doc, "Explode family"))
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

                            foreach (var item in ge)
                            {
                                //if (counter < 2000)
                                //{
                                try
                                {
                                    DirectShape directShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeFitting));
                                    directShape.SetShape(new List<GeometryObject>() { item });
                                    counter++;
                                }
                                catch { }
                                //}
                            }
                        }
                    }


            }
            GeometryElement geometryElement = familyInstance.get_Geometry(opt);


                // Set a name for the DirectShape
                //directShape.Name = "DirectShape from Family";

                // Prepare a list of geometry objects
                List<GeometryObject> geoObjects = new List<GeometryObject>();

                // Iterate over the geometry and add it to the list
                foreach (GeometryObject geoObj in geometryElement)
                {

                    GeometryInstance gi = geoObj as GeometryInstance;

                    GeometryElement ge = gi.GetInstanceGeometry();

                    List<GeometryObject> cabinet = ge.Where(x => x.Id == 3638 || 
                                                                 x.Id == 3673 ||
                                                                 x.Id == 11288 ||
                                                                 x.Id == 11584 ||
                                                                 x.Id == 11678 ||
                                                                 x.Id == 11132 ||
                                                                x.Id == 11393 ||
                                                                x.Id == 11487
                                                                 ).ToList();

                    DirectShape directShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_Furniture));
                    directShape.SetShape(cabinet);

                    foreach (var item in ge)
                    {
                        //if (counter < 2000)
                        //{
                            try
                            {
                            //    DirectShape directShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                            //directShape.Name = item.Id.ToString();
                            //directShape.SetShape(new List<GeometryObject>() { item });
                            //directShape.LookupParameter("Comments").Set(item.Id.ToString());
                            //    counter++;
                            }
                            catch { }
                        //}
                    }

                    geoObjects.Add(geoObj);
                    // Set the geometry of the DirectShape
                }


                

                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
