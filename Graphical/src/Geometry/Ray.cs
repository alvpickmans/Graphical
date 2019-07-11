using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphical.Extensions;

namespace Graphical.Geometry
{
    /// <summary>
    /// Ray defined by an Origin Vertex and a Normalized Direction
    /// </summary>
    public class Ray
    {
        #region Public Properties
        /// <summary>
        /// Origin Vertex
        /// </summary>
        public Vertex Origin { get; private set; }

        /// <summary>
        /// Direction Vector
        /// </summary>
        public Vector Direction { get; private set; }
        #endregion

        #region Private Constructors
        internal Ray(Vertex origin, Vector direction)
        {
            this.Origin = origin ?? throw new ArgumentNullException(nameof(origin));
            this.Direction = direction ?? throw new ArgumentNullException(nameof(direction));

            if (direction.Length.AlmostEqualTo(0))
                throw new ArgumentException($"Cannot create a {nameof(Ray)} with a {nameof(Vector)} of size 0.", nameof(direction));
        }
        #endregion

        #region Public Constructors
        /// <summary>
        /// Creates a Ray by defining it's origin and a second vertex defining its direction
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public static Ray ByTwoVertices(Vertex origin, Vertex vertex)
        {
            return new Ray(
                    origin,
                    Vector.ByTwoVertices(origin, vertex)
                );
        }

        /// <summary>
        /// Creates a Ray by its origin point and direction vector.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Ray ByOriginAndVector(Vertex origin, Vector vector)
        {
            return new Ray(origin, vector);
        }

        /// <summary>
        /// Creates a Ray parallel to the X Axis through the given origin vertex
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static Ray XAxis(Vertex origin)
        {
            return new Ray(
                    origin,
                    Vector.XAxis()
                );
        }

        /// <summary>
        /// Creates a Ray parallel to the Y Axis through the given origin vertex
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static Ray YAxis(Vertex origin)
        {
            return new Ray(
                    origin,
                    Vector.YAxis()
                );
        } 

