using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using MathNet.Numerics.Providers.LinearAlgebra;
using Rhino.UI;
using static Autodesk.Revit.DB.SpecTypeId;
using RG = Rhino.Geometry;

namespace RLX
{

    class XyzProximityComparer : IComparer<XYZ>
    {
        XYZ _p;

        public XyzProximityComparer(XYZ p)
        {
            _p = p;
        }

        public int Compare(XYZ x, XYZ y)
        {
            double dx = x.DistanceTo(_p);
            double dy = y.DistanceTo(_p);
            return Util.IsEqual(dx, dy) ? 0
              : (dx < dy ? -1 : 1);
        }
    }

    internal class Helpers
    {


        public static void CopyUniclassFromType(Document doc, Element element)
        {
            Element eleType = doc.GetElement(element.GetTypeId());

            //Parameter prTitle = eleType.LookupParameter("Identity_Classification_Uniclass 2015_Pr_Title");

            Parameter typeEF = eleType.LookupParameter("Identity_Classification_Uniclass 2015_Ef");
            Parameter typeEFcode = eleType.LookupParameter("Identity_Classification_Uniclass 2015_Ef_Code");

            Parameter typePr = eleType.LookupParameter("Identity_Classification_Uniclass 2015_Pr");
            Parameter typePrcode = eleType.LookupParameter("Identity_Classification_Uniclass 2015_Pr_Code");

            Parameter eleEf = element.LookupParameter("RLX_ClassificationUniclassEF_Description");
            Parameter eleEfcode = element.LookupParameter("RLX_ClassificationUniclassEF_Number");

            Parameter elePr = element.LookupParameter("RLX_ClassificationUniclassPr_Description");
            Parameter elePrcode = element.LookupParameter("RLX_ClassificationUniclassPr_Number");

            if (typeEF != null && typeEF.HasValue)
                eleEf?.Set(typeEF?.AsValueString().Split(':')[1].Trim());

            if (typeEFcode != null && typeEFcode.HasValue)
            eleEfcode?.Set(typeEFcode?.AsValueString());

            if (typePr != null && typePr.HasValue)
                    elePr.Set(typePr?.AsValueString()?.Split(':')[1]?.Trim());


            if (typePrcode != null && typePrcode.HasValue)
            elePrcode.Set(typePrcode?.AsValueString());

            //If Uniclass System is Empty, fill it with the type value or instance value:
            Parameter eleSsDescr = element.LookupParameter("RLX_ClassificationUniclassSs_Description");
            Parameter eleSsCode = element.LookupParameter("RLX_ClassificationUniclassSs_Number");

            if (eleSsDescr.AsString() == string.Empty)
            {
                Parameter instanceSsDescr = element.LookupParameter("Identity_Classification_Uniclass 2015_Ss");

                if (instanceSsDescr != null && instanceSsDescr.HasValue)
                {
                    eleSsDescr.Set(instanceSsDescr.AsValueString().Split(':')[1].Trim());
                }

            }
            
            if (eleSsCode.AsString() == string.Empty)
            {
                Parameter instanceSsCode = element.LookupParameter("Identity_Classification_Uniclass 2015_Ss_Code");
                if (instanceSsCode != null && instanceSsCode.HasValue)
                {
                eleSsCode.Set(instanceSsCode?.AsValueString());

                }

            }
        }


        public static string ConvertToSentenceCase(string text)
        {
            // Use regex to split sentences by ., !, ?
            string pattern = @"([.!?]\s*)";
            string[] sentences = Regex.Split(text, pattern);

            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

            for (int i = 0; i < sentences.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(sentences[i]))
                {
                    sentences[i] = textInfo.ToTitleCase(sentences[i].Trim().ToLower());
                }
            }

            return string.Join("", sentences).Trim();
        }


