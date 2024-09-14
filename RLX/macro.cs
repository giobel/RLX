/*
 * Created by SharpDevelop.
 * User: GioJu
 * Date: 8/28/2024
 * Time: 10:12 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace DS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.DB.Macros.AddInId("144ABEBE-73A8-487A-A500-4EFCDE977A1F")]
    public partial class ThisApplication
    {
        private void Module_Startup(object sender, EventArgs e)
        {

        }

        private void Module_Shutdown(object sender, EventArgs e)
        {

        }

        #region Revit Macros generated code
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(Module_Startup);
            this.Shutdown += new System.EventHandler(Module_Shutdown);
        }
        #endregion


        public void FillLocations()
        {
            UIDocument uidoc = this.ActiveUIDocument;
            Document doc = uidoc.Document;


            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
            builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);
            builtInCats.Add(BuiltInCategory.OST_GenericModel);
            builtInCats.Add(BuiltInCategory.OST_PipeCurves);
            builtInCats.Add(BuiltInCategory.OST_PipeInsulations);
            builtInCats.Add(BuiltInCategory.OST_PipeFitting);
            builtInCats.Add(BuiltInCategory.OST_PipeAccessory);
            builtInCats.Add(BuiltInCategory.OST_Sprinklers);
            builtInCats.Add(BuiltInCategory.OST_Furniture);



            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


            IList<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();

            using (Transaction t = new Transaction(doc, "Fill params"))
            {

                t.Start();

                foreach (Element element in elements)
                {



                    string ds_axis = element.LookupParameter("DS_Axis").AsValueString();

                    if (ds_axis != null)
                    {


                    }
                    //					
                    //					Parameter fac = element.LookupParameter("RLX_Axis");
                    //					fac.Set("Silvertown Tunnel");
                    //					
                    //					Parameter fac = element.LookupParameter("RLX_Location");
                    //					fac.Set("Silvertown Tunnel");
                    //
                    //					Parameter fac = element.LookupParameter("RLX_Zone");
                    //					fac.Set("Silvertown Tunnel");

                    //					Parameter comp = element.LookupParameter("RLX_Component");
                    //					comp.Set("ST150030-COW-FRS-40-ZZ-REQ-FE-0006");


                    //					Parameter sys = element.LookupParameter("RLX_System");
                    //					sys.Set("Fire System_Fixed fire-fighting system");



                }


                t.Commit();
            }

        }

        public void PipeFittingFindConnectedPipes()
        {

            UIDocument uidoc = this.ActiveUIDocument;
            Document doc = uidoc.Document;

            Reference fittingRef = uidoc.Selection.PickObject(ObjectType.Element, "Select Pipe Fitting");

            Element e = doc.GetElement(fittingRef);

            var ele = e as FamilyInstance;
            var connectors = ele.MEPModel.ConnectorManager.Connectors.Cast<IConnector>();
            List<Element> ConnectedElements = new List<Element>();
            foreach (IConnector c in connectors)
            {
                var r = c as Connector;
                var allRef = r.AllRefs;
                foreach (Connector connector in allRef)
                {
                    TaskDialog.Show("R", connector.Owner.Category.Name);
                    ConnectedElements.Add(connector.Owner);
                }
            }

        }



        public void Add1000ToId()
        {
            UIDocument uidoc = this.ActiveUIDocument;
            Document doc = uidoc.Document;


            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
            builtInCats.Add(BuiltInCategory.OST_PipeCurves);

            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


            IList<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();


            using (Transaction t = new Transaction(doc, "Fill params"))
            {

                t.Start();

                foreach (Element element in elements)
                {

                    Parameter id = element.LookupParameter("DS_AssetID");

                    if (id != null && int.Parse(id.AsValueString()) < 999)
                    {

                        int newId = int.Parse(id.AsValueString()) + 1000;

                        id.Set(newId.ToString());

                    }



                }


                t.Commit();
            }

        }


        public void FamilyInstanceToDS()
        {

            UIDocument uidoc = this.ActiveUIDocument;
            Document doc = uidoc.Document;


            Reference familyRef = uidoc.Selection.PickObject(ObjectType.Element, "Pick family");

            Options opt = new Options();

            opt.DetailLevel = ViewDetailLevel.Fine;

            FamilyInstance familyInstance = doc.GetElement(familyRef) as FamilyInstance;

            var subElements = familyInstance.GetSubComponentIds();

            using (Transaction t = new Transaction(doc, "AA"))
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

                    int counter = 0;

                    foreach (var item in ge)
                    {
                        //if (counter < 2000)
                        //{
                        try
                        {
                            DirectShape directShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                            directShape.SetShape(new List<GeometryObject>() { item });
                            counter++;
                        }
                        catch { }
                        //}
                    }

                    geoObjects.Add(geoObj);
                    // Set the geometry of the DirectShape
                }




                t.Commit();
            }
        }


        public void FillValvesTitle()
        {
            UIDocument uidoc = this.ActiveUIDocument;
            Document doc = uidoc.Document;


            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
            builtInCats.Add(BuiltInCategory.OST_PipeAccessory);
            builtInCats.Add(BuiltInCategory.OST_GenericModel);


            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


            IList<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();


            using (Transaction t = new Transaction(doc, "Fill valves params"))
            {

                t.Start();

                foreach (Element element in elements)
                {

                    Parameter title = element.LookupParameter("RLX_Title");



                    if (title.AsValueString() == null || title.AsValueString().Length < 3)
                    {

                        //						TaskDialog.Show("T", title.AsValueString());
                        try
                        {
                            title.Set(element.LookupParameter("Family").AsValueString());
                        }
                        catch { }

                    }



                }


                t.Commit();
            }

        }






        public void UpdateOld20digitsIds()
        {
            UIDocument uidoc = this.ActiveUIDocument;
            Document doc = uidoc.Document;


            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
            builtInCats.Add(BuiltInCategory.OST_PipeInsulations);
            builtInCats.Add(BuiltInCategory.OST_PipeCurves);
            builtInCats.Add(BuiltInCategory.OST_PipeInsulations);
            builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);
            builtInCats.Add(BuiltInCategory.OST_PipeAccessory);
            builtInCats.Add(BuiltInCategory.OST_PipeFitting);
            builtInCats.Add(BuiltInCategory.OST_Sprinklers);
            builtInCats.Add(BuiltInCategory.OST_GenericModel);





            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


            IList<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();

            //			TaskDialog.Show("R", elements.Count.ToString());

            using (Transaction t = new Transaction(doc, "Update old uniqueIDS"))
            {

                t.Start();

                foreach (Element element in elements)
                {

                    Parameter oldId = element.LookupParameter("RLX_UniqueIdentifier");

                    string ds_ID = element.LookupParameter("DS_AssetID").AsValueString();

                    if (oldId.AsValueString() != null && oldId.AsValueString().Length == 20)
                    {
                        string oldIdtrimmed = oldId.AsValueString().Substring(0, 12);
                        oldId.Set(oldIdtrimmed + ds_ID);
                    }


                }



                t.Commit();
            }

        }


        public void FamilyToDS()
        {

            UIDocument uidoc = this.ActiveUIDocument;
            Document doc = uidoc.Document;

            Reference familyRef = uidoc.Selection.PickObject(ObjectType.Element, "Pick family");

            FamilyInstance familyInstance = doc.GetElement(familyRef) as FamilyInstance;

            var subElements = familyInstance.GetSubComponentIds();

            GeometryElement geometryElement = familyInstance.get_Geometry(new Options());

            using (Transaction t = new Transaction(doc, "AA"))
            {


                t.Start();
                //				DirectShape directShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));

                // Set a name for the DirectShape
                //		        directShape.Name = "DirectShape from Family";

                // Prepare a list of geometry objects
                List<GeometryObject> geoObjects = new List<GeometryObject>();

                // Iterate over the geometry and add it to the list
                foreach (GeometryObject geoObj in geometryElement)
                {
                    geoObjects.Add(geoObj);

                    DirectShape directShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
                    directShape.SetShape(new List<GeometryObject>() { geoObj });

                }

                // Set the geometry of the DirectShape
                //        directShape.SetShape(geoObjects);

                t.Commit();
            }






        }


        public void ColorByZone()
        {

            UIDocument uidoc = this.ActiveUIDocument;
            Document doc = uidoc.Document;

            FilteredElementCollector elementsInView = new FilteredElementCollector(doc);
            FillPatternElement solidFillPattern = elementsInView.OfClass(typeof(FillPatternElement)).Cast<FillPatternElement>().First(a => a.GetFillPattern().IsSolidFill);


            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
            builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);
            builtInCats.Add(BuiltInCategory.OST_GenericModel);
            builtInCats.Add(BuiltInCategory.OST_PipeCurves);
            builtInCats.Add(BuiltInCategory.OST_PipeInsulations);
            builtInCats.Add(BuiltInCategory.OST_PipeFitting);
            builtInCats.Add(BuiltInCategory.OST_PipeAccessory);


            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


            IList<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();

            var grouped = elements.GroupBy(x => x.LookupParameter("RLX_Zone").AsValueString());

            Random pRand = new Random();
            var md5 = MD5.Create();
            OverrideGraphicSettings ogs = new OverrideGraphicSettings();
            ogs.SetSurfaceForegroundPatternId(solidFillPattern.Id);

            string error = "";
            using (Transaction t = new Transaction(doc, "Override Colors"))
            {
                t.Start();
                foreach (var element in grouped)
                {


                    var firstElement = element.First();
                    string colorName = firstElement.LookupParameter("RLX_Zone").AsValueString();

                    if (colorName == null)
                    {
                        colorName = "null";
                    }
                    var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(colorName));

                    byte iR, iG, iB;
                    iR = Convert.ToByte(pRand.Next(0, 255));
                    iG = Convert.ToByte(pRand.Next(0, 255));
                    iB = Convert.ToByte(pRand.Next(0, 255));

                    Autodesk.Revit.DB.Color pcolor = new Autodesk.Revit.DB.Color(hash[0], hash[1], hash[2]);



                    ogs.SetSurfaceForegroundPatternColor(pcolor);

                    try
                    {
                        //foreach (FamilyInstance item in element)
                        foreach (var item in element)
                        {
                            doc.ActiveView.SetElementOverrides(item.Id, ogs);

                        }

                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                    }
                }

                t.Commit();
            }

            if (error != "")
            {
                TaskDialog.Show("Error", error);
            }





        }


        public void FillAllSpaceNA()
        {

            UIDocument uidoc = this.ActiveUIDocument;
            Document doc = uidoc.Document;


            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
            builtInCats.Add(BuiltInCategory.OST_PipeCurves);
            builtInCats.Add(BuiltInCategory.OST_PipeAccessory);
            builtInCats.Add(BuiltInCategory.OST_Sprinklers);
            builtInCats.Add(BuiltInCategory.OST_GenericModel);
            builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);

            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);

            IList<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();

            using (Transaction t = new Transaction(doc, "Fill Space NA"))
            {

                t.Start();

                foreach (var element in elements)
                {

                    Parameter p = element.LookupParameter("RLX_Space");

                    if (p.AsValueString() == null || p.AsValueString().Length < 2)
                    {

                        p.Set("N/A");
                    }
                }

                t.Commit();

            }
        }







        public void ClosestChainage()
        {
            UIDocument uidoc = this.ActiveUIDocument;
            Document doc = uidoc.Document;

            Reference alignmentsRef = uidoc.Selection.PickObject(ObjectType.Element, "Pick Chainages");

            Element alignment = doc.GetElement(alignmentsRef);

            LocationCurve alignmentLoc = alignment.Location as LocationCurve;

            Curve alignmentCrv = alignmentLoc.Curve;
            List<XYZ> alignmentPts = alignmentCrv.Tessellate().ToList();


            IList<XYZ> tessellation = alignmentCrv.Tessellate();

            List<XYZ> pts = new List<XYZ>(1);

            double stepsize = 1000 / 304.8;
            double dist = 0.0;

            XYZ p = alignmentCrv.GetEndPoint(0);

            foreach (XYZ q in tessellation)
            {
                if (0 == pts.Count)
                {
                    pts.Add(p);
                    dist = 0.0;
                }
                else
                {
                    dist += p.DistanceTo(q);

                    if (dist >= stepsize)
                    {
                        pts.Add(q);
                        dist = 0;
                    }
                    p = q;
                }
            }



            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
            builtInCats.Add(BuiltInCategory.OST_PipeAccessory);
            builtInCats.Add(BuiltInCategory.OST_Sprinklers);
            builtInCats.Add(BuiltInCategory.OST_GenericModel);
            builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);

            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);

            IList<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();

            //			TaskDialog.Show("R", alignmentCrv.Length.ToString());
            using (Transaction t = new Transaction(doc, "Place circles"))
            {

                t.Start();


                foreach (Element element in elements)
                {
                    //TaskDialog.Show("R", element.Name);
                    LocationPoint lp = element.Location as LocationPoint;
                    XYZ projectedPt = new XYZ(lp.Point.X, lp.Point.Y, 0);
                    //				XYZ closestPt = alignmentCrv.Project(lp.Point).XYZPoint;
                    int closestPtIndex = FindClosestPoint(projectedPt, alignmentPts);

                    int closestPtIndex2 = FindClosestPoint(projectedPt, pts);

                    CreateCircle(doc, alignmentCrv.GetEndPoint(0), 3);
                    CreateCircle(doc, alignmentPts[closestPtIndex], 1);
                    CreateCircle(doc, pts[closestPtIndex2], 1);

                    foreach (var pt in pts)
                    {
                        CreateCircle(doc, pt, 2);
                    }

                    List<XYZ> splitLits = alignmentPts.GetRange(0, closestPtIndex);


                    //				TaskDialog.Show("R", closestPtIndex.ToString());
                    TaskDialog.Show("R", (ComputePolylineLength(splitLits) + 680.543).ToString());
                }
                t.Commit();
            }

        }

        public static double ComputePolylineLength(List<XYZ> points)
        {
            if (points == null || points.Count < 2)
            {
                throw new ArgumentException("The list of points must contain at least two points.");
            }

            double totalLength = 0.0;

            for (int i = 0; i < points.Count - 1; i++)
            {
                totalLength += points[i].DistanceTo(points[i + 1]);
            }

            return UnitUtils.ConvertFromInternalUnits(totalLength, UnitTypeId.Meters);
        }

        public int FindClosestPoint(XYZ targetPoint, List<XYZ> points)
        {
            XYZ closestPoint = null;
            double minDistance = double.MaxValue;

            foreach (XYZ point in points)
            {
                double distance = targetPoint.DistanceTo(point);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = point;
                }
            }

            TaskDialog.Show("R", UnitUtils.ConvertFromInternalUnits(minDistance, UnitTypeId.Meters).ToString());
            return points.IndexOf(closestPoint);
        }


        DetailArc CreateCircle(  Document doc,  XYZ location,  double radius)
        {
            XYZ norm = XYZ.BasisZ;

            double startAngle = 0;
            double endAngle = 2 * Math.PI;

            Plane plane = Plane.CreateByNormalAndOrigin(norm, location);



            Arc arc = Arc.Create(plane,
              radius, startAngle, endAngle);

            return doc.Create.NewDetailCurve(
              doc.ActiveView, arc) as DetailArc;
        }
        
        public void CopyToClip()
        {
            UIDocument uidoc = this.ActiveUIDocument;
            Document doc = uidoc.Document;

            IList<Reference> refs = uidoc.Selection.PickObjects(ObjectType.Element, "Select");

            string results = "";

            foreach (var r in refs)
            {

                results += "x.Id == " + doc.GetElement(r).LookupParameter("Comments").AsValueString() + "|| \n";
            }

            Clipboard.SetText(results);
        


        public void ExportSchedule()
        {

            UIDocument uidoc = this.ActiveUIDocument;
            Document doc = this.ActiveUIDocument.Document;

            FilteredElementCollector collection = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule));
            ViewScheduleExportOptions opt = new ViewScheduleExportOptions();

            string folderName = @"C:\Digital Sapiens\Digital Sapiens\Sergio Ros - Riverlinx - Asset Fire Model";

            foreach (ViewSchedule vs in collection)
            {

                if (vs.Name.StartsWith("DS"))
                {
                    try
                    {
                        vs.Export(folderName, vs.Name + ".csv", opt);
                    }
                    catch
                    {
                        TaskDialog.Show("Error", vs.Name);
                    }
                }
            }

            TaskDialog.Show("r", "Done");

        }
        public void ZoomSelected()
        {
            UIDocument uidoc = this.ActiveUIDocument;
            Document doc = uidoc.Document;

            ICollection<ElementId> eids = uidoc.Selection.GetElementIds();

            uidoc.ShowElements(eids);
        }
    }
}