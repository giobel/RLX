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
    public class Duct_Pipe_BeginningOfChain2 : IExternalCommand
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



            using (Transaction t = new Transaction(doc, "Fill XYZ"))
            {

                t.Start();

                foreach (var group in grouped)
                {
                    if (group.First().LookupParameter("RLX_UniqueIdentifier").AsValueString() != null)
                    {

                        List<XYZ> pts = new List<XYZ>();

                        foreach (Element eleRef in group)
                        {
                            if (eleRef is Duct || eleRef is Pipe || eleRef is Conduit || eleRef is CableTray)
                            {

                                LocationCurve locCurve = null;

                                switch (eleRef.Category.Name)
                                {
                                    case "Ducts":
                                        Duct duct = eleRef as Duct;
                                        locCurve = duct.Location as LocationCurve;
                                        break;
                                    case "Pipes":
                                        Pipe pipe = eleRef as Pipe;
                                        locCurve = pipe.Location as LocationCurve;
                                        break;
                                    case "Conduits":
                                        Conduit conduit = eleRef as Conduit;
                                        locCurve = conduit.Location as LocationCurve;
                                        break;
                                    case "Cable Trays":
                                        CableTray cableTray = eleRef as CableTray;
                                        locCurve = cableTray.Location as LocationCurve;
                                        break;

                                }


                                Curve curve = locCurve.Curve;
                                XYZ startPoint = curve.GetEndPoint(0);
                                XYZ endPoint = curve.GetEndPoint(1);


                                pts.Add(startPoint);
                                pts.Add(endPoint);


                            }
                        }

                        XYZ centroid = Helpers.ComputeCentroid(pts);

                        var orderedPts = pts.OrderBy(x => x.DistanceTo(centroid)).ToList();

                        //set the most distant point for the walk. For the code to work this should be the most isolated point
                        XYZ pt = orderedPts.Last();


                        // Generate index list
                        List<int> srsList = Enumerable.Range(0, pts.Count).ToList();

                        //ssorted points
                        List<XYZ> ordered = new List<XYZ>();
                        List<XYZ> ptList = new List<XYZ>();

                        int j, k;

                        // Loop through all points
                        for (k = 0; k < pts.Count; k++)
                        {
                            j = Helpers.ClosestIndex(pts, pt);
                            ptList.Add(pts[j]);
                            //intList.Add(srsList[j]);
                            pt = pts[j];

                            // Remove the selected point and its corresponding index
                            pts.RemoveAt(j);
                            srsList.RemoveAt(j);
                        }


                        List<XYZ> origin = new List<XYZ>() { ptList.First(), ptList.Last() };

                        XYZ startPt = origin.OrderBy(x => x.X).ThenBy(x => x.Y).ToList().First();

                        doc.Create.NewFamilyInstance(startPt, positionFamily, StructuralType.NonStructural);


                        if (startPt != null)
                    {
                        startPt = ttr.OfPoint(startPt);
                    }
                    else
                    {

                        TaskDialog.Show("R", "Failed");
                        return Result.Failed;
                    }

                    double metricX = UnitUtils.ConvertFromInternalUnits(startPt.X, UnitTypeId.Meters);
                    double metricY = UnitUtils.ConvertFromInternalUnits(startPt.Y, UnitTypeId.Meters);
                    double metricZ = UnitUtils.ConvertFromInternalUnits(startPt.Z, UnitTypeId.Meters);

                    //TaskDialog.Show("R", String.Format("{0} {1} {2}", metricX, metricY, metricZ));

                    Helpers.FillXYZParam(group, metricX, metricY, metricZ);

                    counterModified++;
                    }

                }//close foreach group

                t.Commit();
            }
            TaskDialog.Show("R", $"{counterModified} modified of {countElements}");

            return Result.Succeeded;
        }
    }
}
