#region namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using Graphical.Geometry;
using DSPoint = Autodesk.DesignScript.Geometry.Point;
using System.Globalization; 
#endregion

namespace Graphical.Base
{
    /// <summary>
    /// Representation of vertex points on a graph.
    /// </summary>
    [IsVisibleInDynamoLibrary(false)]
    public class gVertex : IGraphicItem, IDisposable, ICloneable, IEquatable<gVertex>
    {

        #region Constants
        const int rounding = 10 * 10;
        const double rounding2 = 10.0 * 10;
        #endregion
        
        //TODO: Reorganize methods 
        #region Variables
        internal DSPoint point { get { return DSPoint.ByCoordinates(X, Y, Z); } }
        internal int polygonId { get; set; }

        internal double X { get; private set; }
        internal double Y { get; private set; }
        internal double Z { get; private set; }

        #endregion

        #region Constructors
        internal gVertex(double x, double y, double z = 0, int pId = -1)
        {
            polygonId = pId;
            X = Math.Round(x, 6);
            Y = Math.Round(y, 6);
            Z = Math.Round(z, 6);
        }

        /// <summary>
        /// gEdge constructor method by a give point.
        /// </summary>
        /// <param name="point">Input point</param>
        /// <returns></returns>
        public static gVertex ByPoint(DSPoint point)
        {
            return new gVertex(point.X, point.Y, point.Z);
        }

        /// <summary>
        /// gEdge constructor method by a given set of XYZ coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        internal static gVertex ByCoordinates(double x, double y, double z)
        {
            return new gVertex(x, y, z);
        }

        /// <summary>
        /// Returns the vertex in between two vertices.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns name="midVertex"></returns>
        internal static gVertex MidVertex ( gVertex v1, gVertex v2)
        {
            double x = (v1.X + v2.X) / 2, y = (v1.Y + v2.Y) / 2, z = (v1.Z + v2.Z) / 2;
            return new gVertex(x, y, z);
        }
        #endregion

        internal static List<gVertex> OrderByRadianAndDistance (List<gVertex> vertices, gVertex centre = null)
        {
            if(centre == null) { centre = gVertex.MinimumVertex(vertices); }
            return vertices.OrderBy(v => RadAngle(centre, v)).ThenBy(v => centre.DistanceTo(v)).ToList();
            
        }

        internal static int Orientation(gVertex v1, gVertex p2, gVertex p3, string plane = "xy")
        {
            // See https://www.geeksforgeeks.org/orientation-3-ordered-points/
            // for details of below formula.
            double value = 0;
            switch (plane)
            {
                case "xy":
                    value = (p2.X - v1.X) * (p3.Y - p2.Y) - (p2.Y - v1.Y) * (p3.X - p2.X);
                    break;
                case "xz":
                    value = (p2.X - v1.X) * (p3.Z - p2.Z) - (p2.Z - v1.Z) * (p3.X - p2.X);
                    break;
                case "yz":
                    value = (p2.Y - v1.Y) * (p3.Z - p2.Z) - (p2.Z - v1.Z) * (p3.Y - p2.Y);
                    break;
                default:
                    throw new Exception("Plane not defined");
            }
            //Rounding due to floating point error.
            value = Math.Round(value, 6);
            if (value == 0) { return 0; } //Points are colinear

            return (value > 0) ? 1 : -1; //Counter clock or clock wise
        }

        internal static double RadAngle(gVertex centre, gVertex vertex)
        {
            using (DSPoint p1 = centre.point)
            using(DSPoint p2 = vertex.point)
            {
                return Graphical.Geometry.Point.RadAngle(p1, p2);
            }
        }

        internal static double ArcRadAngle (gVertex centre, gVertex start, gVertex end)
        {
            using (DSPoint c = centre.point)
            using (DSPoint s = start.point)
            using (DSPoint e = end.point)
            {
                return Graphical.Geometry.Point.ArcRadAngle(c, s, e);
            }
        }

        internal static gVertex MinimumVertex(List<gVertex> vertices)
        {
            return vertices.OrderBy(v => v.Y).ThenBy(v => v.X).ThenBy(v => v.Z).ToList().First();
        }

        internal double DistanceTo(gVertex vertex)
        {
            return Math.Sqrt(Math.Pow(vertex.X - X, 2) + Math.Pow(vertex.Y - Y, 2) + Math.Pow(vertex.Z - Z, 2));
        }

        internal double DistanceTo(gEdge edge)
        {
            using (DSPoint p = this.point)
            using (Line line = edge.LineGeometry())
            {
                return p.DistanceTo(line);
            }
        }

        internal double DistanceTo(Autodesk.DesignScript.Geometry.Geometry obj)
        {
            using (DSPoint p = this.point)
            {
                return p.DistanceTo(obj); 
            }
        }

        internal DSPoint GetProjectionOnPlane(string plane = "xy")
        {
            switch (plane)
            {
                case "xy":
                    return DSPoint.ByCoordinates(X, Y, 0);
                case "xz":
                    return DSPoint.ByCoordinates(X, 0, Z);
                case "yz":
                    return DSPoint.ByCoordinates(0, Y, Z);
                default:
                    return null;
            }
        }

        internal static bool OnLine(gVertex vertex, Line line)
        {
            using (DSPoint p = vertex.point)
            {
                return p.DoesIntersect(line);
            }
        }

        internal static bool OnLine(gVertex start, gVertex vertex, gVertex end)
        {
            using (DSPoint startPt = start.point)
            using (DSPoint p = vertex.point)
            using (DSPoint endPt = end.point)
            using (Line line = Line.ByStartPointEndPoint(startPt, endPt))
            {
                return p.DoesIntersect(line);
            }
        }


        #region Override Methods
        //TODO: Improve overriding equality methods as per http://www.loganfranken.com/blog/687/overriding-equals-in-c-part-1/

        /// <summary>
        /// Override of Equals method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Boolean</returns>
        public bool Equals(gVertex obj)
        {
            if (obj == null) { return false; }
            
            return this.X == obj.X && this.Y == obj.Y && this.Z == obj.Z;
        }

        /// <summary>
        /// Override of GetHashCode method
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

       

        /// <summary>
        /// Override of ToStringMethod
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            NumberFormatInfo inf = new NumberFormatInfo();
            inf.NumberDecimalSeparator = ".";
            return string.Format("gVertex(X = {0}, Y = {1}, Z = {2})", X.ToString("0.000", inf), Y.ToString("0.000", inf), Z.ToString("0.000", inf));
        }

        /// <summary>
        /// Customizing the render of gVertex
        /// </summary>
        /// <param name="package"></param>
        /// <param name="parameters"></param>
        [IsVisibleInDynamoLibrary(false)]
        public void Tessellate(IRenderPackage package, TessellationParameters parameters)
        {
            package.AddPointVertex(X, Y, Z);
            package.AddPointVertexColor(255, 0, 0, 255);
        }

        /// <summary>
        /// Implementation of Dispose method
        /// </summary>
        public void Dispose()
        {
            ((IDisposable)point).Dispose();
        }

        /// <summary>
        /// Implementation of Clone method
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        public object Clone()
        {
            gVertex newVertex = new gVertex(this.X, this.Y, this.Z, this.polygonId);

            return newVertex;
        }

        
        #endregion

    }
}
