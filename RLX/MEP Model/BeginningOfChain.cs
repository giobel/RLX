#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rhino;
using System.Collections;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Plumbing;

#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class BeginningOfChain : IExternalCommand
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


            List<BuiltInCategory> cats = new List<BuiltInCategory>()
            {
                BuiltInCategory.OST_DuctCurves,
                BuiltInCategory.OST_DuctFitting,
                BuiltInCategory.OST_PipeCurves,
                BuiltInCategory.OST_PipeFitting,
                BuiltInCategory.OST_Conduit,
                BuiltInCategory.OST_ConduitFitting,
                BuiltInCategory.OST_CableTray,
                BuiltInCategory.OST_CableTrayFitting
            };

            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(cats);


            IList<Element> visibleElements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();

            
            XYZ pbp = BasePoint.GetProjectBasePoint(doc).Position;
            XYZ sp = BasePoint.GetSurveyPoint(doc).Position;

            // Calculate the offset from project base point to survey point
            XYZ offset = sp - pbp;


            Transform surveyTransf = doc.ActiveProjectLocation.GetTotalTransform();

            ProjectLocation pl = doc.ActiveProjectLocation;
            Transform ttr = pl.GetTotalTransform().Inverse;

            var grouped = visibleElements.GroupBy(x => x.LookupParameter("RLX_UniqueIdentifier").AsValueString());

            FamilySymbol positionFamily = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfType<FamilySymbol>().Where(x => x.FamilyName == "Position").First();

            //TaskDialog.Show("R", positionFamily.Name);

            int countElements = grouped.Count();
            int counterModified = 0;



            Options options = new Options();
            options.ComputeReferences = true;
            options.IncludeNonVisibleObjects = false;
            options.DetailLevel = ViewDetailLevel.Coarse;



            using (Transaction t = new Transaction(doc, "Fill XYZ"))
            {

                t.Start();

                foreach (var group in grouped)
                {
                    if (group.First().LookupParameter("RLX_UniqueIdentifier").AsValueString() != null)
                    {
                        //collect fittings end points
                        List<XYZ> fittingsPts = new List<XYZ>();

                        List<XYZ> beginningOfChain = new List<XYZ>();

                        //process fittings first
                        foreach (var element in group.Where(e => e.Category.Name.Contains("Fitting")))
                        {
                                GeometryElement ge = element.get_Geometry(options);

                                foreach (GeometryObject geometryObject in ge)
                                {
                                    if (geometryObject is GeometryInstance geometryInstance)
                                    {

                                        GeometryElement instanceGeometry = geometryInstance.GetInstanceGeometry();

                                        foreach (GeometryObject geo  in instanceGeometry)
                                        {
                                            if (geo is Line || geo is Arc)
                                            {
                                                Curve crv = geo as Curve;
                                                fittingsPts.Add(crv.GetEndPoint(0));
                                                fittingsPts.Add(crv.GetEndPoint(1));
                                                //doc.Create.NewFamilyInstance(crv.GetEndPoint(0), positionFamily, StructuralType.NonStructural);
                                                //doc.Create.NewFamilyInstance(crv.GetEndPoint(1), positionFamily, StructuralType.NonStructural);
                                            }
                                        }

                                    }
                                }

                            
                        }

                        foreach (var element in group.Where(e => !e.Category.Name.Contains("Fitting"))){

                            LocationCurve lc = element.Location as LocationCurve;

                            Curve curve = lc.Curve;
                            XYZ startPoint = curve.GetEndPoint(0);
                            XYZ endPoint = curve.GetEndPoint(1);
                            
                            if (fittingsPts.Count > 0)
                            {
                                if (fittingsPts.Min(p => p.DistanceTo(startPoint)) < UnitUtils.ConvertToInternalUnits(100, UnitTypeId.Millimeters))
                                {
                                    //start point is close to fitting
                                    //TaskDialog.Show("R", "Start point is close to fitting");
                                }
                                else
                                {
                                    //start point is not close to fitting --> may be the beginning of the chain
                                    //TaskDialog.Show("R", "Start point is not close to fitting");
                                    //doc.Create.NewFamilyInstance(startPoint, positionFamily, StructuralType.NonStructural);
                                    beginningOfChain.Add(startPoint);

                                }

                                if (fittingsPts.Min(p => p.DistanceTo(endPoint)) < UnitUtils.ConvertToInternalUnits(100, UnitTypeId.Millimeters))
                                {
                                    //start point is close to fitting
                                    //TaskDialog.Show("R", "End point is close to fitting");
                                }
                                else
                                {
                                    //start point is not close to fitting --> may be the beginning of the chain
                                    //TaskDialog.Show("R", "End point is not close to fitting");
                                    //doc.Create.NewFamilyInstance(endPoint, positionFamily, StructuralType.NonStructural);
                                    beginningOfChain.Add(endPoint);
                                }
                            }
                            else
                            {
                                doc.Create.NewFamilyInstance(startPoint, positionFamily, StructuralType.NonStructural);
                                beginningOfChain.Add(startPoint);
                            }
                            //to do: when the pipe starts and ends with fittings!
                        }


                        if (beginningOfChain.Count>0)
                        {
                            XYZ orderedStartPt = beginningOfChain.OrderBy(p => p.X).ThenBy(p => p.Y).ToList().First();

                            orderedStartPt = ttr.OfPoint(orderedStartPt);

                            double metricX = UnitUtils.ConvertFromInternalUnits(orderedStartPt.X, UnitTypeId.Meters);
                            double metricY = UnitUtils.ConvertFromInternalUnits(orderedStartPt.Y, UnitTypeId.Meters);
                            double metricZ = UnitUtils.ConvertFromInternalUnits(orderedStartPt.Z, UnitTypeId.Meters);

                            doc.Create.NewFamilyInstance(beginningOfChain.OrderBy(p => p.X).ThenBy(p => p.Y).ToList().First(), positionFamily, StructuralType.NonStructural);

                            Helpers.FillXYZParam(group, metricX, metricY, metricZ);
                        }

                    }

                }
                t.Commit();
            }
                        counterModified++;
            TaskDialog.Show("R", $"{counterModified} modified of {countElements}");

            return Result.Succeeded;
        }
    }
}
