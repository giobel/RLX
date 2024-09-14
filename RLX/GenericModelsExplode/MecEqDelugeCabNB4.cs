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
    public class MecEqDelugeCabNB4 : IExternalCommand
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

            IList<Reference> familyReferences = uidoc.Selection.PickObjects(ObjectType.Element, "Pick family");


            Options opt = new Options();

            opt.DetailLevel = ViewDetailLevel.Fine;

            using (Transaction t = new Transaction(doc, "Explode NB Cabinet family"))
            {


                t.Start();

                foreach (Reference familyRef in familyReferences)
            {

                FamilyInstance familyInstance = doc.GetElement(familyRef) as FamilyInstance;

                var subElements = familyInstance.GetSubComponentIds();




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
                                directShape.LookupParameter("Comments").Set("Deluge Cabinet NB 4");
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
                        x.Id == 661

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



                        List<GeometryObject> pipes1 = ge.Where(x =>
    x.Id == 968 ||
    x.Id == 946 ||
    x.Id == 1133 ||
    x.Id == 1122 ||
    x.Id == 1478 ||
    x.Id == 919 ||
    x.Id == 855||
    x.Id == 1001||
    x.Id == 1106|
    x.Id == 1090 
    ).ToList();

                        DirectShape pipesShape1 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                        pipesShape1.SetShape(pipes1);
                        pipesShape1.LookupParameter("Comments").Set("Deluge Cabinet NB 4");

                        List<Tuple<string, string>> pipesData = new List<Tuple<string, string>>();
                        pipesData.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Description", "Pipes and fittings"));
                        pipesData.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Number", "Pr_65_52_63"));


                        foreach (var info in pipesData)
                        {
                            Parameter p = pipesShape1.LookupParameter(info.Item1);
                            p.Set(info.Item2);
                        }


                        List<GeometryObject> pipes2 = ge.Where(x =>
    x.Id ==990  ||
    x.Id ==957  ||
    x.Id ==935  ||
    x.Id ==1079  ||
    x.Id ==1456  ||
    x.Id ==887  ||
    x.Id ==871 ||
    x.Id ==1024 ||
    x.Id ==1063 ||
    x.Id == 1522
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
    x.Id == 1182 ||
    x.Id == 1171 ||
    x.Id == 1160 ||
    x.Id == 1511||
    x.Id == 1500||
    x.Id == 1489||
    x.Id == 1467 ||
    x.Id == 1144 ||
    x.Id == 1209 ||
    x.Id == 1193||
    x.Id == 1440||
    x.Id == 1417||
    x.Id == 1401

    ).ToList();

                        DirectShape pipesShape3 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                        pipesShape3.SetShape(pipes3);
                        pipesShape3.LookupParameter("Comments").Set("Deluge Cabinet NB");

                        foreach (var info in pipesData)
                        {
                            Parameter p = pipesShape3.LookupParameter(info.Item1);
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
