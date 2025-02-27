using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLX
{
    using System;
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    public class SmallestEnclosingCircle
    {
        public static (XYZ center, double radius) GetCircumscribedCircle(List<XYZ> points)
        {
            if (points.Count == 0) throw new Exception("No points provided.");
            if (points.Count == 1) return (points[0], 0);
            if (points.Count == 2) return TwoPointCircle(points[0], points[1]);

            return WelzlAlgorithm(points, new List<XYZ>());
        }

        private static (XYZ center, double radius) TwoPointCircle(XYZ A, XYZ B)
        {
            XYZ center = (A + B) / 2;
            double radius = A.DistanceTo(B) / 2;
            return (center, radius);
        }

        private static (XYZ center, double radius) ThreePointCircle(XYZ A, XYZ B, XYZ C)
        {
            // Midpoints of two sides
            XYZ midAB = (A + B) / 2;
            XYZ midBC = (B + C) / 2;

            // Normal vectors for perpendicular bisectors
            XYZ dirAB = (B - A).Normalize();
            XYZ dirBC = (C - B).Normalize();
            XYZ normalAB = new XYZ(-dirAB.Y, dirAB.X, 0);
            XYZ normalBC = new XYZ(-dirBC.Y, dirBC.X, 0);

            // Find intersection of two bisectors
            XYZ center = LineIntersection(midAB, normalAB, midBC, normalBC);

            // Compute radius
            double radius = center.DistanceTo(A);

            return (center, radius);
        }

        private static XYZ LineIntersection(XYZ p1, XYZ d1, XYZ p2, XYZ d2)
        {
            double det = d1.X * d2.Y - d1.Y * d2.X;
            if (Math.Abs(det) < 1e-6) throw new Exception("Points are collinear, no unique circle.");

            double t = ((p2.X - p1.X) * d2.Y - (p2.Y - p1.Y) * d2.X) / det;
            return new XYZ(p1.X + t * d1.X, p1.Y + t * d1.Y, 0);
        }

        private static (XYZ center, double radius) WelzlAlgorithm(List<XYZ> points, List<XYZ> boundary)
        {
            if (points.Count == 0 || boundary.Count == 3)
            {
                return ComputeBaseCircle(boundary);
            }

            XYZ p = points[points.Count - 1];
            points.RemoveAt(points.Count - 1);

            (XYZ center, double radius) = WelzlAlgorithm(points, boundary);

            if (p.DistanceTo(center) <= radius)
            {
                return (center, radius);
            }

            boundary.Add(p);
            return WelzlAlgorithm(points, boundary);
        }

        private static (XYZ center, double radius) ComputeBaseCircle(List<XYZ> boundary)
        {
            if (boundary.Count == 0) return (new XYZ(0, 0, 0), 0);
            if (boundary.Count == 1) return (boundary[0], 0);
            if (boundary.Count == 2) return TwoPointCircle(boundary[0], boundary[1]);
            return ThreePointCircle(boundary[0], boundary[1], boundary[2]);
        }

        public static void ShowCircumscribedCircle(UIApplication uiapp)
        {
            List<XYZ> points = new List<XYZ>
        {
            new XYZ(2, 3, 0),
            new XYZ(5, 7, 0),
            new XYZ(8, 2, 0),
            new XYZ(4, 6, 0),
            new XYZ(7, 1, 0)
        };

            (XYZ center, double radius) = GetCircumscribedCircle(points);

            TaskDialog.Show("Circumscribed Circle",
                $"Center: {center}\nRadius: {radius:F3}");
        }
    }

}
