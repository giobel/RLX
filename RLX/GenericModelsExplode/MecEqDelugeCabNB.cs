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
    public class MecEqDelugeCabNB : IExternalCommand
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

            using (Transaction t = new Transaction(doc, "Explode NB Cabinet family"))
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

                            DirectShape directShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeFitting));
                            directShape.SetShape(solids);
                            directShape.LookupParameter("Comments").Set("Deluge Cabinet NB");
                            counter++;

                            
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
                    x.Id == 8319 ||
                    x.Id == 8289 ||
                    x.Id == 8270 ||
                    x.Id == 8054 ||
                    x.Id == 8019 ||
                    x.Id == 8000 ||
                    x.Id == 7981 ||
                    x.Id == 7962 ||
                    x.Id == 7943 ||
                    x.Id == 7812
                                                                 ).ToList();

                    DirectShape directShapeCabinet = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_Furniture));
                    directShapeCabinet.SetShape(cabinet);
                    directShapeCabinet.LookupParameter("Comments").Set("Deluge Cabinet NB");

                    List<Tuple<string, string>> data = new List<Tuple<string, string>>();
                    data.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Description", "Fire equipment cabinets"));
                    data.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Number", "Pr_80_77_28_28"));

                    foreach (var info in data)
                    {
                        Parameter p = directShapeCabinet.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }

                    List<Tuple<string, string>> valvesData = new List<Tuple<string, string>>();
                    valvesData.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Description", "Deluge valves"));
                    valvesData.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Number", "Pr_65_54_30_22"));


                    List<GeometryObject> valve1 = ge.Where(x => x.Id == 8684 || x.Id == 8673 || x.Id == 8662 || x.Id == 8635 || x.Id == 8519 || x.Id == 8458 || x.Id == 8447).ToList();

                    DirectShape valveShape1 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeAccessory));
                    valveShape1.SetShape(valve1);
                    valveShape1.Name = "Valve 1";
                    valveShape1.LookupParameter("Comments").Set("Deluge Cabinet NB");

                    foreach (var info in valvesData)
                    {
                        Parameter p = valveShape1.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }

                    List<GeometryObject> valve2 = ge.Where(x => x.Id == 3485 || x.Id == 3474 || x.Id == 3415 || x.Id == 3338 || x.Id == 3297 || x.Id == 3246 || x.Id == 3217 || x.Id == 3166 || x.Id == 3139 || x.Id == 3112 || x.Id == 3085 || x.Id == 3058 || x.Id == 3031 || x.Id == 3004 || x.Id == 2977 || x.Id == 2950 || x.Id == 2939).ToList();

                    DirectShape valveShape2 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeAccessory));
                    valveShape2.SetShape(valve2);
                    valveShape2.Name = "Valve 2";
                    valveShape2.LookupParameter("Comments").Set("Deluge Cabinet NB");

                    foreach (var info in valvesData)
                    {
                        Parameter p = valveShape2.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }

                    List<GeometryObject> valve3 = ge.Where(x =>x.Id == 7001 || x.Id == 6990 || x.Id == 6931 || x.Id == 6854 || x.Id == 6813 || x.Id == 6762 || x.Id == 6733 || x.Id == 6682 || x.Id == 6655 || x.Id == 6628 || x.Id == 6601 || x.Id == 6574 || x.Id == 6547 || x.Id == 6520 || x.Id == 6493 || x.Id == 6466 || x.Id == 6455).ToList();

                    DirectShape valveShape3 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeAccessory));
                    valveShape3.SetShape(valve3);
                    valveShape3.Name = "Valve 3";
                    valveShape3.LookupParameter("Comments").Set("Deluge Cabinet NB");

                    foreach (var info in valvesData)
                    {
                        Parameter p = valveShape3.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }

                    List<GeometryObject> delugeValve1 = ge.Where(x => x.Id == 7760 || x.Id == 7693 || x.Id == 7674 || x.Id == 7609 || x.Id == 7574 || x.Id == 7523 || x.Id == 7456 || x.Id == 7388 || x.Id == 7377 || x.Id == 7350 || x.Id == 7339 || x.Id == 7312 || x.Id == 7301 || x.Id == 7274 || x.Id == 7263 || x.Id == 7236 || x.Id == 7225 || x.Id == 7198 || x.Id == 7187 || x.Id == 7160 || x.Id == 7149 || x.Id == 7122 || x.Id == 7111 || x.Id == 7052 || x.Id == 6381 || x.Id == 6370 || x.Id == 6359 || x.Id == 6276 || x.Id == 6253 || x.Id == 6194 || x.Id == 6175 || x.Id == 6164 || x.Id == 6063 || x.Id == 6046 || x.Id == 5946 || x.Id == 5803 || x.Id == 5792 || x.Id == 5685 || x.Id == 5666 || x.Id == 5615 || x.Id == 5580 || x.Id == 5557 || x.Id == 5432 || x.Id == 5403 || x.Id == 5380 || x.Id == 5357 || x.Id == 5334 || x.Id == 5305 || x.Id == 5282 || x.Id == 5133 || x.Id == 5114 || x.Id == 5085 || x.Id == 5074 || x.Id == 5003 || x.Id == 4914 || x.Id == 4879 || x.Id == 4844 || x.Id == 4777 || x.Id == 4750 || x.Id == 4699 || x.Id == 4676 || x.Id == 4647 || x.Id == 4618 || x.Id == 4589 || x.Id == 4560 || x.Id == 4531 || x.Id == 4502 || x.Id == 4473 || x.Id == 4444 || x.Id == 4403 || x.Id == 4368 || x.Id == 4321 || x.Id == 4298 || x.Id == 4231 || x.Id == 676).ToList();
                    DirectShape delugeValveShape1 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeAccessory));
                    delugeValveShape1.SetShape(delugeValve1);
                    delugeValveShape1.Name = "Deluge Valve 1";
                    delugeValveShape1.LookupParameter("Comments").Set("Deluge Cabinet NB");

                    foreach (var info in valvesData)
                    {
                        Parameter p = delugeValveShape1.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }

                    List<GeometryObject> delugeValve2 = ge.Where(x =>x.Id == 4179 || x.Id == 4112 || x.Id == 4093 || x.Id == 4058 || x.Id == 4007 || x.Id == 3940 || x.Id == 3872 || x.Id == 3861 || x.Id == 3834 || x.Id == 3823 || x.Id == 3796 || x.Id == 3785 || x.Id == 3758 || x.Id == 3747 || x.Id == 3720 || x.Id == 3709 || x.Id == 3682 || x.Id == 3671 || x.Id == 3644 || x.Id == 3633 || x.Id == 3606 || x.Id == 3595 || x.Id == 3536 || x.Id == 2916 || x.Id == 2893 || x.Id == 2882 || x.Id == 2739 || x.Id == 2728 || x.Id == 2621 || x.Id == 2602 || x.Id == 2551 || x.Id == 2516 || x.Id == 2493 || x.Id == 2392 || x.Id == 2375 || x.Id == 2352 || x.Id == 2227 || x.Id == 2198 || x.Id == 2175 || x.Id == 2152 || x.Id == 2141 || x.Id == 2130 || x.Id == 2095 || x.Id == 2072 || x.Id == 2037 || x.Id == 1970 || x.Id == 1943 || x.Id == 1892 || x.Id == 1869 || x.Id == 1795 || x.Id == 1712 || x.Id == 1689 || x.Id == 1630 || x.Id == 1611 || x.Id == 1588 || x.Id == 1559 || x.Id == 1536 || x.Id == 1387 || x.Id == 1368 || x.Id == 1339 || x.Id == 1328 || x.Id == 1257 || x.Id == 1168 || x.Id == 1127 || x.Id == 1098 || x.Id == 1069 || x.Id == 1040 || x.Id == 1011 || x.Id == 982 || x.Id == 953 || x.Id == 924 || x.Id == 895 || x.Id == 872 || x.Id == 837 || x.Id == 790 || x.Id == 767 || x.Id == 700).ToList();

                    DirectShape delugeValveShape2 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeAccessory));
                    delugeValveShape2.SetShape(delugeValve2);
                    delugeValveShape2.Name = "Deluge Valve 2";
                    delugeValveShape2.LookupParameter("Comments").Set("Deluge Cabinet NB");

                    foreach (var info in valvesData)
                    {
                        Parameter p = delugeValveShape2.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }


                    List<GeometryObject> pipes1 = ge.Where(x =>
x.Id == 8251 ||
x.Id == 8232 ||
x.Id == 8221 ||
x.Id == 8188 ||
x.Id == 8100 ||
x.Id == 7787 ||
x.Id == 658 ||
x.Id == 637 ||
x.Id == 85 ||
x.Id == 69 ||
x.Id == 6027 ||
x.Id == 5992 ||
x.Id == 5969
).ToList();

                    DirectShape pipesShape1 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                    pipesShape1.SetShape(pipes1);
                    pipesShape1.LookupParameter("Comments").Set("Deluge Cabinet NB");

                    List<Tuple<string, string>> pipesData = new List<Tuple<string, string>>();
                    pipesData.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Description", "Pipes and fittings"));
                    pipesData.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Number", "Pr_65_52_63"));


                    foreach (var info in pipesData)
                    {
                        Parameter p = pipesShape1.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }


                    List<GeometryObject> pipes2 = ge.Where(x =>
x.Id == 8177 ||
x.Id == 8155 ||
x.Id == 8089 ||
x.Id == 8797 ||
x.Id == 8786 ||
x.Id == 4206 ||
x.Id == 605 ||
x.Id == 584 ||
x.Id == 53 ||
x.Id == 37
).ToList();

                    DirectShape pipesShape2 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                    pipesShape2.SetShape(pipes2);
                    pipesShape2.LookupParameter("Comments").Set("Deluge Cabinet NB");

                    foreach (var info in pipesData)
                    {
                        Parameter p = pipesShape2.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }

                    List<GeometryObject> pipes3 = ge.Where(x =>
x.Id == 8166 ||
x.Id == 8144 ||
x.Id == 8133 ||
x.Id == 568 ||
x.Id == 552 ||
x.Id == 520
).ToList();

                    DirectShape pipesShape3 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                    pipesShape3.SetShape(pipes3);
                    pipesShape3.LookupParameter("Comments").Set("Deluge Cabinet NB");

                    foreach (var info in pipesData)
                    {
                        Parameter p = pipesShape3.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }



                    List<GeometryObject> pipes4 = ge.Where(x =>
x.Id == 8767 ||
x.Id == 8756 ||
x.Id == 677 ||
x.Id == 8719
).ToList();

                    DirectShape pipesShape4 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                    pipesShape4.SetShape(pipes4);
                    pipesShape4.LookupParameter("Comments").Set("Deluge Cabinet NB");

                    foreach (var info in pipesData)
                    {
                        Parameter p = pipesShape4.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }

                List<GeometryObject> pipes5 = ge.Where(x =>
                x.Id == 8210 ||
                x.Id == 8199 ||
                x.Id == 8122 ||
                x.Id == 621 ||
                x.Id == 536 ||
                x.Id == 504).ToList();

                    DirectShape pipesShape5 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                    pipesShape5.SetShape(pipes5);
                    pipesShape5.LookupParameter("Comments").Set("Deluge Cabinet NB");

                    foreach (var info in pipesData)
                    {
                        Parameter p = pipesShape5.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }

                    List<GeometryObject> pipes6 = ge.Where(x =>
    x.Id == 8735).ToList();

                    DirectShape pipesShape6 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                    pipesShape6.SetShape(pipes6);
                    pipesShape6.LookupParameter("Comments").Set("Deluge Cabinet NB");

                    foreach (var info in pipesData)
                    {
                        Parameter p = pipesShape6.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }


                }
                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
