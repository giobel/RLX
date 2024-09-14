﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RG = Rhino.Geometry;

namespace RLX
{
    internal class Helpers
    {

        public static XYZ GetElementCentroid(Element element)
        {
            BoundingBoxXYZ bbox = element.get_BoundingBox(null);
            if (bbox == null)
                return null;

            XYZ min = bbox.Min;
            XYZ max = bbox.Max;

            return new XYZ((min.X + max.X) / 2, (min.Y + max.Y) / 2, (min.Z + max.Z) / 2);
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
                _x.Set(x.ToString());
                Parameter _y = element.LookupParameter("RLX_CoordinatesY");
                _y.Set(y.ToString());
                Parameter _z = element.LookupParameter("RLX_CoordinatesZ");
                _z.Set(z.ToString());
            }


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
