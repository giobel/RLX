#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RG = Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class MergeCurves : IExternalCommand
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


            IList<Reference> alignmentsRef = uidoc.Selection.PickObjects(ObjectType.Element, "Pick curves");

            List<Curve> allCurves = new List<Curve>();

            CurveArray curveArray = new CurveArray();
            
            List<ModelCurve> mcurveArray = new List<ModelCurve>();

            List<XYZ> points = new List<XYZ>();

            List<RG.Point3d> rhinoPts = new List<RG.Point3d>();

            foreach (var item in alignmentsRef)
            {
                Element alignment = doc.GetElement(item);

                ModelCurve mc = alignment as ModelCurve;

                mcurveArray.Append(mc);

                LocationCurve locationCurve = alignment.Location as LocationCurve;

                allCurves.Add(locationCurve.Curve);
                curveArray.Append(locationCurve.Curve);

                foreach (XYZ pt in locationCurve.Curve.Tessellate())
                {
                    points.Add(pt);
                    rhinoPts.Add(new RG.Point3d(pt.X, pt.Y, pt.Z));
                }

            }

            var newList = points.Distinct(new XYZEqualityComparer()).ToList();

            newList = Helpers.SortPoints(newList);

            Element level = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).First();

            //HermiteSpline hs = HermiteSpline.Create(newList, true);

            RG.Polyline polyline = new RG.Polyline(rhinoPts);

            TaskDialog.Show("R", (polyline.Length*304.8).ToString());
            
            
            using (Transaction t = new Transaction(doc, "Import alignments"))
            {
                t.Start();


                //SketchPlane sp = SketchPlane.Create(doc, level.Id);

                
               


                //doc.Create.NewModelCurve(hs, sp);


                //doc.Create.NewModelCurveArray(curveArray, sp);

                //for (int i = 0; i < mcurveArray.Count - 1; i++)
                //{
                //    ModelCurve curve1 = mcurveArray[i];
                //    for (int j = i + 1; j < mcurveArray.Count; j++)
                //    {
                //        ModelCurve curve2 = mcurveArray[j];

                //        // Join the curves
                //        JoinGeometryUtils.JoinGeometry(doc, curve1, curve2);
                //    }

                //}


                t.Commit();

            }
            return Result.Succeeded;
        
}
    }
}
