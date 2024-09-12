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

            //Reference familyRef = uidoc.Selection.PickObject(ObjectType.Element, "Pick family");

            IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element, "Select families");

            Options opt = new Options();

            opt.DetailLevel = ViewDetailLevel.Fine;




            foreach (Reference familyRef in references)
            {

                Element familyInstance = doc.GetElement(familyRef) as Element;

                using (Transaction t = new Transaction(doc, "Explode hydrant family"))
                {


                    t.Start();


                    GeometryElement geometryElement = familyInstance.get_Geometry(opt);


                    List<GeometryObject> geoObjects = new List<GeometryObject>();

                    // Iterate over the geometry and add it to the list
                    foreach (GeometryObject geoObj in geometryElement)
                    {
                        if (geoObj.GetType() == typeof(Solid))
                        {
                            TaskDialog.Show("R", "solid");
                        }

                        GeometryInstance gi = geoObj as GeometryInstance;

                        GeometryElement ge = gi.GetInstanceGeometry();


                        List<GeometryObject> cabinet = ge.Where(x =>    x.Id == 3638 ||
                                                                         x.Id == 3673 ||
                                                                        x.Id == 11288 ||
                                                                        x.Id == 11678 ||
                                                                        x.Id == 11584 ||
                                                                        x.Id == 11132 ||
                                                                        x.Id == 11487 ||
                                                                        x.Id == 11346).ToList();

                        List<GeometryObject> pipeHose1 = ge.Where(x => x.Id == 5461 ||
                                                                       x.Id == 5438 ||
                                                                       x.Id == 5421 ||
                                                                       x.Id == 5290 ||
                                                                       x.Id == 5255 ||
                                                                       x.Id == 5220 ||
                                                                       x.Id == 5025 ||
                                                                       x.Id == 4958
                                                                       ).ToList();

                        List<GeometryObject> pipeHose2 = ge.Where(x => x.Id == 4923 ||
                                                                      x.Id == 4900 ||
                                                                      x.Id == 4883 ||
                                                                      x.Id == 4752 ||
                                                                      x.Id == 4717 ||
                                                                      x.Id == 4682 ||
                                                                      x.Id == 4487 ||
                                                                      x.Id == 4420
                                                                      ).ToList();

                        List<GeometryObject> valve = ge.Where(x => x.Id == 4196 ||
                                                                  x.Id == 4129 ||
                                                                  x.Id == 3845 ||
                                                                  x.Id == 3778 ||
                                                                  x.Id == 3551 ||
                                                                  x.Id == 11723 ||
                                                                  x.Id == 4391 ||
                                                                  x.Id == 3288
                                                  ).ToList();

                        List<GeometryObject> pipes = ge.Where(x =>
                                                                  x.Id == 5496 ||
                                                                  x.Id == 11007
                                                  ).ToList();

                        try
                        {
                            DirectShape dsCabinet = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_Furniture));
                            dsCabinet.SetShape(cabinet);

                            DirectShape dsPipeHose1 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeAccessory));
                            dsPipeHose1.SetShape(pipeHose1);

                            DirectShape dsPipeHose2 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeAccessory));
                            dsPipeHose2.SetShape(pipeHose2);


                            DirectShape dsValve = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeAccessory));
                            dsValve.SetShape(valve);

                            DirectShape dsPipe = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                            dsPipe.SetShape(pipes);
                        }
                        catch { }


                    }




                    t.Commit();
                }


            }


            return Result.Succeeded;
        }
    }
}
