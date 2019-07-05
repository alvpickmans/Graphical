﻿#region namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphical.Extensions;
#endregion

namespace Graphical.Geometry
{
    /// <summary>
    /// Representation of vertex points on a graph.
    /// </summary>
    public class Vertex : Geometry, ICloneable, IEquatable<Vertex>
    {

        #region Properties
        /// <summary>
        /// X Coordinate
        /// </summary>
        public double X { get; private set; }

        /// <summary>
        /// Y Coordinate
        /// </summary>
        public double Y { get; private set; }

        /// <summary>
        /// Z Coordinate
        /// </summary>
        public double Z { get; private set; }

        #endregion

        #region Internal/Private Constructors
        private Vertex(double x, double y, double z = 0) : base()
        {
            X = x.Round();
            Y = y.Round();
            Z = z.Round();
        }
        
        #endregion
        #region Public Constructors
        /// <summary>
        /// Vertex constructor method by a given set of XYZ coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vertex ByCoordinates(double x, double y, double z = 0)
        {
            return new Vertex(x, y, z);
        }

        /// <summary>
        /// Vertex constructor by a array of 3 coordinates
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static Vertex ByCoordinatesArray(double[] array)
        {
            if(array.Count() != 3)
            {
                throw new ArgumentOutOfRangeException("array", array.Count(), "Number of coordinates provided must be 3");
            }
            return new Vertex(array[0], array[1], array[2]);
        }

        /// <summary>
        /// Returns the vertex in between two Vertices.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns name="midVertex"></returns>
        public static Vertex MidVertex ( Vertex v1, Vertex v2)
        {
            double x = (v1.X + v2.X) / 2, y = (v1.Y + v2.Y) / 2, z = (v1.Z + v2.Z) / 2;
            return new Vertex(x, y, z);
        }

