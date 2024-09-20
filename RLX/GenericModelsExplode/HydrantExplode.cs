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

            List<Tuple<string, string>> cabinetData = new List<Tuple<string, string>>();
            cabinetData.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Description", "Fire equipment cabinets"));
            cabinetData.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Number", "Pr_80_77_28_28"));
            cabinetData.Add(new Tuple<string, string>("DS_AssetType", "CAB"));


            List<Tuple<string, string>> waterHoseData = new List<Tuple<string, string>>();
            waterHoseData.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Description", "Water hoses"));
            waterHoseData.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Number", "Pr_65_52_38_96"));
            waterHoseData.Add(new Tuple<string, string>("DS_AssetType", "FHR"));


            List<Tuple<string, string>> pipesData = new List<Tuple<string, string>>();
            pipesData.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Description", "Pipes and fittings"));
            pipesData.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Number", "Pr_65_52_63"));
            pipesData.Add(new Tuple<string, string>("DS_AssetType", "PPW"));

            List<Tuple<string, string>> valvesData = new List<Tuple<string, string>>();
            valvesData.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Description", "Deluge valves"));
            valvesData.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Number", "Pr_65_54_30_22"));

            List<Tuple<string, string>> locations = new List<Tuple<string, string>>();
            locations.Add(new Tuple<string, string>("RLX_Location", ""));
            locations.Add(new Tuple<string, string>("RLX_Space", ""));
            locations.Add(new Tuple<string, string>("RLX_Zone", ""));



            foreach (Reference familyRef in references)
            {

                Element familyInstance = doc.GetElement(familyRef) as Element;

                // Create a new tuple with the updated value
                Tuple<string, string> modifiedTuple = new Tuple<string, string>(locations[0].Item1, familyInstance.LookupParameter("RLX_Location").AsValueString());
                                // Update the list with the new tuple
                locations[0] = modifiedTuple;

                // Create a new tuple with the updated value
                Tuple<string, string> modifiedTuple1 = new Tuple<string, string>(locations[1].Item1, familyInstance.LookupParameter("RLX_Space").AsValueString());
                // Update the list with the new tuple
                locations[1] = modifiedTuple1;


                // Create a new tuple with the updated value
                Tuple<string, string> modifiedTuple2 = new Tuple<string, string>(locations[2].Item1, familyInstance.LookupParameter("RLX_Zone").AsValueString());
                locations[2] = modifiedTuple2;





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
                            dsCabinet.SetName("Cabinet");
                            dsCabinet.LookupParameter("Comments").Set("Cabinet");

                            foreach (var info in cabinetData)
                            {
                                Parameter p = dsCabinet.LookupParameter(info.Item1);
                                p.Set(info.Item2);
                            }

                            foreach (var info in locations)
                            {
                                Parameter p = dsCabinet.LookupParameter(info.Item1);
                                p.Set(info.Item2);
                            }

                            DirectShape dsPipeHose1 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeAccessory));
                            dsPipeHose1.SetShape(pipeHose1);
                            dsPipeHose1.SetName("Pipe Hose 1");
                            dsPipeHose1.LookupParameter("Comments").Set("Pipe Hose 1");

                            foreach (var info in waterHoseData)
                            {
                                Parameter p = dsPipeHose1.LookupParameter(info.Item1);
                                p.Set(info.Item2);
                            }

                            foreach (var info in locations)
                            {
                                Parameter p = dsPipeHose1.LookupParameter(info.Item1);
                                p.Set(info.Item2);
                            }

                            DirectShape dsPipeHose2 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeAccessory));
                            dsPipeHose2.SetShape(pipeHose2);
                            dsPipeHose2.SetName("Pipe Hose 2");
                            dsPipeHose2.LookupParameter("Comments").Set("Pipe Hose 2");

                            foreach (var info in waterHoseData)
                            {
                                Parameter p = dsPipeHose2.LookupParameter(info.Item1);
                                p.Set(info.Item2);
                            }

                            foreach (var info in locations)
                            {
                                Parameter p = dsPipeHose2.LookupParameter(info.Item1);
                                p.Set(info.Item2);
                            }

                            DirectShape dsValve = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeAccessory));
                            dsValve.SetShape(valve);
                            dsValve.SetName("Valve");
                            dsValve.LookupParameter("Comments").Set("Valve");

                            foreach (var info in valvesData)
                            {
                                Parameter p = dsValve.LookupParameter(info.Item1);
                                p.Set(info.Item2);
                            }

                            foreach (var info in locations)
                            {
                                Parameter p = dsValve.LookupParameter(info.Item1);
                                p.Set(info.Item2);
                            }


                            DirectShape dsPipe = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                            dsPipe.SetShape(pipes);



                            foreach (var info in pipesData)
                            {
                                Parameter p = dsPipe.LookupParameter(info.Item1);
                                p.Set(info.Item2);
                            }


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
