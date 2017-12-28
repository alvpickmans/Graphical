#region namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using System.Globalization; 
#endregion

namespace Graphical.Base
{
    /// <summary>
    /// Representation of vertex points on a graph.
    /// </summary>
    public class Vertex : IGraphicItem, IDisposable
    {
        #region Variables
        internal Point point { get; }
        internal int polygonId { get; set; }
        //public Edge[] AdjacentEdges { get; private set; }

        internal double X { get; private set; }
        internal double Y { get; private set; }
        internal double Z { get; private set; }

        #endregion

        #region Constructors
        internal Vertex(double x, double y, double z = 0, int pId = -1)
        {
            point = Point.ByCoordinates(x, y, z);
            polygonId = pId;
            X = x; Y = y; Z = z;
        }

        /// <summary>
        /// Edge constructor method by a give point.
        /// </summary>
        /// <param name="point">Input point</param>
        /// <returns></returns>
        public static Vertex ByPoint(Point point)
        {
            return new Vertex(point.X, point.Y, point.Z);
        }

        /// <summary>
        /// Edge constructor method by a given set of XYZ coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vertex ByCoordinates(double x, double y, double z)
        {
            return new Vertex(x, y, z);
        }

        public static Vertex MidVertex ( Vertex v1, Vertex v2)
        {
            double x = (v1.X + v2.X) / 2, y = (v1.Y + v2.Y) / 2, z = (v1.Z + v2.Z) / 2;
            return new Vertex(x, y, z);
        }
        #endregion


        internal double DistanceTo(object obj)
        {
            if(GetType() == obj.GetType())
            {
                Vertex v = (Vertex)obj;
                return point.DistanceTo(v.point);
            }else if(obj.GetType() == typeof(Edge))
            {
                Edge e = (Edge)obj;
                return DistanceTo(e.LineGeometry);
            }
            else
            {
                return point.DistanceTo(obj as Autodesk.DesignScript.Geometry.Geometry);
            }
        }

        internal static int Orientation(Vertex v1, Vertex v2, Vertex v3)
        {
            return Graphical.Geometry.Point.Orientation(v1.point, v2.point, v3.point);
        }



        #region Override Methods
        //TODO: Improve overriding equality methods as per http://www.loganfranken.com/blog/687/overriding-equals-in-c-part-1/

        /// <summary>
        /// Override of Equals method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) { return false; }

            Vertex v = (Vertex)obj;
            bool eq = point.Equals(v.point);
            bool eq2 = X == v.X && Y == v.Y && Z == v.Z;
            return point.Equals(v.point);
        }

        /// <summary>
        /// Override of GetHashCode method
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return point.GetHashCode();
        }

       

        /// <summary>
        /// Override of ToStringMethod
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            NumberFormatInfo inf = new NumberFormatInfo();
            inf.NumberDecimalSeparator = ".";
            return string.Format("Vertex(X = {0}, Y = {1}, Z = {2})", point.X.ToString("0.000", inf), point.Y.ToString("0.000", inf), point.Z.ToString("0.000", inf));
        }

        /// <summary>
        /// Customizing the render of Vertex
        /// </summary>
        /// <param name="package"></param>
        /// <param name="parameters"></param>
        [IsVisibleInDynamoLibrary(false)]
        public void Tessellate(IRenderPackage package, TessellationParameters parameters)
        {
            package.AddPointVertex(point.X, point.Y, point.Z);
            package.AddPointVertexColor(255, 0, 0, 255);
        }

        public void Dispose()
        {
            ((IDisposable)point).Dispose();
        }
        #endregion

    }
}
