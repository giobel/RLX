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
    public class CWDryRiserOutletExplode : IExternalCommand
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

            IList<Reference> familyRefs = uidoc.Selection.PickObjects(ObjectType.Element, "Pick family");

            Options opt = new Options();

            opt.DetailLevel = ViewDetailLevel.Fine;




            using (Transaction t = new Transaction(doc, "Explode family"))
            {


                t.Start();

                foreach (Reference familyRef in familyRefs)
                {

                    FamilyInstance familyInstance = doc.GetElement(familyRef) as FamilyInstance;

                    GeometryElement geometryElement = familyInstance.get_Geometry(opt);

                    // Prepare a list of geometry objects
                    List<GeometryObject> geoObjects = new List<GeometryObject>();

                    // Iterate over the geometry and add it to the list
                    foreach (GeometryObject geoObj in geometryElement)
                    {

                        GeometryInstance gi = geoObj as GeometryInstance;

                        GeometryElement ge = gi.GetInstanceGeometry();

                        List<GeometryObject> cabinet = ge.Where(x =>
                        x.Id == 2058 ||
                        x.Id == 3241 ||
                        x.Id == 3206 ||
                        x.Id == 1008 ||
                        x.Id == 2033 ||
                        x.Id == 3260 ||
                        x.Id == 3187).ToList();

                        DirectShape directShapeCabinet = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_Furniture));
                        directShapeCabinet.SetShape(cabinet);
                        directShapeCabinet.SetName("CAB");
                        directShapeCabinet.LookupParameter("Component").Set("CAB CP");

                        foreach (var info in cabinetData)
                        {
                            Parameter p = directShapeCabinet.LookupParameter(info.Item1);
                            p.Set(info.Item2);
                        }





                        List<GeometryObject> waterHose1 = ge.Where(x =>
                            x.Id == 3074 ||
                            x.Id == 2767 ||
                            x.Id == 2700 ||
                            x.Id == 2675 ||
                            x.Id == 3452 ||
                            x.Id == 3299 ||
                            x.Id == 2078).ToList();


                        DirectShape waterHose1Shape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeAccessory));
                        waterHose1Shape.SetShape(waterHose1);
                        waterHose1Shape.LookupParameter("Component").Set("FHR1 CP");

                        foreach (var info in waterHoseData)
                        {
                            Parameter p = waterHose1Shape.LookupParameter(info.Item1);
                            p.Set(info.Item2);
                        }

                        List<GeometryObject> waterHose2 = ge.Where(x =>
                           x.Id == 2592 ||
                           x.Id == 2513 ||
                           x.Id == 2318 ||
                           x.Id == 2251 ||
                           x.Id == 2226).ToList();


                        DirectShape waterHose2Shape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeAccessory));
                        waterHose2Shape.SetShape(waterHose2);
                        waterHose2Shape.LookupParameter("Component").Set("FHR2 CP");

                        foreach (var info in waterHoseData)
                        {
                            Parameter p = waterHose2Shape.LookupParameter(info.Item1);
                            p.Set(info.Item2);
                        }


                        List<GeometryObject> pipeTop = ge.Where(x => x.Id == 3157).ToList();

                        DirectShape pipeTopShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                        pipeTopShape.SetShape(pipeTop);
                        pipeTopShape.LookupParameter("Component").Set("PPW CP");

                        foreach (var info in pipesData)
                        {
                            Parameter p = pipeTopShape.LookupParameter(info.Item1);
                            p.Set(info.Item2);
                        }

                        List<GeometryObject> pipeBot = ge.Where(x => x.Id == 3063).ToList();

                        DirectShape pipeBotShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                        pipeBotShape.SetShape(pipeBot);
                        pipeBotShape.LookupParameter("Component").Set("PPW CP");

                        foreach (var info in pipesData)
                        {
                            Parameter p = pipeBotShape.LookupParameter(info.Item1);
                            p.Set(info.Item2);
                        }

                    }
                }

                t.Commit();

            }

            return Result.Succeeded;
        }
    }
}
