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
using Autodesk.Revit.DB.Plumbing;

#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class Duct_Pipe_BeginningOfChain1 : IExternalCommand
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


            List<BuiltInCategory> cats = new List<BuiltInCategory>
            {
                BuiltInCategory.OST_DuctCurves,
                BuiltInCategory.OST_DuctFitting,
                BuiltInCategory.OST_PipeCurves,
                BuiltInCategory.OST_PipeFitting
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

                    List<XYZ> points = new List<XYZ>();

                    foreach (Element eleRef in group)
                    {
                        if (eleRef is Duct)
                        {

                            Duct duct = eleRef as Duct;
                            LocationCurve locCurve = duct.Location as LocationCurve;

                            Curve curve = locCurve.Curve;
                            XYZ startPoint = curve.GetEndPoint(0);
                            XYZ endPoint = curve.GetEndPoint(1);


                                points.Add(startPoint);
                                points.Add(endPoint);

                            
                        }
                        else if(eleRef is Pipe)
                            {
                                Pipe pipe = eleRef as Pipe;
                                LocationCurve locCurve = pipe.Location as LocationCurve;

                                Curve curve = locCurve.Curve;
                                XYZ startPoint = curve.GetEndPoint(0);
                                XYZ endPoint = curve.GetEndPoint(1);

                                points.Add(startPoint);
                                points.Add(endPoint);
                            }
                        }


                    List<XYZ> endPoints = new List<XYZ>();

                    List<XYZ> ordered = points.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();


                    //FamilyInstance instance = doc.Create.NewFamilyInstance(ordered.First(), positionFamily, StructuralType.NonStructural);


                    XYZ startPt = null;

                    if (ordered.Count > 3)
                    {
                        startPt = Helpers.GetStartPoint(doc, ordered);
                    }
                    else { 
                    
                        startPt = ordered.First();
                    }



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
