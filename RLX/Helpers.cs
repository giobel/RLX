using System;
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
