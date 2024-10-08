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
    public class HydrantBranchExplode : IExternalCommand
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

                using (Transaction t = new Transaction(doc, "Explode hydrant branch family"))
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
                                    directShape.Name = "Tunnel NB Hydrant branch";
                                    directShape.LookupParameter("Comments").Set("Tunnel NB Hydrant branch");
                                    counter++;
                                }
                                catch { }
                                //}

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





                        List<GeometryObject> pipes = ge.Where(x =>
// tunnel south bound family
//x.Id == 140 || x.Id == 118 || x.Id == 96 || x.Id == 85 || x.Id == 283 || x.Id == 272 || x.Id == 261 || x.Id == 250 || x.Id == 69 || x.Id == 53 || x.Id == 37 || x.Id == 21 || x.Id == 212 || x.Id == 196 || x.Id == 178 || x.Id == 162 ||

//tunnel north bound family
x.Id == 140 || x.Id == 118 || x.Id == 96 || x.Id == 85 || x.Id == 283 || x.Id == 272 || x.Id == 261 || x.Id == 250 || x.Id == 69 || x.Id == 53 || x.Id == 37 || x.Id == 21 || x.Id == 212 || x.Id == 196 || x.Id == 178 || x.Id == 162
).ToList();

                        DirectShape pipesShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                        pipesShape.SetShape(pipes);
                        pipesShape.SetName("Tunnel NB");
                        pipesShape.LookupParameter("Comments").Set("Tunnel NB Hydrant branch");

                    }




                    t.Commit();
                }
            }
            return Result.Succeeded;
        }
    }
}
