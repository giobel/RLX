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

            Reference familyRef = uidoc.Selection.PickObject(ObjectType.Element, "Pick family");

            Options opt = new Options();

            opt.DetailLevel = ViewDetailLevel.Fine;

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
                                    directShape.SetShape(solids );
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

                    List<GeometryObject> cabinet = ge.Where(x =>
                    x.Id == 12983 ||
                    x.Id == 12964 ||
                    x.Id == 12952 ||
                    x.Id == 12807 ||
                    x.Id == 12791 ||
                    x.Id == 12782 ||
                    x.Id == 12772 ||
                    x.Id == 12762 ||
                    x.Id == 12752 ||
                    x.Id == 12683 
                                                                 ).ToList();

                    DirectShape directShapeCabinet = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_Furniture));
                    directShapeCabinet.SetShape(cabinet);

                    List<Tuple<string, string>> data = new List<Tuple<string, string>>();
                    data.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Description", "Fire equipment cabinets"));
                    data.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Number", "Pr_80_77_28_28"));

                    foreach (var info in data)
                    {
                        Parameter p = directShapeCabinet.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }

                    List<Tuple<string, string>> valvesData = new List<Tuple<string, string>>();
                    data.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Description", "Deluge valves"));
                    data.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Number", "Pr_65_54_30_22"));


                    List<GeometryObject> valve1 = ge.Where(x =>

                    x.Id == 8606 ||
x.Id == 8600 ||
x.Id == 8592 ||
x.Id == 8582 ||
x.Id == 8536 ||
x.Id == 8490 ||
x.Id == 8483  ).ToList();

                    DirectShape valveShape1 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeAccessory));
                    valveShape1.SetShape(valve1);

                    foreach (var info in valvesData)
                    {
                        Parameter p = valveShape1.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }

                    List<GeometryObject> valve2 = ge.Where(x =>
                    x.Id == 10314 ||
x.Id == 10307 ||
x.Id == 10275 ||
x.Id == 10226 ||
x.Id == 10199 ||
x.Id == 10171 ||
x.Id == 10154 ||
x.Id == 10126 ||
x.Id == 10113 ||
x.Id == 10099 ||
x.Id == 10085 ||
x.Id == 10071 ||
x.Id == 10057 ||
x.Id == 10043 ||
x.Id == 10029 ||
x.Id == 10015 ||
x.Id == 10007).ToList();

                    DirectShape valveShape2 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeAccessory));
                    valveShape2.SetShape(valve2);

                    foreach (var info in valvesData)
                    {
                        Parameter p = valveShape2.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }

                    List<GeometryObject> valve3 = ge.Where(x =>
                    x.Id == 12238 ||
x.Id == 12231 ||
x.Id == 12199 ||
x.Id == 12150 ||
x.Id == 12123 ||
x.Id == 12095 ||
x.Id == 12078 ||
x.Id == 12050 ||
x.Id == 12037 ||
x.Id == 12023 ||
x.Id == 12009 ||
x.Id == 11995 ||
x.Id == 11981 ||
x.Id == 11967 ||
x.Id == 11953 ||
x.Id == 11939 ||
x.Id == 11931).ToList();

                    DirectShape valveShape3 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeAccessory));
                    valveShape3.SetShape(valve3);

                    foreach (var info in valvesData)
                    {
                        Parameter p = valveShape3.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }

                    List<GeometryObject> delugeValve1 = ge.Where(x =>
                    x.Id == 12656 ||
x.Id == 12619 ||
x.Id == 12610 ||
x.Id == 12555 ||
x.Id == 12527 ||
x.Id == 12491 ||
x.Id == 12453 ||
x.Id == 12445 ||
x.Id == 12432 ||
x.Id == 12424 ||
x.Id == 12411 ||
x.Id == 12403 ||
x.Id == 12390 ||
x.Id == 12382 ||
x.Id == 12369 ||
x.Id == 12361 ||
x.Id == 12348 ||
x.Id == 12340 ||
x.Id == 12327 ||
x.Id == 12319 ||
x.Id == 12306 ||
x.Id == 12298 ||
x.Id == 12266 ||
x.Id == 11884 ||
x.Id == 11875 ||
x.Id == 11868 ||
x.Id == 11816 ||
x.Id == 11810 ||
x.Id == 11773 ||
x.Id == 11764 ||
x.Id == 11756 ||
x.Id == 11691 ||
x.Id == 11680 ||
x.Id == 11638 ||
x.Id == 11545 ||
x.Id == 11538 ||
x.Id == 11470 ||
x.Id == 11463 ||
x.Id == 11434 ||
x.Id == 11418 ||
x.Id == 11410 ||
x.Id == 11326 ||
x.Id == 11320 ||
x.Id == 11312 ||
x.Id == 11304 ||
x.Id == 11294 ||
x.Id == 11279 ||
x.Id == 11271 ||
x.Id == 11175 ||
x.Id == 11168 ||
x.Id == 11150 ||
x.Id == 11143 ||
x.Id == 11099 ||
x.Id == 11041 ||
x.Id == 11026 ||
x.Id == 11010 ||
x.Id == 10974 ||
x.Id == 10961 ||
x.Id == 10932 ||
x.Id == 10924 ||
x.Id == 10908 ||
x.Id == 10892 ||
x.Id == 10876 ||
x.Id == 10860 ||
x.Id == 10844 ||
x.Id == 10828 ||
x.Id == 10812 ||
x.Id == 10796 ||
x.Id == 10781 ||
x.Id == 10773 ||
x.Id == 10765 ||
x.Id == 10757 ||
x.Id == 10721 ||
x.Id == 8789).ToList();

                    DirectShape delugeValveShape1 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeAccessory));
                    delugeValveShape1.SetShape(delugeValve1);

                    foreach (var info in valvesData)
                    {
                        Parameter p = delugeValveShape1.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }

                    List<GeometryObject> delugeValve2 = ge.Where(x =>
                    x.Id == 10694 ||
x.Id == 10657 ||
x.Id == 10648 ||
x.Id == 10631 ||
x.Id == 10603 ||
x.Id == 10567 ||
x.Id == 10529 ||
x.Id == 10521 ||
x.Id == 10508 ||
x.Id == 10500 ||
x.Id == 10487 ||
x.Id == 10479 ||
x.Id == 10466 ||
x.Id == 10458 ||
x.Id == 10445 ||
x.Id == 10437 ||
x.Id == 10424 ||
x.Id == 10416 ||
x.Id == 10403 ||
x.Id == 10395 ||
x.Id == 10382 ||
x.Id == 10374 ||
x.Id == 10342 ||
x.Id == 9999 ||
x.Id == 9991 ||
x.Id == 9984 ||
x.Id == 9891 ||
x.Id == 9884 ||
x.Id == 9816 ||
x.Id == 9809 ||
x.Id == 9780 ||
x.Id == 9764 ||
x.Id == 9756 ||
x.Id == 9691 ||
x.Id == 9680 ||
x.Id == 9672 ||
x.Id == 9588 ||
x.Id == 9582 ||
x.Id == 9574 ||
x.Id == 9566 ||
x.Id == 9559 ||
x.Id == 9552 ||
x.Id == 9537 ||
x.Id == 9529 ||
x.Id == 9513 ||
x.Id == 9477 ||
x.Id == 9464 ||
x.Id == 9435 ||
x.Id == 9427 ||
x.Id == 9380 ||
x.Id == 9326 ||
x.Id == 9320 ||
x.Id == 9283 ||
x.Id == 9274 ||
x.Id == 9265 ||
x.Id == 9250 ||
x.Id == 9242 ||
x.Id == 9146 ||
x.Id == 9139 ||
x.Id == 9121 ||
x.Id == 9114 ||
x.Id == 9070 ||
x.Id == 9012 ||
x.Id == 8997 ||
x.Id == 8981 ||
x.Id == 8965 ||
x.Id == 8949 ||
x.Id == 8933 ||
x.Id == 8917 ||
x.Id == 8901 ||
x.Id == 8885 ||
x.Id == 8869 ||
x.Id == 8863 ||
x.Id == 8855 ||
x.Id == 8847 ||
x.Id == 8839 ||
x.Id == 8803).ToList();

                    DirectShape delugeValveShape2 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeAccessory));
                    delugeValveShape2.SetShape(delugeValve2);

                    foreach (var info in valvesData)
                    {
                        Parameter p = delugeValveShape2.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }


                    List<GeometryObject> pipes1 = ge.Where(x =>
                    x.Id == 12942 ||
x.Id == 12933 ||
x.Id == 12925 ||
x.Id == 12917 ||
x.Id == 12909 ||
x.Id == 12901 ||
x.Id == 12845 ||
x.Id == 12828 ||
x.Id == 8772 ||
x.Id == 8758 ||
x.Id == 8743 ||
x.Id == 8663 ||
x.Id == 8632 ||
x.Id == 8408 ||
x.Id == 8395 ||
x.Id == 12669 ||
x.Id == 12569
).ToList();

                    DirectShape pipesShape1 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                    pipesShape1.SetShape(pipes1);

                    List<Tuple<string, string>> pipesData = new List<Tuple<string, string>>();
                    data.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Description", "Pipes and fittings"));
                    data.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Number", "Pr_65_52_63"));

                    List<GeometryObject> pipes2 = ge.Where(x =>
x.Id == 12893 ||
x.Id == 12885 ||
x.Id == 12877 ||
x.Id == 12867 ||
x.Id == 12856 ||
x.Id == 12823 ||
x.Id == 13010 ||
x.Id == 13007 ||
x.Id == 8728 ||
x.Id == 8710 ||
x.Id == 8695 ||
x.Id == 8679 ||
x.Id == 8647 ||
x.Id == 8383 ||
x.Id == 8370 ||
x.Id == 10707 
).ToList();

                    DirectShape pipesShape2 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                    pipesShape2.SetShape(pipes2);

                    foreach (var info in pipesData)
                    {
                        Parameter p = pipesShape2.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }

                    List<GeometryObject> pipes3 = ge.Where(x =>
                    x.Id == 12976 ||
x.Id == 12836 ||
x.Id == 8790 ||
x.Id == 8617
).ToList();

                    DirectShape pipesShape3 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                    pipesShape3.SetShape(pipes3);

                    foreach (var info in pipesData)
                    {
                        Parameter p = pipesShape3.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }



                    List<GeometryObject> pipes4 = ge.Where(x =>
x.Id == 8417
).ToList();

                    DirectShape pipesShape4 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                    pipesShape4.SetShape(pipes4);

                    foreach (var info in pipesData)
                    {
                        Parameter p = pipesShape4.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }



                    foreach (var item in ge)
                    {
                        //if (counter < 2000)
                        //{
                            try
                            {
                            
                            //DirectShape directShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
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
