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
using RG = Rhino.Geometry;
#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class AlignmentToModelCurve : IExternalCommand
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

            Reference importRef = uidoc.Selection.PickObject(ObjectType.Element, "Pick alignment dwg");

            ImportInstance importInstance = doc.GetElement(importRef) as ImportInstance;

            Options opt = new Options();

            opt.DetailLevel = ViewDetailLevel.Fine;

            GeometryElement geometryElement = importInstance.get_Geometry(new Options());

            Level level = new FilteredElementCollector(doc)
                            .OfClass(typeof(Level))
                            .FirstElement() as Level;

            Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, new XYZ(0, 0, level.Elevation));

        

            using (Transaction t = new Transaction(doc, "Convert alignment"))
            {
                t.Start();

                SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
    
                if (geometryElement != null)
                {
                    // Iterate through geometry objects
                    foreach (GeometryObject geometryObject in geometryElement)
                    {
                        if (geometryObject is GeometryInstance geometryInstance)
                        {
                            // Handle geometry instance
                            GeometryElement instanceGeometry = geometryInstance.GetInstanceGeometry();

                            List<GeometryObject> go = new List<GeometryObject>();
                            List<Curve> curves = new List<Curve>();

                            

                            foreach (GeometryObject geom in instanceGeometry)
                            {
                                if (geom is Line)
                                {
                                    //Curve crv = geom as Curve;
                                    //doc.Create.NewModelCurve(crv, sketchPlane);
                                }
                                else if (geom is NurbSpline)
                                {
                                    NurbSpline crv = geom as NurbSpline;
                                    
                                    doc.Create.NewModelCurve(crv, sketchPlane);
                                }
                                else if (geom is Arc)
                                {
                                    Arc crv = geom as Arc;

                                    doc.Create.NewModelCurve(crv, sketchPlane);
                                }
                            }

                            //NurbSpline ns = NurbSpline.CreateCurve()

                            //CurveLoop curveLoop = CurveLoop.Create(curves);
                            //doc.Create.NewModelCurve(curveLoop, sketchPlane);
                            //DirectShape directShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
                            //directShape.SetShape(go);
                        }
                    }
                }

                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}