        /// <summary>
        /// Creates a Ray parallel to the Z Axis through the given origin vertex
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static Ray ZAxis(Vertex origin)
        {
            return new Ray(
                    origin,
                    Vector.ZAxis()
                );
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Checks if the given Vertex is contained on the Ray.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public bool Contains(Vertex vertex)
        {
            if (this.Origin.Equals(vertex))
                return true;

            double t = this.IntersectionOffset(vertex);

            return t > 0 && !t.AlmostEqualTo(0);
        }


        public Geometry Intersection(Edge edge)
        {
            if (!this.TryIntersectionOffset(edge, out double offset))
                return null;

            if (Double.IsInfinity(offset))
            {
                bool containsStart = this.Contains(edge.StartVertex);
                bool containsEnd = this.Contains(edge.EndVertex);

                if (containsStart && containsEnd)
                    return edge;

                if (containsStart)
                    return Edge.ByStartVertexEndVertex(this.Origin, edge.StartVertex);

                if (containsEnd)
                    return Edge.ByStartVertexEndVertex(this.Origin, edge.EndVertex);

                return null;

            }

            var intersection = this.Origin.Translate(this.Direction.Scale(offset));
            return intersection.OnEdge(edge) ? intersection : null;

        }

        public bool Intersects(Edge edge)
        {
            return this.TryIntersectionOffset(edge, out double offset);
        }

        /// <summary>
        /// Returns the offset from the <see cref="Origin"/> to a <see cref="Vertex"/>
        /// or <see cref="Double.NaN"/> if no intersection exists.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public double IntersectionOffset(Vertex vertex)
        {
            if (this.Direction.Length.AlmostEqualTo(0))
                return Double.NaN;

            // If Ray and vector from Origin-Vertex are not parallel,
            // Cannot intersect and offset is NaN
            var vector = Vector.ByTwoVertices(this.Origin, vertex);
            if (!this.Direction.IsParallelTo(vector))
                return Double.NaN;

            // Need to check if point falls on the visible path of Ray.
            // Vertex = Origin + offset * Direction; offset = (V-O)/D
            if (!this.Direction.X.AlmostEqualTo(0))
                return (vertex.X - this.Origin.X) / this.Direction.X;

            if (!this.Direction.Y.AlmostEqualTo(0))
                return (vertex.Y - this.Origin.Y) / this.Direction.Y;

            if (!this.Direction.Z.AlmostEqualTo(0))
                return (vertex.Z - this.Origin.Z) / this.Direction.Z;

            throw new Exception($"Could not calculate intersection between {vertex} and {this}");
        }

        /// <summary>
        /// Returns the offset from the <see cref="Origin"/> to the intersection
        /// point with an <see cref="Edge"/>, <see cref="Double.PositiveInfinity"/> if they are parallel
        /// or <see cref="Double.NaN"/> if not intersecting (no coplanar).
        /// </summary>
        /// <param name="edge"></param>
        /// <returns>Offset from Ray's Origin point, <see cref="Double.PositiveInfinity"/> if they are parallel or <see cref="Double.NaN"/> if not intersecting</returns>
        public double IntersectionOffset(Edge edge)
        {
            if (!edge.IsCoplanarTo(this.Origin, this.Direction))
                return Double.NaN;

            if (this.Direction.IsParallelTo(edge.Direction))
                return Double.PositiveInfinity;

            var a = this.Direction;
            var b = edge.Direction;
            var c = Vector.ByTwoVertices(this.Origin, edge.StartVertex);
            var cxb = c.Cross(b);
            var axb = a.Cross(b);
            var dot = cxb.Dot(axb);

            return (dot) / Math.Pow(axb.Length, 2);
        }

        /// <summary>
        /// Determines if a Ray intersects a <see cref="Vertex"/>. See <see cref="Ray.IntersectionOffset(Vertex)"/>
        /// for more details on possible outputs.
        /// </summary>
        /// <param name="vertex">Vertex to test intersection against</param>
        /// <param name="offset">Offset from Origin along Ray's direction to the Vertex</param>
        /// <param name="bothSides">Considers as intersecting when it happens on either side of Origin (direction or its inverse)</param>
        /// <returns>True if Ray intersects the edge.</returns>
        public bool TryIntersectionOffset(Vertex vertex, out double offset, bool bothSides = false)
        {
            offset = this.IntersectionOffset(vertex);

            if (Double.IsNaN(offset))
                return false;

            // If negative, is on the opposite direction
            if (offset < 0 && !offset.AlmostEqualTo(0))
                return bothSides;

            return true;
        }

        /// <summary>
        /// Determines if a Ray intersects an <see cref="Edge"/>. See <see cref="Ray.IntersectionOffset(Edge)"/>
        /// for more details on possible outputs.
        /// </summary>
        /// <param name="edge">Edge to test intersection against</param>
        /// <param name="offset">Offset from Origin along Ray's direction to the intersection</param>
        /// <param name="bothSides">Considers as intersecting when it happens on either side of Origin (direction or its inverse)</param>
        /// <returns>True if Ray intersects the edge.</returns>
        public bool TryIntersectionOffset(Edge edge, out double offset, bool bothSides = false)
        {
            offset = this.IntersectionOffset(edge);

            // Ray and Edge are not coplanar
            if (Double.IsNaN(offset))
                return false;

            // If infinity, can be only parallel or colinear.
            if (Double.IsInfinity(offset))
                return this.TryIntersectionOffset(edge.StartVertex, out double startOffset, bothSides)
                    || this.TryIntersectionOffset(edge.EndVertex, out double endOffset, bothSides);

            // If negative, is on the opposite direction
            if (offset < 0 && !offset.AlmostEqualTo(0))
                return bothSides;

            return true;
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Override of GetHashCode method
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Origin.GetHashCode() ^ this.Direction.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format($"{nameof(Ray)}({nameof(this.Origin)}: {this.Origin}, {nameof(this.Direction)}: {this.Direction})");
        }
        #endregion
    }
}