        public static XYZ ComputeCentroid(List<XYZ> points)
        {
            if (points == null || points.Count == 0)
                throw new ArgumentException("Point list is empty!");

            double xSum = 0, ySum = 0, zSum = 0;
            int count = points.Count;

            foreach (XYZ pt in points)
            {
                xSum += pt.X;
                ySum += pt.Y;
                zSum += pt.Z;
            }

            return new XYZ(xSum / count, ySum / count, zSum / count);
        }

        public static int ClosestIndex(List<XYZ> points, XYZ target)
        {
            double minDistance = double.MaxValue;
            int closestIndex = -1;

            for (int i = 0; i < points.Count; i++)
            {
                double distance = points[i].DistanceTo(target);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }


        public static string LocationforDescription()
        {
            //Silvertown Bld:
            //return "Newham Portal Building";
            //Silvertown Services:
            return "Newham Services Building";
        }



        public static bool DoElementsHaveSameParameterValues(List<Element> elements, List<string> paramsToExport)
        {
            if (elements == null || elements.Count == 0) return false;

            // Get the first element as the reference
            Element firstElement = elements[0];

            // Dictionary to store the parameter values of the first element
            Dictionary<string, string> referenceValues = new Dictionary<string, string>();

            foreach (string paramName in paramsToExport)
            {
                Parameter param = firstElement.LookupParameter(paramName);
                if (param != null)
                {
                    referenceValues[paramName] = param.AsString() ?? param.AsValueString() ?? "";
                }
                else
                {
                    referenceValues[paramName] = ""; // Treat missing parameters as empty
                }
            }

            // Compare other elements against the first one
            foreach (Element element in elements.Skip(1))
            {
                foreach (string paramName in paramsToExport)
                {
                    Parameter param = element.LookupParameter(paramName);
                    string currentValue = param != null ? param.AsString() ?? param.AsValueString() ?? "" : "";

                    // Compare values
                    if (referenceValues[paramName] != currentValue)
                    {
                        return false; // Mismatch found
                    }
                }
            }

            return true; // All elements have the same parameter values
        }


        public static ElementMulticategoryFilter RLXcatFilter()
        {

            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>
            {
                BuiltInCategory.OST_CableTray,
                BuiltInCategory.OST_CableTrayFitting,
                BuiltInCategory.OST_Conduit,
                BuiltInCategory.OST_ConduitFitting,
                BuiltInCategory.OST_DataDevices,
                BuiltInCategory.OST_DuctAccessory,
                BuiltInCategory.OST_DuctCurves,
                BuiltInCategory.OST_DuctFitting,
                BuiltInCategory.OST_DuctTerminal,
                BuiltInCategory.OST_ElectricalEquipment,
                BuiltInCategory.OST_ElectricalFixtures,
                BuiltInCategory.OST_FireAlarmDevices,
                BuiltInCategory.OST_GenericModel,
                BuiltInCategory.OST_LightingDevices,
                BuiltInCategory.OST_LightingFixtures,
                BuiltInCategory.OST_MechanicalEquipment,
                BuiltInCategory.OST_PipeAccessory,
                BuiltInCategory.OST_PipeCurves,
                BuiltInCategory.OST_PipeFitting,
                BuiltInCategory.OST_PlumbingFixtures,
                BuiltInCategory.OST_SecurityDevices,
                BuiltInCategory.OST_Sprinklers,
                BuiltInCategory.OST_TelephoneDevices
            };










            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);

            return filter1 ;
        }

        public static ElementMulticategoryFilter RLXcatFilterAccessories()
        {

            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>
            {
                BuiltInCategory.OST_GenericModel,
                BuiltInCategory.OST_DuctTerminal,
                BuiltInCategory.OST_DataDevices,
            BuiltInCategory.OST_Sprinklers,
            BuiltInCategory.OST_MechanicalEquipment,
            BuiltInCategory.OST_DuctAccessory,
            BuiltInCategory.OST_PipeAccessory,
            BuiltInCategory.OST_FireAlarmDevices,
            BuiltInCategory.OST_LightingFixtures,
            BuiltInCategory.OST_LightingDevices,
            BuiltInCategory.OST_ElectricalFixtures,
            BuiltInCategory.OST_ElectricalEquipment,
            BuiltInCategory.OST_TelephoneDevices,
            BuiltInCategory.OST_SecurityDevices,
            BuiltInCategory.OST_PlumbingFixtures,
            BuiltInCategory.OST_CableTrayFitting,


            };

            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);

