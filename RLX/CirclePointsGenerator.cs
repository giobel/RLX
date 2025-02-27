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

    public class CirclePointsGenerator
    {
        public static List<XYZ> GetPointsOnCircle(XYZ center, double radius, int numPoints)
        {
            List<XYZ> points = new List<XYZ>();
            double angleStep = 2 * Math.PI / numPoints; // Divide full circle into equal steps

            for (int i = 0; i < numPoints; i++)
            {
                double theta = i * angleStep; // Current angle in radians
                double x = center.X + radius * Math.Cos(theta);
                double y = center.Y + radius * Math.Sin(theta);
                points.Add(new XYZ(x, y, center.Z)); // Keep same Z level
            }

            return points;
        }

        public static void ShowCirclePoints(UIApplication uiapp)
        {
            XYZ center = new XYZ(5, 5, 0); // Example center
            double radius = 3; // Example radius
            int numPoints = 8; // Example: 8 points evenly spaced

            List<XYZ> circlePoints = GetPointsOnCircle(center, radius, numPoints);

            string result = "Points on Circle:\n";
            foreach (XYZ p in circlePoints)
            {
                result += $"{p}\n";
            }

            TaskDialog.Show("Circle Points", result);
        }
    }

}
