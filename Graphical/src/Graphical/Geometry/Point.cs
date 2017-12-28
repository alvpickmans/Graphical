using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using DSVector = Autodesk.DesignScript.Geometry.Vector;
using DSPoint = Autodesk.DesignScript.Geometry.Point;
using DSLine = Autodesk.DesignScript.Geometry.Line;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

namespace Graphical.Geometry
{
    public static class Point
    {
        //Vector-plane intersection https://math.stackexchange.com/questions/100439/determine-where-a-vector-will-intersect-a-plane

        public static DSPoint ProjectToLine(this DSPoint point, DSVector vector, DSLine line)
        {
            /*
			 * Segment MN = M(x1, y1, z1), N(x2, y2, z2);
			 * Vector: Origin(o1, o2, o3), Direction(d1, d2, d3)
			 */
            DSVector lineVector = line.Direction.Normalized();
            DSVector d = vector.Normalized();

            //if parallel, dot product equals to 1, so no intersection)
            double dot = d.Dot(lineVector);
			if(Math.Abs(dot) == 1) { return null; };

            /*
			 *As they are not parallel, first let´s solve the problem in 2D as if their projections
			 * don´t intersect, they won´t intersect either.
			 */
            DSPoint O = point; ;//Origin of vector
            DSPoint M = line.StartPoint;
            DSPoint N = line.EndPoint;
            double x,  y,  z;
            double t;
            //y = slope*x + b
			// if line is perp to X axis, y = y
            
            double slope = (N.Y - M.Y) / (N.X - M.X);
            // b = y1 + slope*x1
            //line equation: y = slope*x + y1 - slope * x1
            //vector equation: x = o1 + d1*t; y = o2 + d2*t
            //replacing vector equation on line, will return t
            if (Double.IsInfinity(slope))
            {
                t = (M.X - O.X) / d.X;
            }else
            {
                t = (slope * O.X - O.Y + M.Y - slope * M.X) / (d.Y - slope * d.X);
            }
            
            x = O.X + d.X * t;
            y = O.Y + d.Y * t;
            z = O.Y + d.Z * t;
            return DSPoint.ByCoordinates(x,y, z);
        }



        public static List<DSPoint> OrderByRadianAndDistance(
            [DefaultArgumentAttribute("Point.ByCoordinates(0, 0, 0);")]DSPoint centre, List<DSPoint> points)
        {
            List<DSPoint> ordered = points.OrderBy(p => RadAngle(centre, p)).ThenBy(p => centre.DistanceTo(p)).ToList();
            return ordered;
        }

        public static DSPoint MinimumPoint(List<DSPoint> points)
        {
            //TODO: Implement a better way of selecting the minimum point.
            return points.OrderBy(p => p.Y).ThenBy(p => p.X).ThenBy(p => p.Z).ToList().First();
        }

        public static double RadAngle(DSPoint centre, DSPoint point)
        {
            //Rad angles http://math.rice.edu/~pcmi/sphere/drg_txt.html
            double dx = point.X - centre.X;
            double dy = point.Y - centre.Y;
            double rad;
            //TODO: Implement Z angle? that would becom UV coordinates.
            //double dz = vertex.point.Z - centre.point.Z;

            if(dx == 0 && dy == 0) { return 0; }

            if (dx == 0)// both vertices on Y axis
            {
                if (dy < 0)//vertex below X axis
                {
                    rad = (Math.PI * 3 / 2);
                    return (Math.PI * 3 / 2);
                }
                else//vertex above X Axis
                {
                    return Math.PI / 2;
                }
            }
            if (dy == 0)// both vertices on X Axis
            {
                if (dx < 0)// vertex on the left of Y axis
                {
                    return Math.PI;
                }
                else//vertex on the right of Y axis
                {
                    return 0;
                }
            }
            if (dx < 0) { return Math.PI + Math.Atan(dy / dx); }
            if (dy < 0) { return 2 * Math.PI + Math.Atan(dy / dx); }
            return Math.Atan(dy / dx);
        }

        public static double RadiansArcStartCentreEnd(DSPoint centre, DSPoint start, DSPoint end)
        {
            double a = Math.Pow((end.X - start.X), 2) + Math.Pow((end.Y - start.Y), 2);
            double b = Math.Pow((end.X - centre.X), 2) + Math.Pow((end.Y - centre.Y), 2);
            double c = Math.Pow((start.X - centre.X), 2) + Math.Pow((start.Y - centre.Y), 2);
            return Math.Acos((a + c - b)/(2 * Math.Sqrt(a) * Math.Sqrt(c)));

        }

        public static bool OnLine(DSPoint point, DSLine line)
        {
            return line.DoesIntersect(point);
        }

        public static bool OnLine(DSPoint start, DSPoint middle, DSPoint end)
        {
            using(DSLine line = DSLine.ByStartPointEndPoint(start, end))
            {
                return line.DoesIntersect(middle);
            }
        }

        internal static bool OnLineProjection(DSPoint start, DSPoint point, DSPoint end, string plane = "xy")
        {
            bool onLine;
            double x = point.X, y = point.Y, z = point.Z;
            double sX = start.X, sY = start.Y, sZ = start.Z;
            double eX = end.X, eY = end.Y, eZ = end.Z;
            switch (plane)
            {
                case "xy":
                    onLine = x <= Math.Max(sX, eX) && x >= Math.Min(sX, eX) &&
                        y <= Math.Max(sY, eY) && y >= Math.Min(sY, eY);
                    break;
                case "xz":
                    onLine =
                        x <= Math.Max(sX, eX) && x >= Math.Min(sX, eX) &&
                        z <= Math.Max(sZ, eZ) && z >= Math.Min(sZ, eZ);
                    break;
                case "yz":
                    onLine =
                        y <= Math.Max(sY, eY) && y >= Math.Min(sY, eY) &&
                        z <= Math.Max(sZ, eZ) && z >= Math.Min(sZ, eZ);
                    break;
                default:
                    throw new Exception("Plane not defined");
            }
            return onLine;

        }

        /// <summary>
        /// Find the orientation of ordered triplet 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="plane">plane of orientation</param>
        /// <returns name="orientation">0 if points ar colinear.
        /// 1 if orientation is counter clock wise.
        /// -1 if orientation is clock kwise.</returns>
        public static int Orientation(DSPoint p1, DSPoint p2, DSPoint p3, string plane = "xy")
        {
            // See https://www.geeksforgeeks.org/orientation-3-ordered-points/
            // for details of below formula.
            double value;
            switch (plane)
            {
                case "xy":
                    value = (p2.X - p1.X) * (p3.Y - p2.Y) - (p2.Y - p1.Y) * (p3.X - p2.X);
                    break;
                case "xz":
                    value = (p2.X - p1.X) * (p3.Z - p2.Z) - (p2.Z - p1.Z) * (p3.X - p2.X);
                    break;
                case "yz":
                    value = (p2.Y - p1.Y) * (p3.Z - p2.Z) -(p2.Z - p1.Z) * (p3.Y - p2.Y);
                    break;
                default:
                    throw new Exception("Plane not defined");
            }

            if(value == 0) { return 0; } //Points are colinear
            int result = (value > 0) ? 1 : -1;

            return (value > 0) ? 1 : -1; //Counter clock or clock wise

        }

    }
}
