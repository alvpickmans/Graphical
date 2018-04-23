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
using Graphical.Base;

namespace Graphical.Geometry
{
    
    /// <summary>
    /// Static class extending Point functionality
    /// </summary>
    public static class Point
    {
        //Vector-plane intersection https://math.stackexchange.com/questions/100439/determine-where-a-vector-will-intersect-a-plane

        #region Constants
        const int rounding = 10 * 10;
        const double rounding2 = 10.0 * 10; 
        #endregion

        #region Public Methods
        /// <summary>
        /// Order the list of points by the radian angle from a centre point. If angle is equal, closer to centre will be first.
        /// </summary>
        /// <param name="centre">Centre point</param>
        /// <param name="points">Points to order</param>
        /// <returns name="points">Ordered points</returns>
        public static List<DSPoint> OrderByRadianAndDistance(
            [DefaultArgument("Autodesk.DesignScript.Geometry.Point.ByCoordinates(0,0,0);")]DSPoint centre,
            List<DSPoint> points)
        {
            List<DSPoint> ordered = points.OrderBy(p => RadAngle(centre, p)).ThenBy(p => centre.DistanceTo(p)).ToList();
            return ordered;
        }

        /// <summary>
        /// Returns the minimum point from a list of points. Minimum is evaulated by the point with minimum Y, then X and finally Z coordinate.
        /// </summary>
        /// <param name="points">List of points</param>
        /// <returns name="minPoint">Minimum Point</returns>
        public static DSPoint MinimumPoint(List<DSPoint> points)
        {
            //TODO: Implement a better way of selecting the minimum point.
            return points.OrderBy(p => p.Y).ThenBy(p => p.X).ThenBy(p => p.Z).ToList().First();
        }


        /// <summary>
        /// Radian angle of an arc
        /// </summary>
        /// <param name="centre"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static double ArcRadAngle(DSPoint centre, DSPoint start, DSPoint end)
        {
            double a = Math.Pow((end.X - centre.X), 2) + Math.Pow((end.Y - centre.Y), 2);
            double b = Math.Pow((end.X - start.X), 2) + Math.Pow((end.Y - start.Y), 2);
            double c = Math.Pow((centre.X - start.X), 2) + Math.Pow((centre.Y - start.Y), 2);
            return Math.Acos((a + c - b) / (2 * Math.Sqrt(a) * Math.Sqrt(c)));

        } 
        #endregion

        #region Internal Methods

        /// <summary>
        /// Projects a Point to a line by a given vector direction.
        /// </summary>
        /// <param name="point">Point to project</param>
        /// <param name="vector">Vector direction</param>
        /// <param name="line">Line where the point is projected</param>
        /// <returns name="projection">Point projected</returns>
        internal static DSPoint ProjectToLine(this DSPoint point, DSVector vector, DSLine line)
        {
            /*
			 * Segment MN = M(x1, y1, z1), N(x2, y2, z2);
			 * Vector: Origin(o1, o2, o3), Direction(d1, d2, d3)
			 */
            DSVector lineVector = line.Direction.Normalized();
            DSVector d = vector.Normalized();

            //if parallel, dot product equals to 1, so no intersection)
            double dot = d.Dot(lineVector);
            if (Math.Abs(dot) == 1) { return null; };

            /*
			 *As they are not parallel, first let´s solve the problem in 2D as if their projections
			 * don´t intersect, they won´t intersect either.
			 */
            DSPoint O = point; ;//Origin of vector
            DSPoint M = line.StartPoint;
            DSPoint N = line.EndPoint;
            double x, y, z;
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
            }
            else
            {
                t = (slope * O.X - O.Y + M.Y - slope * M.X) / (d.Y - slope * d.X);
            }

            x = O.X + d.X * t;
            y = O.Y + d.Y * t;
            z = O.Y + d.Z * t;
            return DSPoint.ByCoordinates(x, y, z);
        }

        /// <summary>
        /// Radian angle from a centre to another point
        /// </summary>
        /// <param name="centre"></param>
        /// <param name="point"></param>
        /// <returns name="rad">Radians</returns>
        internal static double RadAngle(DSPoint centre, DSPoint point)
        {
            //Rad angles http://math.rice.edu/~pcmi/sphere/drg_txt.html
            double dx = point.X - centre.X;
            double dy = point.Y - centre.Y;
            //TODO: Implement Z angle? that would becom UV coordinates.
            //double dz = vertex.point.Z - centre.point.Z;

            if (dx == 0 && dy == 0) { return 0; }

            if (dx == 0)// both vertices on Y axis
            {
                if (dy < 0)//vertex below X axis
                {
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


        internal static bool OnLine(DSPoint point, DSLine line)
        {
            return line.DoesIntersect(point);
        }

        internal static bool OnLine(DSPoint start, DSPoint middle, DSPoint end)
        {
            using (DSLine line = DSLine.ByStartPointEndPoint(start, end))
            {
                return line.DoesIntersect(middle);
            }
        }

        /// <summary>
        /// Assuming point is colinear to start and end points, determines if point is in between.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="point"></param>
        /// <param name="end"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        internal static bool OnLineProjection(DSPoint start, DSPoint point, DSPoint end, string plane = "xy")
        {
            double x = point.X, y = point.Y, z = point.Z;
            double sX = start.X, sY = start.Y, sZ = start.Z;
            double eX = end.X, eY = end.Y, eZ = end.Z;
            switch (plane)
            {
                case "xy":
                    return x <= Math.Max(sX, eX) && x >= Math.Min(sX, eX) &&
                        y <= Math.Max(sY, eY) && y >= Math.Min(sY, eY);
                case "xz":
                    return x <= Math.Max(sX, eX) && x >= Math.Min(sX, eX) &&
                        z <= Math.Max(sZ, eZ) && z >= Math.Min(sZ, eZ);
                case "yz":
                    return y <= Math.Max(sY, eY) && y >= Math.Min(sY, eY) &&
                        z <= Math.Max(sZ, eZ) && z >= Math.Min(sZ, eZ);
                default:
                    throw new Exception("Plane not defined");
            }
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
        /// -1 if orientation is clock wise.</returns>
         public static int Orientation(DSPoint p1, DSPoint p2, DSPoint p3, string plane = "xy")
        {
            // See https://www.geeksforgeeks.org/orientation-3-ordered-points/
            // for details of below formula.
            double value = 0;
            switch (plane)
            {
                case "xy":
                    value = (p2.X - p1.X) * (p3.Y - p2.Y) - (p2.Y - p1.Y) * (p3.X - p2.X);
                    break;
                case "xz":
                    value = (p2.X - p1.X) * (p3.Z - p2.Z) - (p2.Z - p1.Z) * (p3.X - p2.X);
                    break;
                case "yz":
                    value = (p2.Y - p1.Y) * (p3.Z - p2.Z) - (p2.Z - p1.Z) * (p3.Y - p2.Y);
                    break;
                default:
                    throw new Exception("Plane not defined");
            }
            //Rounding due to floating point error.
            value = Math.Round(value, 6);
            if (value == 0) { return 0; } //Points are colinear
            int result = (value > 0) ? 1 : -1;

            return (value > 0) ? 1 : -1; //Counter clock or clock wise

        } 
        #endregion

    }
}
