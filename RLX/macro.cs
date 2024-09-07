using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLX
{
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
            public void FillCommonParams()
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


                ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


                IList<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();


                using (Transaction t = new Transaction(doc, "Fill params"))
                {

                    t.Start();

                    foreach (Element element in elements)
                    {

                        Parameter fac = element.LookupParameter("RLX_Facility");
                        fac.Set("Silvertown Tunnel");

                        Parameter typ = element.LookupParameter("RLX_Type");
                        typ.Set("Project Facilites");

                        Parameter grid = element.LookupParameter("RLX_GridReferenceSystem");
                        grid.Set("BNG");

                        Parameter cost = element.LookupParameter("RLX_MaintenanceCost");
                        cost.Set("N/A");

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

            public void FillPipeMaterials()
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

                ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


                IList<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();


                ElementId noMaterialId = new ElementId(-1);

                using (Transaction t = new Transaction(doc, "Fill materials"))
                {

                    t.Start();

                    foreach (Element element in elements)
                    {

                        Parameter mat = element.LookupParameter("RLX_MainMaterial");

                        //					TaskDialog.Show("R", element.LookupParameter("Material").AsElementId().ToString());



                        if (mat.AsElementId() == noMaterialId)
                        {

                            mat.Set(element.LookupParameter("Material").AsElementId());

                        }



                    }


                    t.Commit();
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

            public void FillMechEquipTitle()
            {
                UIDocument uidoc = this.ActiveUIDocument;
                Document doc = uidoc.Document;


                List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
                builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);

                ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


                IList<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();


                using (Transaction t = new Transaction(doc, "Fill params"))
                {

                    t.Start();

                    foreach (Element element in elements)
                    {

                        Parameter title = element.LookupParameter("RLX_Title");

                        if (title == null || title.AsValueString().Length < 3)
                        {

                            title.Set(element.LookupParameter("Family").AsValueString());

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

            public void FillValvesDSids()
            {
                UIDocument uidoc = this.ActiveUIDocument;
                Document doc = uidoc.Document;


                List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
                builtInCats.Add(BuiltInCategory.OST_PipeAccessory);

                ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


                IList<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();

                //			TaskDialog.Show("R", elements.Count().ToString());

                int counter = 3500;

                using (Transaction t = new Transaction(doc, "Fill valves params"))
                {

                    t.Start();

                    foreach (var fa in elements)
                    {

                        try
                        {


                            Element element = fa as Element;



                            Parameter assetId = element.LookupParameter("DS_AssetID");

                            string assetIdvalue = assetId.AsValueString();

                            //					TaskDialog.Show("R", assetIdvalue.Length.ToString());

                            if (assetIdvalue == null || assetIdvalue.Length < 3)
                            {

                                assetId.Set(counter.ToString());

                                counter++;

                            }



                        }
                        catch (Exception ex)
                        {
                            TaskDialog.Show("R", ex.Message);
                        }

                    }


                    t.Commit();
                }

            }

            public void GenerateSprinklersDSids()
            {
                UIDocument uidoc = this.ActiveUIDocument;
                Document doc = uidoc.Document;

                IList<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_Sprinklers).WhereElementIsNotElementType().ToElements();

                //			TaskDialog.Show("R", elements.Count().ToString());

                int counter = 3000;

                using (Transaction t = new Transaction(doc, "Fill sprinklers params"))
                {

                    t.Start();

                    foreach (FamilyInstance fa in elements)
                    {

                        try
                        {


                            Element element = fa as Element;



                            Parameter assetId = element.LookupParameter("DS_AssetID");

                            string assetIdvalue = assetId.AsValueString();

                            //					TaskDialog.Show("R", assetIdvalue.Length.ToString());

                            if (assetIdvalue == null || assetIdvalue.Length < 3)
                            {

                                assetId.Set(counter.ToString());

                                counter++;

                            }



                        }
                        catch (Exception ex)
                        {
                            TaskDialog.Show("R", ex.Message);
                        }

                    }


                    t.Commit();
                }

            }



            public void GenerateNozzlesDSids()
            {
                UIDocument uidoc = this.ActiveUIDocument;
                Document doc = uidoc.Document;

                IList<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_GenericModel).WhereElementIsNotElementType().ToElements();

                //			TaskDialog.Show("R", elements.Count().ToString());

                int counter = 3700;

                using (Transaction t = new Transaction(doc, "Generate Nozzles Ids"))
                {

                    t.Start();

                    foreach (FamilyInstance fa in elements)
                    {

                        try
                        {


                            Element element = fa as Element;



                            Parameter assetId = element.LookupParameter("DS_AssetID");

                            string assetIdvalue = assetId.AsValueString();

                            //					TaskDialog.Show("R", assetIdvalue.Length.ToString());

                            if (assetIdvalue == null || assetIdvalue.Length < 3)
                            {

                                assetId.Set(counter.ToString());

                                counter++;

                            }



                        }
                        catch (Exception ex)
                        {
                            TaskDialog.Show("R", ex.Message);
                        }

                    }


                    t.Commit();
                }

            }


            public void FillSprinklersParams()
            {
                UIDocument uidoc = this.ActiveUIDocument;
                Document doc = uidoc.Document;


                List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
                builtInCats.Add(BuiltInCategory.OST_Sprinklers);


                ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


                IList<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();


                List<Tuple<string, string>> data = new List<Tuple<string, string>>();
                data.Add(new Tuple<string, string>("RLX_ClassificationUniclassEF_Description", "Fire-extinguishing supply"));
                data.Add(new Tuple<string, string>("RLX_ClassificationUniclassEF_Number", "EF_55_30"));
                data.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Description", "Foam sprinklers"));
                data.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Number", "Pr_70_55_33_30"));
                data.Add(new Tuple<string, string>("RLX_ClassificationUniclassSs_Description", "Fire-stopping systems"));
                data.Add(new Tuple<string, string>("RLX_ClassificationUniclassSs_Number", "Ss_25_60_30"));
                data.Add(new Tuple<string, string>("DS_AssetType", "NOZ"));


                using (Transaction t = new Transaction(doc, "Fill uniclasses"))
                {

                    t.Start();

                    foreach (Element element in elements)
                    {

                        foreach (var info in data)
                        {

                            Parameter p = element.LookupParameter(info.Item1);

                            p.Set(info.Item2);
                        }

                        Parameter title = element.LookupParameter("RLX_Title");

                        if (title.AsValueString() == null || title.AsValueString().Length < 3)
                        {

                            title.Set(element.LookupParameter("Family").AsValueString());
                        }

                        ElementType type = doc.GetElement(element.GetTypeId()) as ElementType;
                        ElementId materialId = type.LookupParameter("Frame Material").AsElementId();

                        element.LookupParameter("RLX_MainMaterial").Set(materialId);

                    }


                    t.Commit();
                }

            }




            public void FillPipesAndHangersUniclassParams()
            {
                UIDocument uidoc = this.ActiveUIDocument;
                Document doc = uidoc.Document;


                List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
                builtInCats.Add(BuiltInCategory.OST_PipeInsulations);
                builtInCats.Add(BuiltInCategory.OST_PipeCurves);
                builtInCats.Add(BuiltInCategory.OST_PipeInsulations);
                builtInCats.Add(BuiltInCategory.OST_PipeFitting);
                builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);

                ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


                IList<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();


                List<Tuple<string, string>> data = new List<Tuple<string, string>>();
                data.Add(new Tuple<string, string>("RLX_ClassificationUniclassEF_Description", "Fire-extinguishing supply"));
                data.Add(new Tuple<string, string>("RLX_ClassificationUniclassEF_Number", "EF_55_30"));
                data.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Description", "Pipes and fittings"));
                data.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Number", "Pr_65_52_63"));
                data.Add(new Tuple<string, string>("RLX_ClassificationUniclassSs_Description", "Fire-stopping systems"));
                data.Add(new Tuple<string, string>("RLX_ClassificationUniclassSs_Number", "Ss_25_60_30"));
                data.Add(new Tuple<string, string>("DS_AssetType", "PPW"));


                using (Transaction t = new Transaction(doc, "Fill uniclasses"))
                {

                    t.Start();

                    foreach (Element element in elements)
                    {

                        foreach (var info in data)
                        {

                            Parameter p = element.LookupParameter(info.Item1);

                            p.Set(info.Item2);
                        }

                    }


                    t.Commit();
                }

            }

            public void FillValvesUniclassParams()
            {
                UIDocument uidoc = this.ActiveUIDocument;
                Document doc = uidoc.Document;


                List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
                builtInCats.Add(BuiltInCategory.OST_PipeAccessory);
                builtInCats.Add(BuiltInCategory.OST_GenericModel);


                ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


                IList<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();


                List<Tuple<string, string>> data = new List<Tuple<string, string>>();
                data.Add(new Tuple<string, string>("RLX_ClassificationUniclassEF_Description", "Fire-extinguishing supply"));
                data.Add(new Tuple<string, string>("RLX_ClassificationUniclassEF_Number", "EF_55_30"));
                data.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Description", "Deluge valves"));
                data.Add(new Tuple<string, string>("RLX_ClassificationUniclassPr_Number", "Pr_65_54_30_22"));
                data.Add(new Tuple<string, string>("RLX_ClassificationUniclassSs_Description", "Fire-stopping systems"));
                data.Add(new Tuple<string, string>("RLX_ClassificationUniclassSs_Number", "Ss_25_60_30"));
                data.Add(new Tuple<string, string>("DS_AssetType", "VAV"));


                using (Transaction t = new Transaction(doc, "Fill uniclasses"))
                {

                    t.Start();

                    foreach (Element element in elements)
                    {

                        foreach (var info in data)
                        {

                            Parameter p = element.LookupParameter(info.Item1);

                            p.Set(info.Item2);
                        }

                    }


                    t.Commit();
                }

            }

            public void FillUniqueId()
            {
                UIDocument uidoc = this.ActiveUIDocument;
                Document doc = uidoc.Document;


                List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
                builtInCats.Add(BuiltInCategory.OST_PipeInsulations);
                builtInCats.Add(BuiltInCategory.OST_PipeCurves);
                builtInCats.Add(BuiltInCategory.OST_PipeInsulations);
                builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);
                builtInCats.Add(BuiltInCategory.OST_PipeAccessory);
                builtInCats.Add(BuiltInCategory.OST_Sprinklers);
                builtInCats.Add(BuiltInCategory.OST_GenericModel);




                ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


                IList<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();


                using (Transaction t = new Transaction(doc, "Fill uniqueIDS"))
                {

                    t.Start();

                    foreach (Element element in elements)
                    {

                        string axis = element.LookupParameter("DS_Axis").AsValueString();
                        string chainage = "xxxx";
                        string location = element.LookupParameter("DS_Location").AsValueString();
                        string lane = element.LookupParameter("DS_Lane").AsValueString();
                        string assetType = element.LookupParameter("DS_AssetType").AsValueString();
                        string id = element.LookupParameter("DS_AssetID").AsValueString();



                        Parameter p = element.LookupParameter("RLX_UniqueIdentifier");

                        if (p.AsValueString() == null || p.AsValueString().Length < 3)
                        {

                            p.Set(axis + chainage + location + lane + assetType + id);

                        }
                    }


                    t.Commit();
                }

            }


            public void CopyOLDUniqueId()
            {
                UIDocument uidoc = this.ActiveUIDocument;
                Document doc = uidoc.Document;


                List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
                builtInCats.Add(BuiltInCategory.OST_PipeInsulations);
                builtInCats.Add(BuiltInCategory.OST_PipeCurves);
                builtInCats.Add(BuiltInCategory.OST_PipeInsulations);
                builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);
                builtInCats.Add(BuiltInCategory.OST_PipeAccessory);
                builtInCats.Add(BuiltInCategory.OST_Sprinklers);
                builtInCats.Add(BuiltInCategory.OST_GenericModel);




                ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);


                IList<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();


                using (Transaction t = new Transaction(doc, "Copy old uniqueIDS"))
                {

                    t.Start();

                    foreach (Element element in elements)
                    {

                        string oldId = element.LookupParameter("RLX_UniqueIdentifier").AsValueString();

                        string axis = oldId.Substring(0, 1);
                        string location = oldId.Substring(5, 3);
                        string lane = oldId.Substring(8, 1);

                        Parameter p = element.LookupParameter("DS_OLD UID");

                        p.Set(oldId);

                        Parameter ds_axis = element.LookupParameter("DS_Axis");
                        ds_axis.Set(axis);

                        Parameter ds_location = element.LookupParameter("DS_Location");
                        ds_location.Set(location);

                        Parameter ds_lane = element.LookupParameter("DS_Lane");
                        ds_lane.Set(lane);

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

            public void ColorByDS_ID()
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

                var grouped = elements.GroupBy(x => x.LookupParameter("DS_AssetID").AsValueString());

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
                        string colorName = firstElement.LookupParameter("DS_AssetID").AsValueString();

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

            public void ColorByRLX_ID()
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

                var grouped = elements.GroupBy(x => x.LookupParameter("RLX_UniqueIdentifier").AsValueString());

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
                        string colorName = firstElement.LookupParameter("RLX_UniqueIdentifier").AsValueString();

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


            public void FillXYZ()
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

                var grouped = elements.GroupBy(x => x.LookupParameter("DS_AssetID").AsValueString());

                XYZ pbp = BasePoint.GetProjectBasePoint(doc).Position;
                XYZ sp = BasePoint.GetSurveyPoint(doc).Position;

                // Calculate the offset from project base point to survey point
                XYZ offset = sp - pbp;


                Transform surveyTransf = doc.ActiveProjectLocation.GetTotalTransform();

                ProjectLocation pl = doc.ActiveProjectLocation;
                Transform ttr = pl.GetTotalTransform().Inverse;

                using (Transaction t = new Transaction(doc, "Fill XYZ"))
                {

                    t.Start();

                    foreach (var element in grouped)
                    {

                        XYZ cen = GetElementCentroid(element.First());




                        //XYZ newC = surveyTransf.OfPoint(cen);
                        //					TaskDialog.Show("R", String.Format("{0} {1} {2}", newC.X, newC.Y, newC.Z));

                        //					XYZ centroid = CalculateCentroidOfElements(element) - offset;

                        XYZ centroid = ttr.OfPoint(CalculateCentroidOfElements(element));

                        double metricX = UnitUtils.ConvertFromInternalUnits(centroid.X, UnitTypeId.Meters);
                        double metricY = UnitUtils.ConvertFromInternalUnits(centroid.Y, UnitTypeId.Meters);
                        double metricZ = UnitUtils.ConvertFromInternalUnits(centroid.Z, UnitTypeId.Meters);

                        //TaskDialog.Show("R", String.Format("{0} {1} {2}", metricX, metricY, metricZ));

                        FillXYZParam(element, metricX, metricY, metricZ);

                    }

                    t.Commit();
                }

            }

            private XYZ GetElementCentroid(Element element)
            {
                BoundingBoxXYZ bbox = element.get_BoundingBox(null);
                if (bbox == null)
                    return null;

                XYZ min = bbox.Min;
                XYZ max = bbox.Max;

                return new XYZ((min.X + max.X) / 2, (min.Y + max.Y) / 2, (min.Z + max.Z) / 2);
            }


            private XYZ CalculateCentroidOfElements(IEnumerable<Element> elements)
            {
                if (elements == null || !elements.Any())
                    return null;

                XYZ sumCentroid = new XYZ(0, 0, 0);
                int count = 0;

                foreach (Element element in elements)
                {
                    XYZ centroid = GetElementCentroid(element);
                    if (centroid != null)
                    {
                        sumCentroid = sumCentroid.Add(centroid);
                        count++;
                    }
                }

                if (count == 0)
                    return null;

                return new XYZ(sumCentroid.X / count, sumCentroid.Y / count, sumCentroid.Z / count);
            }


            private void FillXYZParam(IEnumerable<Element> elements, double x, double y, double z)
            {

                foreach (Element element in elements)
                {
                    Parameter _x = element.LookupParameter("RLX_CoordinatesX");
                    _x.Set(x.ToString());
                    Parameter _y = element.LookupParameter("RLX_CoordinatesY");
                    _y.Set(y.ToString());
                    Parameter _z = element.LookupParameter("RLX_CoordinatesZ");
                    _z.Set(z.ToString());
                }


            }



            public void PipeFittingsCopyClosestPipeParams()
            {

                UIDocument uidoc = this.ActiveUIDocument;
                Document doc = uidoc.Document;

                IList<Element> allPipes = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_PipeCurves).WhereElementIsNotElementType().ToElements();

                List<Curve> pipeCurves = new List<Curve>();

                foreach (Element pipe in allPipes)
                {


                    LocationCurve pipeCurve = pipe.Location as LocationCurve;
                    pipeCurves.Add(pipeCurve.Curve);
                }

                if (allPipes.Count() != pipeCurves.Count())
                {

                    TaskDialog.Show("Error", "Cannot find a curve for some pipes");
                    return;
                }

                IList<Element> allPipeFittings = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_PipeFitting).WhereElementIsNotElementType().ToElements();

                Element closestPipe = null;
                double distance = 1000;

                List<string> paramsToSet = new List<string>(){"RLX_ActualCost","RLX_ClassificationUniclassEF_Description",
                "RLX_ClassificationUniclassEF_Number","RLX_ClassificationUniclassPr_Description",
                "RLX_ClassificationUniclassPr_Number","RLX_ClassificationUniclassSs_Description",
                "RLX_ClassificationUniclassSs_Number","RLX_Component","RLX_CoordinatesX","RLX_CoordinatesY",
                "RLX_CoordinatesZ","RLX_Facility","RLX_GridReferenceSystem","RLX_Location","RLX_MaintenanceCost",
                "RLX_Space","RLX_Specification","RLX_System","RLX_Type","RLX_UniqueIdentifier","RLX_Zone","DS_AssetID"};

                using (Transaction t = new Transaction(doc, "Pipe fittings copy pipe parameters"))
                {
                    t.Start();

                    foreach (Element fitting in allPipeFittings)
                    {

                        LocationPoint point = fitting.Location as LocationPoint;

                        for (int i = 0; i < allPipes.Count(); i++)
                        {
                            double currentDistance = pipeCurves[i].Distance(point.Point);
                            if (currentDistance < distance)
                            {

                                distance = currentDistance;
                                closestPipe = allPipes[i];

                            }
                        }

                        foreach (string paramName in paramsToSet)
                        {
                            //					TaskDialog.Show("R", paramName);
                            string p = closestPipe.LookupParameter(paramName).AsValueString();
                            fitting.LookupParameter(paramName).Set(p);
                        }


                        closestPipe = null;
                        distance = 1000;

                    }
                    t.Commit();
                }//close transaction


            }



            public void FillEmptyDescriptions()
            {

                UIDocument uidoc = this.ActiveUIDocument;
                Document doc = uidoc.Document;


                List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
                builtInCats.Add(BuiltInCategory.OST_PipeCurves);
                builtInCats.Add(BuiltInCategory.OST_PipeFitting);
                builtInCats.Add(BuiltInCategory.OST_PipeAccessory);
                builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);
                builtInCats.Add(BuiltInCategory.OST_Sprinklers);
                builtInCats.Add(BuiltInCategory.OST_PlaceHolderPipes);
                builtInCats.Add(BuiltInCategory.OST_GenericModel);





                ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);

                IList<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();

                using (Transaction t = new Transaction(doc, "Fill empty descriptions"))
                {
                    t.Start();

                    foreach (var element in elements)
                    {

                        string descr = element.LookupParameter("RLX_Description").AsValueString();

                        if (descr == null || descr.Length < 3)
                        {

                            string title = element.LookupParameter("RLX_Title").AsValueString();
                            string location = element.LookupParameter("RLX_Location").AsValueString();
                            string zone = element.LookupParameter("RLX_Zone").AsValueString();
                            string space = element.LookupParameter("RLX_Space").AsValueString();

                            element.LookupParameter("RLX_Description").Set(String.Format("{0} {1} {2} {3}", title, location, zone, space));
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


            DetailArc CreateCircle(
      Document doc,
      XYZ location,
      double radius)
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
        }
    }
}