        /// <summary>
        /// Returns the Origin vertex at 0, 0, 0;
        /// </summary>
        /// <returns></returns>
        public static Vertex Origin()
        {
            return new Vertex(0, 0, 0);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns the Minimum VErtex from a set of vertices,
        /// being this with the minimum Y, X and then Z coordinates.
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static Vertex MinimumVertex(List<Vertex> vertices)
        {
            return vertices.OrderBy(v => v.Y).ThenBy(v => v.X).ThenBy(v => v.Z).ToList().First();
        }

        /// <summary>
        /// Sorts a list of vertices by their polar angle  and distance
        /// around a centre point. At equal angle, closest vertex comes first.
        /// If centre is not provided, the Minimum Vertex of the set will be used as Centre.
        /// </summary>
        /// <param name="vertices">Vertices to sort</param>
        /// <param name="centre">Centre</param>
        /// <returns></returns>
        public static List<Vertex> OrderByRadianAndDistance(List<Vertex> vertices, Vertex centre = null)
        {
            if (centre == null) { centre = Vertex.MinimumVertex(vertices); }
            return vertices.OrderBy(v => RadAngle(centre, v)).ThenBy(v => centre.DistanceTo(v)).ToList();

        }

        /// <summary>
        /// Euclidean distance to another Vertex
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public double DistanceTo(Vertex vertex)
        {
            return Math.Sqrt(Math.Pow(vertex.X - X, 2) + Math.Pow(vertex.Y - Y, 2) + Math.Pow(vertex.Z - Z, 2));
        }

        /// <summary>
        /// Euclidean minimum distance to an Edge
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public double DistanceTo(Edge edge)
        {
            // http://mathworld.wolfram.com/Point-LineDistance3-Dimensional.html
            Vector v1 = Vector.ByTwoVertices(this, edge.StartVertex);
            Vector v2 = Vector.ByTwoVertices(this, edge.EndVertex);
            Vector numerator = v1.Cross(v2);
            Vector denominator = Vector.ByTwoVertices(edge.EndVertex, edge.StartVertex);
            return numerator.Length / denominator.Length;
        }

        /// <summary>
        /// Translates a Vertex by a given Vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vertex Translate(Vector vector)
        {
            return Vertex.ByCoordinates(this.X + vector.X, this.Y + vector.Y, this.Z + vector.Z);
        }

        /// <summary>
        /// Translates a Vertex by a given Vector and distance
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public Vertex Translate(Vector vector, double distance)
        {
            Vector normalized = vector.Normalized();
            Vector distVector = normalized * distance;
            return this.Translate(distVector);
        }

        /// <summary>
        /// Determines if the Vertex is contained a given Edge.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool OnEdge(Edge edge)
        {
            return this.OnEdge(edge.StartVertex, edge.EndVertex);
        }


        /// <summary>
        /// Determins if a Vertex lies on the left-hand side of an edge on the XY plane.
        /// 1 => for vertex left of the edge
        /// 0 => for vertex on the edge
        /// -1 => for vertex right of the edege
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="vertex"></param>
        /// <returns> 
        ///     
        /// </returns>
        public int IsLeftFrom(Edge edge)
        {
            double value =  (edge.EndVertex.X - edge.StartVertex.X) * (this.Y - edge.StartVertex.Y) - (edge.EndVertex.Y - edge.StartVertex.Y) * (this.X - edge.StartVertex.X);

            if (value.AlmostEqualTo(0)) { return 0; }

            return value > 0 ? 1 : -1;
        }

        /// <summary>
        /// Determines if the Vertex is contained on an
        /// Edge defined by a start and end vertices
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool OnEdge(Vertex start, Vertex end)
        {
            if (this.Equals(start) || this.Equals(end)) { return true; }
            // https://www.lucidarme.me/check-if-a-point-belongs-on-a-line-segment/
            Vector startEnd = Vector.ByTwoVertices(start, end);
            Vector startMid = Vector.ByTwoVertices(start, this);
            Vector endMid = Vector.ByTwoVertices(this, end);
            if (!startMid.IsParallelTo(endMid)) { return false; } // Not aligned
            double dotAC = startEnd.Dot(startMid);
            double dotAB = startEnd.Dot(startEnd);
            return 0 <= dotAC && dotAC <= dotAB;
        }

        /// <summary>
        /// Checks if a list of Vertices are coplanar. True for three or less Vertices.
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns>Boolean</returns>
        public static bool Coplanar(List<Vertex> vertices)
        {
            // https://math.stackexchange.com/questions/1330357/show-that-four-points-are-coplanar
            if (!vertices.Any()) { throw new ArgumentOutOfRangeException("Vertices", "Vertices list cannot be empty"); }
            if (vertices.Count <= 3) { return true; }
            Vector ab = Vector.ByTwoVertices(vertices[0], vertices[1]);
            Vector ac = Vector.ByTwoVertices(vertices[0], vertices[2]);
            Vector cross = ab.Cross(ac);

            return vertices.Skip(3).All(vtx => Vector.ByTwoVertices(vertices[0], vtx).Dot(cross).AlmostEqualTo(0));
        }

        /// <summary>
        /// TODO: Implement Colinear vertices.
        /// http://mathworld.wolfram.com/Collinear.html
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static bool Colinear(List<Vertex> vertices)
        {
            throw new NotImplementedException();
        }

        public static List<Vertex> ConvexHull(List<Vertex> vertices)
        {
            var verticesCount = vertices.Count();
            //If less than 3, cannot be created
            if(verticesCount < 3)
            {
                throw new ArgumentOutOfRangeException("vertices", verticesCount, "No ConvexHull can be created with less than 3 vertices");
            }

            // TODO: Check if they are colinear
            //if (Vertex.Colinear(vertices))
            //{
            //    throw new ArgumentException("Vertices are colinear", "vertices");
            //}

            Vertex minVertex = Vertex.MinimumVertex(vertices);
            List<Vertex> sorted = Vertex.OrderByRadianAndDistance(vertices, minVertex);

            Stack<Vertex> stack = new Stack<Vertex>();

            // Adding fist vertex to stack.
            stack.Push(minVertex);
            //First vertex on sorted should be the minVertex itself
            // so just skip it.
            for (int i = 1; i < verticesCount; i++)
            {
                int nextIndex = (i + 1) % verticesCount;
                var vertex = sorted[i];
                var nextVertex = sorted[nextIndex];

                double vertexAngle = Vertex.RadAngle(minVertex, vertex);
                double nextVertexAngle = Vertex.RadAngle(minVertex, nextVertex);

                // If for some reason, vertex and nextVertex are the same, skip it
                if (vertex.Equals(nextVertex)) { continue; }

                // If the angle is same as next, skip as we'll only use the fartest 
                // vertex with same angle.
                if (vertexAngle.AlmostEqualTo(nextVertexAngle)) { continue; }

                // If stack is still less than 3, keep adding.
                if(stack.Count < 3)
                {
                    stack.Push(vertex);
                }
                else
                {
                    // While it is not a left turn, remove top on stack
                    while(Vertex.Orientation(GetNextToTop<Vertex>(stack), stack.Peek(), vertex) != 1)
                    {
                        stack.Pop();
                    }
                    stack.Push(vertex);
                }

            }

            return stack.Reverse().ToList();
        }
        #endregion

        private static T GetNextToTop<T>(Stack<T> stack)
        {
            var top = stack.Pop();
            var next = stack.Peek();
            stack.Push(top);
            return next;
        }

        /// <summary>
        /// TODO: To be reviewed
        /// Returns the orientation of vertices p1-p2-p3
        /// 0 => Vertices are colinear
        /// 1 => Counterclock-wise, left turn
        /// -1 => Clock-wise, right turn
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        internal static int Orientation(Vertex p1, Vertex p2, Vertex p3, string plane = "xy")
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
            if (value.AlmostEqualTo(0)) { return 0; } //Points are colinear

            return (value > 0) ? 1 : -1; //Counter clock or clock wise
        }


