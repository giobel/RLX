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
using System.Security.Cryptography;
using System.Text;
#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class CombineModelCurvevs : IExternalCommand
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

            IList<Reference> modelCurvesRefs = uidoc.Selection.PickObjects(ObjectType.Element, "Pick model curves");

            Options opt = new Options();

            opt.DetailLevel = ViewDetailLevel.Fine;


            Level level = new FilteredElementCollector(doc)
                            .OfClass(typeof(Level))
                            .FirstElement() as Level;

            Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, new XYZ(0, 0, level.Elevation));

            IList<double> knots = new List<double>();
            IList<XYZ> ctrlPts= new List<XYZ>();
            IList<double> weights = new List<double>();

            CurveArray ca = new CurveArray();

            using (Transaction t = new Transaction(doc, "Convert alignment"))
            {
                t.Start();

                SketchPlane sketchPlane = SketchPlane.Create(doc, plane);

                foreach (var item in modelCurvesRefs)
                {
                    Element ele = doc.GetElement(item);
                    LocationCurve lc = ele.Location as LocationCurve;

                    var crv = lc.Curve;

                    if (crv is NurbSpline)
                    {
                        ca.Append(crv);
                        NurbSpline ns = crv as NurbSpline;
                        foreach (XYZ p in crv.Tessellate())
                        {
                            ctrlPts.Add(p);
                        } 


                        //foreach (var pt in ns.CtrlPoints)
                        //{
                        //    ctrlPts.Add(pt);
                        //}
                        //foreach (double w in ns.Weights)
                        //{
                        //    weights.Add(w);
                        //}
                        //foreach (double knot in ns.Knots)
                        //{
                        //    knots.Add(knot);
                        //}
                    }


                }

                

                //Curve nas = NurbSpline.CreateCurve(ctrlPts, knots);

                HermiteSpline hs = HermiteSpline.Create(ctrlPts, false);

                //doc.Create.NewModelCurveArray(ca, sketchPlane);

                doc.Create.NewModelCurve(hs, sketchPlane);

                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}
