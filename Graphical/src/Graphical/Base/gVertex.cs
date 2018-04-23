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

        //HACK: Avoid using point as this creates unhandled geometry and consumes memory.
        //TODO: Reorganize methods 
        #region Variables
        internal DSPoint point { get { return DSPoint.ByCoordinates(X, Y, Z); } }
        internal int polygonId { get; set; }
        //public gEdge[] AdjacentEdges { get; private set; }

        internal double X { get; private set; }
        internal double Y { get; private set; }
        internal double Z { get; private set; }

        #endregion

        #region Constructors
        internal gVertex(double x, double y, double z = 0, int pId = -1)
        {
            //point = DSPoint.ByCoordinates(x, y, z);
            polygonId = pId;
            X = ((int)(x * rounding)) / rounding2;
            Y = ((int)(y * rounding)) / rounding2;
            Z = ((int)(z * rounding)) / rounding2;
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

        internal static int Orientation(gVertex v1, gVertex v2, gVertex v3, string plane = "xy")
        {
            using (DSPoint p1 = v1.point)
            using (DSPoint p2 = v2.point)
            using (DSPoint p3 = v3.point)
            {
                return Graphical.Geometry.Point.Orientation(p1, p2, p3, plane);
            }
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

        
        internal double DistanceTo(object obj)
        {
            if(GetType() == obj.GetType())
            {
                gVertex v = (gVertex)obj;
                using (DSPoint p1 = this.point)
                using (DSPoint p2 = v.point)
                {
                    return p1.DistanceTo(p2);
                }
            }else if(obj.GetType() == typeof(gEdge))
            {
                gEdge e = (gEdge)obj;
                using (DSPoint p = this.point)
                using (Line line = e.LineGeometry())
                {
                    return p.DistanceTo(line);
                }
            }
            else
            {
                using (DSPoint p = this.point)
                {
                    return p.DistanceTo(obj as Autodesk.DesignScript.Geometry.Geometry); 
                }
            }
        }

        internal static int Orientation(gVertex v1, gVertex v2, gVertex v3)
        {
            using (DSPoint p1 = v1.point)
            using (DSPoint p2 = v2.point)
            using (DSPoint p3 = v3.point)
            {
                return Graphical.Geometry.Point.Orientation(p1, p2, p3);
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