        /// <summary>
        /// TODO: To be reviewed
        /// </summary>
        /// <param name="centre"></param>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public static double RadAngle(Vertex centre, Vertex vertex)
        {
            //Rad angles http://math.rice.edu/~pcmi/sphere/drg_txt.html
            double dx = vertex.X - centre.X;
            double dy = vertex.Y - centre.Y;
            bool onYAxis = dx.AlmostEqualTo(0);
            bool onXAxis = dy.AlmostEqualTo(0);
            //TODO: Implement Z angle? that would becom UV coordinates.
            //double dz = vertex.point.Z - centre.point.Z;

            if (onYAxis && onXAxis) { return 0; }

            if (onYAxis)// both Vertices on Y axis
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
            if (onXAxis)// both Vertices on X Axis
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


        /// <summary>
        /// TODO: To be reviewed
        /// </summary>
        /// <param name="centre"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static double ArcRadAngle (Vertex centre, Vertex start, Vertex end)
        {
            double a = Math.Pow((end.X - centre.X), 2) + Math.Pow((end.Y - centre.Y), 2);
            double b = Math.Pow((end.X - start.X), 2) + Math.Pow((end.Y - start.Y), 2);
            double c = Math.Pow((centre.X - start.X), 2) + Math.Pow((centre.Y - start.Y), 2);
            return Math.Acos((a + c - b) / (2 * Math.Sqrt(a) * Math.Sqrt(c)));
        }

        internal static bool OnEdgeProjection(Vertex start, Vertex point, Vertex end, string plane = "xy")
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

        #region Override Methods
        //TODO: Improve overriding equality methods as per http://www.loganfranken.com/blog/687/overriding-equals-in-c-part-1/

        /// <summary>
        /// Override of Equals method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Boolean</returns>
        public bool Equals(Vertex obj)
        {
            if (obj == null) { return false; }
            bool eq = this.X.AlmostEqualTo(obj.X) && this.Y.AlmostEqualTo(obj.Y) && this.Z.AlmostEqualTo(obj.Z);
            return eq;
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
            return string.Format("Vertex(X = {0}, Y = {1}, Z = {2})", X.ToString("0.000", inf), Y.ToString("0.000", inf), Z.ToString("0.000", inf));
        }

        /// <summary>
        /// Implementation of Clone method
        /// </summary>
        public object Clone()
        {
            Vertex newVertex = new Vertex(this.X, this.Y, this.Z);
            newVertex.UserData = this.UserData;

            return newVertex;
        }

        internal override BoundingBox ComputeBoundingBox()
        {
            return BoundingBox.ByMinVertexMaxVertex(this, this);
        }


        #endregion

    }
}
