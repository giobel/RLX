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
    public class MecEqDelugeCabSB_V3 : IExternalCommand
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

                            List<GeometryObject> solids = new List<GeometryObject>();

                            foreach (var item in ge)
                            {
                                solids.Add(item);
                            }

                            DirectShape directShape = DirectShape.CreateElement(doc, fi.Category.Id);
                            directShape.SetShape(solids);
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
                    x.Id == 8090 ||
                    x.Id == 8060 ||
                    x.Id == 8041 ||
                    x.Id == 7918 ||
                    x.Id == 7883 ||
                    x.Id == 7864 ||
                    x.Id == 7845 ||
                    x.Id == 7826 ||
                    x.Id == 7807 ||
                    x.Id == 7676
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

                    x.Id == 418 ||
                    x.Id == 407 ||
                    x.Id == 396 ||
                    x.Id == 369 ||
                    x.Id == 253 ||
                    x.Id == 192 ||
                    x.Id == 181).ToList();

                    DirectShape valveShape1 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeAccessory));
                    valveShape1.SetShape(valve1);

                    foreach (var info in valvesData)
                    {
                        Parameter p = valveShape1.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }

                    List<GeometryObject> valve2 = ge.Where(x =>
                    x.Id == 6865 ||
                    x.Id == 6854 ||
                    x.Id == 6795 ||
                    x.Id == 6718 ||
                    x.Id == 6677 ||
                    x.Id == 6626 ||
                    x.Id == 6597 ||
                    x.Id == 6546 ||
                    x.Id == 6519 ||
                    x.Id == 6492 ||
                    x.Id == 6465 ||
                    x.Id == 6438 ||
                    x.Id == 6411 ||
                    x.Id == 6384 ||
                    x.Id == 6357 ||
                    x.Id == 6330 ||
                    x.Id == 6319).ToList();

                    DirectShape valveShape2 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeAccessory));
                    valveShape2.SetShape(valve2);

                    foreach (var info in valvesData)
                    {
                        Parameter p = valveShape2.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }

                    List<GeometryObject> delugeValve1 = ge.Where(x =>

                    x.Id == 7624 ||
                    x.Id == 7557 ||
                    x.Id == 7538 ||
                    x.Id == 7473 ||
                    x.Id == 7438 ||
                    x.Id == 7387 ||
                    x.Id == 7320 ||
                    x.Id == 7252 ||
                    x.Id == 7241 ||
                    x.Id == 7214 ||
                    x.Id == 7203 ||
                    x.Id == 7176 ||
                    x.Id == 7165 ||
                    x.Id == 7138 ||
                    x.Id == 7127 ||
                    x.Id == 7100 ||
                    x.Id == 7089 ||
                    x.Id == 7062 ||
                    x.Id == 7051 ||
                    x.Id == 7024 ||
                    x.Id == 7013 ||
                    x.Id == 6986 ||
                    x.Id == 6975 ||
                    x.Id == 6916 ||
                    x.Id == 6245 ||
                    x.Id == 6234 ||
                    x.Id == 6223 ||
                    x.Id == 6140 ||
                    x.Id == 6117 ||
                    x.Id == 6058 ||
                    x.Id == 6039 ||
                    x.Id == 6028 ||
                    x.Id == 5927 ||
                    x.Id == 5910 ||
                    x.Id == 5810 ||
                    x.Id == 5667 ||
                    x.Id == 5656 ||
                    x.Id == 5549 ||
                    x.Id == 5530 ||
                    x.Id == 5479 ||
                    x.Id == 5444 ||
                    x.Id == 5421 ||
                    x.Id == 5296 ||
                    x.Id == 5267 ||
                    x.Id == 5244 ||
                    x.Id == 5221 ||
                    x.Id == 5198 ||
                    x.Id == 5169 ||
                    x.Id == 5146 ||
                    x.Id == 4997 ||
                    x.Id == 4978 ||
                    x.Id == 4949 ||
                    x.Id == 4938 ||
                    x.Id == 4867 ||
                    x.Id == 4778 ||
                    x.Id == 4743 ||
                    x.Id == 4708 ||
                    x.Id == 4641 ||
                    x.Id == 4614 ||
                    x.Id == 4563 ||
                    x.Id == 4540 ||
                    x.Id == 4511 ||
                    x.Id == 4482 ||
                    x.Id == 4453 ||
                    x.Id == 4424 ||
                    x.Id == 4395 ||
                    x.Id == 4366 ||
                    x.Id == 4337 ||
                    x.Id == 4308 ||
                    x.Id == 4267 ||
                    x.Id == 4232 ||
                    x.Id == 4185 ||
                    x.Id == 4162 ||
                    x.Id == 4095 ||
                    x.Id == 540     ).ToList();

                    DirectShape delugeValveShape1 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeAccessory));
                    delugeValveShape1.SetShape(delugeValve1);

                    foreach (var info in valvesData)
                    {
                        Parameter p = delugeValveShape1.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }



                    List<GeometryObject> pipes1 = ge.Where(x =>
                    x.Id == 8030 ||
                    x.Id == 8008||
                    x.Id == 7953||
                    x.Id == 522||
                    x.Id == 501||
                    x.Id == 50||
                    x.Id == 34 ||
                    x.Id == 7651
                    ).ToList();

                    DirectShape pipesShape1 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                    pipesShape1.SetShape(pipes1);

                    List<Tuple<string, string>> pipesData = new List<Tuple<string, string>>();
                    data.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Description", "Pipes and fittings"));
                    data.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Number", "Pr_65_52_63"));

                    List<GeometryObject> pipes2 = ge.Where(x =>
x.Id == 12893 ||
x.Id == 12877 ||
x.Id == 12823 ||
x.Id == 13010 ||
x.Id == 13007 ||
x.Id == 10707 ||
x.Id == 8728 ||
x.Id == 8710 ||
x.Id == 8383 ||
x.Id == 8370 ||
x.Id == 8019 ||
x.Id == 7975
).ToList();

                    DirectShape pipesShape2 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                    pipesShape2.SetShape(pipes2);

                    foreach (var info in pipesData)
                    {
                        Parameter p = pipesShape2.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }

                    List<GeometryObject> pipes3 = ge.Where(x =>
                    x.Id == 7997 ||
                    x.Id == 7986 ||
                    x.Id == 485||
                    x.Id == 469
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


                    List<GeometryObject> pipes5 = ge.Where(x =>
x.Id == 8079 ||
x.Id == 7964 ||
x.Id == 541 ||
x.Id == 453
).ToList();

                    DirectShape pipesShape5 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                    pipesShape5.SetShape(pipes5);

                    foreach (var info in pipesData)
                    {
                        Parameter p = pipesShape5.LookupParameter(info.Item1);
                        p.Set(info.Item2);
                    }


                    List<GeometryObject> pipes6 = ge.Where(x =>
x.Id == 66
).ToList();

                    DirectShape pipesShape6 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                    pipesShape6.SetShape(pipes6);

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
