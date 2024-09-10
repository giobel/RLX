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
using RG = Rhino.Geometry;
#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class FindChainage : IExternalCommand
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


            Reference alignmentsRef = uidoc.Selection.PickObject(ObjectType.Element, "Pick Chainages");

            Element alignment = doc.GetElement(alignmentsRef);

            

            LocationCurve alignmentLoc = alignment.Location as LocationCurve;

            Curve alignmentCrv = alignmentLoc.Curve;

            string alignmentName = doc.GetElement(alignmentCrv.GraphicsStyleId).Name;

            //TaskDialog.Show("R", alignmentName);

            double chainageStart = 0;

            if (alignmentName == "SOUTH BOUND")
            {
                chainageStart = 680;
            }
            else if (alignmentName == "NORTH BOUND")
            {
                chainageStart = 680.543;
            }

            List<XYZ> tessellation = alignmentCrv.Tessellate().ToList();

            List<RG.Point3d> rhinoPts = new List<RG.Point3d>();

            foreach (var item in tessellation)
            {
                rhinoPts.Add(Helpers.RevitToRhinoPt(item));
            }

            RG.Polyline rhinoAlignment = new RG.Polyline(rhinoPts);


            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
            builtInCats.Add(BuiltInCategory.OST_PipeAccessory);
            builtInCats.Add(BuiltInCategory.OST_Sprinklers);
            builtInCats.Add(BuiltInCategory.OST_GenericModel);
            builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);
            builtInCats.Add(BuiltInCategory.OST_PipeFitting);
            builtInCats.Add (BuiltInCategory.OST_PipeCurves);


            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);

            IList<Element> mepObjects = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();

            using (Transaction t = new Transaction(doc, "Find closest chainage"))
            {

                t.Start();

                foreach (Element element in mepObjects)
                {


                    XYZ MEPprojectedPt = null;

                    //if (element.Category.Name == "Pipes")
                    //{
                    //    LocationCurve locationCurve = element.Location as LocationCurve;
                    //    MEPprojectedPt = locationCurve.Curve.Evaluate(0.5, true);
                    //}
                    
                    LocationPoint MEPLp = element.Location as LocationPoint;
                    
                    if (MEPLp != null)
                    {
                        MEPprojectedPt = new XYZ(MEPLp.Point.X, MEPLp.Point.Y, 0);
                    }
                    else
                    {
                        //exclude the pipes with a location curve. use the other script
                        LocationCurve lc = element.Location as LocationCurve;
                        if (lc == null)
                        {
                        //these are the directshape elements -> location is null
                        XYZ centroid = Helpers.GetElementCentroid(element);
                        MEPprojectedPt = new XYZ(centroid.X, centroid.Y, 0);
                        }
                    }

                    if (MEPprojectedPt != null)
                    {


                        IntersectionResult intersection = alignmentCrv.Project(MEPprojectedPt);

                        XYZ pointOnAlign = intersection.XYZPoint;

                        //Helpers.CreateCircle(doc, pointOnAlign, 2);

                        //RHINO
                        RG.Point3d closestPt = rhinoAlignment.ClosestPoint(Helpers.RevitToRhinoPt(MEPprojectedPt));

                        double rhinoPar = rhinoAlignment.ClosestParameter(closestPt);
                        RG.Curve[] plcurve = rhinoAlignment.ToPolylineCurve().Split(rhinoPar);

                        RG.Polyline splitAlignment = null;

                        plcurve[0].TryGetPolyline(out splitAlignment);

                        double chainage = splitAlignment.Length * 0.3048 + chainageStart;

                        element.LookupParameter("DS_Chainage").Set(chainage.ToString());
                    }
                    //TaskDialog.Show("Length", chainage.ToString());


                    //TaskDialog.Show("Rhino", $"{closestPt.X * 304.8} {closestPt.Y * 304.8} ");

                    //


                    //TaskDialog.Show("R", $"{pointOnAlign.X * 304.8} {pointOnAlign.Y * 304.8} ");
                    //TaskDialog.Show("R", parameter.ToString());

                    //foreach (var item in tessellation)
                    //{
                    //    Helpers.CreateCircle(doc, item, 2);
                    //}
                }

                t.Commit();

                return Result.Succeeded;
            }
        }
    }
}
