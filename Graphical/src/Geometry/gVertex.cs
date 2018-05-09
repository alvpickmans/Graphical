#region namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Graphical.Geometry
{
    /// <summary>
    /// Representation of vertex points on a graph.
    /// </summary>
    public class gVertex : gBase, ICloneable, IEquatable<gVertex>
    {
        //TODO: Reorganize methods 
        #region Variables
        internal int polygonId { get; set; }

        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        #endregion

        #region Constructors
        private gVertex(double x, double y, double z = 0, int pId = -1)
        {
            polygonId = pId;
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// gVertex constructor method by a given set of XYZ coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static gVertex ByCoordinates(double x, double y, double z = 0)
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

        public static int Orientation(gVertex v1, gVertex p2, gVertex p3, string plane = "xy")
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
            if (Threshold(value,0)) { return 0; } //Points are colinear

            return (value > 0) ? 1 : -1; //Counter clock or clock wise
        }

        public static double RadAngle(gVertex centre, gVertex vertex)
        {
            //Rad angles http://math.rice.edu/~pcmi/sphere/drg_txt.html
            double dx = vertex.X - centre.X;
            double dy = vertex.Y - centre.Y;
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

        public static double ArcRadAngle (gVertex centre, gVertex start, gVertex end)
        {
            double a = Math.Pow((end.X - centre.X), 2) + Math.Pow((end.Y - centre.Y), 2);
            double b = Math.Pow((end.X - start.X), 2) + Math.Pow((end.Y - start.Y), 2);
            double c = Math.Pow((centre.X - start.X), 2) + Math.Pow((centre.Y - start.Y), 2);
            return Math.Acos((a + c - b) / (2 * Math.Sqrt(a) * Math.Sqrt(c)));
            
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
            // http://mathworld.wolfram.com/Point-LineDistance3-Dimensional.html
            gVector v1 = gVector.ByTwoVertices(this, edge.StartVertex);
            gVector v2 = gVector.ByTwoVertices(this, edge.EndVertex);
            gVector numerator = v1.Cross(v2);
            gVector denominator = gVector.ByTwoVertices(edge.EndVertex, edge.StartVertex);
            return numerator.Length / denominator.Length;
        }
        
        internal gVertex Translate(gVector vector)
        {
            return gVertex.ByCoordinates(this.X + vector.X, this.Y + vector.Y, this.Z + vector.Z);
        }

        internal bool OnEdge(gEdge edge)
        {
            return this.OnEdge(edge.StartVertex, edge.EndVertex);
        }

        public bool OnEdge(gVertex start, gVertex end)
        {
            if(this.Equals(start) || this.Equals(end)) { return true; }
            // https://www.lucidarme.me/check-if-a-point-belongs-on-a-line-segment/
            gVector startEnd = gVector.ByTwoVertices(start, end);
            gVector startMid = gVector.ByTwoVertices(start, this);
            gVector endMid = gVector.ByTwoVertices(this, end);
            if (!startMid.IsParallelTo(endMid)){ return false; } // Not aligned
            double dotAC = startEnd.Dot(startMid);
            double dotAB = startEnd.Dot(startEnd);
            return 0 <= dotAC && dotAC <= dotAB;
        }

        internal static bool OnEdgeProjection(gVertex start, gVertex point, gVertex end, string plane = "xy")
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

        //public DSPoint AsPoint()
        //{
        //    return DSPoint.ByCoordinates(this.X, this.Y, this.Z);
        //}

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
            
            return Threshold(this.X, obj.X) && Threshold(this.Y, obj.Y) && Threshold(this.Z, obj.Z);
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
            System.Globalization.NumberFormatInfo inf = new System.Globalization.NumberFormatInfo();
            inf.NumberDecimalSeparator = ".";
            return string.Format("gVertex(X = {0}, Y = {1}, Z = {2})", X.ToString("0.000", inf), Y.ToString("0.000", inf), Z.ToString("0.000", inf));
        }

        /// <summary>
        /// Customizing the render of gVertex
        /// </summary>
        /// <param name="package"></param>
        /// <param name="parameters"></param>
        //[IsVisibleInDynamoLibrary(false)]
        //public void Tessellate(IRenderPackage package, TessellationParameters parameters)
        //{
        //    package.AddPointVertex(X, Y, Z);
        //    package.AddPointVertexColor(255, 0, 0, 255);
        //}

        /// <summary>
        /// Implementation of Clone method
        /// </summary>
        public object Clone()
        {
            gVertex newVertex = new gVertex(this.X, this.Y, this.Z, this.polygonId);

            return newVertex;
        }

        
        #endregion

    }
}