            return filter1;
        }


        public static XYZ GetStartPoint(Document doc, List<XYZ> points)
        {
            // Check if there are enough points to form a curve (at least 3 points for a basic NURBS curve)
            if (points.Count < 3)
            {
                throw new ArgumentException("At least three points are required to create a NURBS curve.");
            }

            // Convert the points from XYZ (Revit) to Point3d (Rhino)
            List<RG.Point3d> rhinoPoints = new List<RG.Point3d>();
            foreach (var pt in points)
            {
                rhinoPoints.Add(new RG.Point3d(pt.X, pt.Y, pt.Z));
            }

            //RG.Point3d startingPoint = rhinoPoints.First();

            //var sortedPoints = rhinoPoints.OrderBy(p => p.DistanceTo(startingPoint)).ToList();

            RG.Curve crv = RG.Curve.CreateControlPointCurve(rhinoPoints);

            

            RG.Point3d minPoint = crv.PointAtStart;
            //// Create a NURBS curve through the points using RhinoCommon
            //var nurbsCurve = RG.NurbsCurve.CreateControlPointCurve(rhinoPoints, 1);

            //List<RG.Point3d> pts = new List<RG.Point3d>() { nurbsCurve.PointAtStart , nurbsCurve.PointAtEnd };

            //RG.Point3d minPoint = pts.OrderBy(p => p.X).ThenBy(p => p.Y).ThenBy(p => p.Z).First();

            //RG.Point3d minPoint = sortedPoints.First();

            return new XYZ(minPoint.X, minPoint.Y, minPoint.Z);
        }

        /// <summary>
        /// Return a string for a real number
        /// formatted to two decimal places.
        /// </summary>
        public static string RealString(double a)
        {
            return a.ToString("0.##");
        }

        /// <summary>
        /// Return a string representation in degrees
        /// for an angle given in radians.
        /// </summary>
        public static string AngleString(double angle)
        {
            return RealString(angle * 180 / Math.PI) + " degrees";
        }


        /// <summary>
        /// Return a string for an XYZ 
        /// point or vector with its coordinates
        /// formatted to two decimal places.
        /// </summary>
        public static string PointString(XYZ p)
        {
            return string.Format("({0},{1},{2})",
              RealString(p.X),
              RealString(p.Y),
              RealString(p.Z));
        }


        /// <summary>
        /// Predicate to report whether the given curve 
        /// type is supported by this utility class.
        /// </summary>
        /// <param name="curve">The curve.</param>
        /// <returns>True if the curve type is supported, 
        /// false otherwise.</returns>
        public static bool IsSupported(
          Curve curve)
        {
            return curve is Line || curve is Arc;
        }

        /// <summary>
        /// Create a new curve with the same 
        /// geometry in the reverse direction.
        /// </summary>
        /// <param name="orig">The original curve.</param>
        /// <returns>The reversed curve.</returns>
        /// <throws cref="NotImplementedException">If the 
        /// curve type is not supported by this utility.</throws>
        static Curve CreateReversedCurve(
          Autodesk.Revit.Creation.Application creapp,
          Curve orig)
        {
            if (!IsSupported(orig))
            {
                throw new NotImplementedException(
                  "CreateReversedCurve for type "
                  + orig.GetType().Name);
            }

            if (orig is Line)
            {
                return Line.CreateBound(
                  orig.GetEndPoint(1),
                  orig.GetEndPoint(0));
            }
            else if (orig is Arc)
            {
                return Arc.Create(orig.GetEndPoint(1),
                  orig.GetEndPoint(0),
                  orig.Evaluate(0.5, true));
            }
            else
            {
                throw new Exception(
                  "CreateReversedCurve - Unreachable");
            }
        }



        /// <summary>
        /// Sort a list of curves to make them correctly 
        /// ordered and oriented to form a closed loop.
        /// </summary>
        public static void SortCurvesContiguous(
          Autodesk.Revit.Creation.Application creapp,
          IList<Curve> curves,
          bool debug_output)
        {

            const double _inch = 1.0 / 12.0;
            const double _sixteenth = _inch / 16.0;

            int n = curves.Count;

            // Walk through each curve (after the first) 
            // to match up the curves in order

            for (int i = 0; i < n; ++i)
            {
                Curve curve = curves[i];
                XYZ endPoint = curve.GetEndPoint(1);

                if (debug_output)
                {
                    Debug.Print("{0} endPoint {1}", i,
                      PointString(endPoint));
                }

                XYZ p;

                // Find curve with start point = end point

                bool found = (i + 1 >= n);

                for (int j = i + 1; j < n; ++j)
                {
                    p = curves[j].GetEndPoint(0);

                    // If there is a match end->start, 
                    // this is the next curve

                    if (_sixteenth > p.DistanceTo(endPoint))
                    {
                        if (debug_output)
                        {
                            Debug.Print(
                              "{0} start point, swap with {1}",
                              j, i + 1);
                        }

                        if (i + 1 != j)
                        {
                            Curve tmp = curves[i + 1];
                            curves[i + 1] = curves[j];
                            curves[j] = tmp;
                        }
                        found = true;
                        break;
                    }

                    p = curves[j].GetEndPoint(1);

                    // If there is a match end->end, 
                    // reverse the next curve

                    if (_sixteenth > p.DistanceTo(endPoint))
                    {
                        if (i + 1 == j)
                        {
                            if (debug_output)
                            {
                                Debug.Print(
                                  "{0} end point, reverse {1}",
                                  j, i + 1);
                            }

                            curves[i + 1] = CreateReversedCurve(
                              creapp, curves[j]);
                        }
                        else
                        {
                            if (debug_output)
                            {
                                Debug.Print(
                                  "{0} end point, swap with reverse {1}",
                                  j, i + 1);
                            }

                            Curve tmp = curves[i + 1];
                            curves[i + 1] = CreateReversedCurve(
                              creapp, curves[j]);
                            curves[j] = tmp;
                        }
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    throw new Exception("SortCurvesContiguous:"
                      + " non-contiguous input curves");
                }
            }
        }
        public static XYZ GetElementCentroid(Element element)
        {
            BoundingBoxXYZ bbox = element.get_BoundingBox(null);
            if (bbox == null)
                return null;

            XYZ min = bbox.Min;
            XYZ max = bbox.Max;

            return new XYZ((min.X + max.X) / 2, (min.Y + max.Y) / 2, (min.Z + max.Z) / 2);
        }

        public static XYZ GetElementCentroidTopZ(Element element)
        {
            BoundingBoxXYZ bbox = element.get_BoundingBox(null);
            if (bbox == null)
                return null;

            XYZ min = bbox.Min;
            XYZ max = bbox.Max;

            return new XYZ((min.X + max.X) / 2, (min.Y + max.Y) / 2, max.Z);
        }

        public static XYZ CalculateCentroidOfElements(IEnumerable<Element> elements)
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


        public static void FillXYZParam(IEnumerable<Element> elements, double x, double y, double z)
        {
            
            foreach (Element element in elements)
            {
                Parameter _x = element.LookupParameter("RLX_CoordinatesX");
                _x.Set(Math.Round(x,3).ToString());
                Parameter _y = element.LookupParameter("RLX_CoordinatesY");
                _y.Set(Math.Round(y, 3).ToString());
                Parameter _z = element.LookupParameter("RLX_CoordinatesZ");
                _z.Set(Math.Round(z, 3).ToString());
            }


        }

        public static void FillXYZParam(Element element, double x, double y, double z)
        {

                Parameter _x = element.LookupParameter("RLX_CoordinatesX");
                _x.Set(Math.Round(x, 3).ToString());
                Parameter _y = element.LookupParameter("RLX_CoordinatesY");
                _y.Set(Math.Round(y, 3).ToString());
                Parameter _z = element.LookupParameter("RLX_CoordinatesZ");
                _z.Set(Math.Round(z, 3).ToString());
        }

        public static bool DoBoundingBoxesIntersect(BoundingBoxXYZ box1, BoundingBoxXYZ box2)
        {
            // Check if the bounding boxes intersect along the X axis
            bool intersectX = box1.Min.X <= box2.Max.X && box1.Max.X >= box2.Min.X;

            // Check if the bounding boxes intersect along the Y axis
            bool intersectY = box1.Min.Y <= box2.Max.Y && box1.Max.Y >= box2.Min.Y;

            // Check if the bounding boxes intersect along the Z axis
            bool intersectZ = box1.Min.Z <= box2.Max.Z && box1.Max.Z >= box2.Min.Z;

            // The bounding boxes intersect if they overlap in all three dimensions
            return intersectX && intersectY && intersectZ;
        }

        public static List<List<T>> ChunkList<T>(List<T> list, int chunkSize)
        {
            var chunks = new List<List<T>>();

            for (int i = 0; i < list.Count; i += chunkSize)
            {
                chunks.Add(list.Skip(i).Take(chunkSize).ToList());
            }

            return chunks;
        }
        public static double FindChainage(XYZ point, RG.Polyline alignment, double startCh)
        {
            RG.Point3d closestPt = alignment.ClosestPoint(Helpers.RevitToRhinoPt(point));

            double rhinoPar = alignment.ClosestParameter(closestPt);
            RG.Curve[] plcurve = alignment.ToPolylineCurve().Split(rhinoPar);

            RG.Polyline splitAlignment = null;

            plcurve[0].TryGetPolyline(out splitAlignment);

            return splitAlignment.Length * 0.3048 + startCh;
        }

        public static RG.Point3d RevitToRhinoPt (XYZ pt)
        {
            return new RG.Point3d(pt.X, pt.Y, pt.Z);
        }

        public static XYZ RhinoToRevitPt(RG.Point3d pt)
        {
            return new XYZ(pt.X, pt.Y, pt.Z);
        }



        public static List<XYZ> SortPoints(List<XYZ> points)
        {
            // Sort the points using a custom comparison
            points.Sort((p1, p2) =>
            {
                int result = p1.X.CompareTo(p2.X); // Compare by X coordinate

                if (result == 0)
                {
                    result = p1.Y.CompareTo(p2.Y); // Compare by Y coordinate if X is equal
                }

                return result;
            });

            return points;

        }
        public static int FindClosestPointIndex(XYZ targetPoint, List<XYZ> points)
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

            return points.IndexOf(closestPoint);
        }

        public static bool IsPointBetweenPoints(XYZ p, XYZ a, XYZ b)
        {

            // Check if the point p is between a and b
            double dotProduct = (p - a).DotProduct(b - a);
            if (dotProduct < 0)
            {
                return false; // The point is behind the start point
            }

            

            double squaredLengthAB = Math.Pow( (b - a).GetLength(), 2);
            if (dotProduct > squaredLengthAB)
            {
                return false; // The point is beyond the end point
            }

            return true; // The point is between the two points
        }

        public static DetailArc CreateCircle(  Document doc,  XYZ location,  double radius)
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




    }


    // Custom equality comparer for XYZ points
    public class XYZEqualityComparer : IEqualityComparer<XYZ>
    {
        public bool Equals(XYZ x, XYZ y)
        {
            if (x == null || y == null)
                return false;

            // Check if both points are approximately equal
            return x.IsAlmostEqualTo(y);
        }

        public int GetHashCode(XYZ obj)
        {
            if (obj == null)
                return 0;

            // Compute hash code based on coordinates
            return obj.X.GetHashCode() ^ obj.Y.GetHashCode() ^ obj.Z.GetHashCode();
        }
    }
}
