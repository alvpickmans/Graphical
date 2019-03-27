using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphical.Extensions;

namespace Graphical.Geometry
{
    /// <summary>
    /// Ray object
    /// </summary>
    public class Ray
    {
        #region Public Properties
        /// <summary>
        /// Ray's origin coordinates
        /// </summary>
        public Vertex Origin { get; private set; }

        /// <summary>
        /// Ray's coordinates defining the direction
        /// </summary>
        public Vector Direction { get; private set; }
        #endregion

        #region Private Constructors
        internal Ray(Vertex origin, Vector direction)
        {
            this.Origin = origin;
            this.Direction = direction;
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
            // If the Vector from Ray's origin to the given
            // vertex is parallel to Ray's Direction, return true.
            var vector = Vector.ByTwoVertices(this.Origin, vertex);

            if (!this.Direction.IsParallelTo(vector)) { return false; }

            // Need to check if point falls on the visible path of Ray.
            // Vertex = Origin + t * Direction; t = (V-O)/D
            // If t < 0, it doesn't

            double t = Double.PositiveInfinity;
            if(this.Direction.X != 0) { t = (vertex.X - this.Origin.X) / this.Direction.X; }
            else if (this.Direction.Y != 0) { t = (vertex.Y - this.Origin.Y) / this.Direction.Y; }
            else if (this.Direction.Z != 0) { t = (vertex.Z - this.Origin.Z) / this.Direction.Z; }
            else
            {
                throw new Exception("Something when wrong, contact Author");
            }

            return t > 0 && !t.AlmostEqualTo(0);
        }


        public Geometry Intersection(Edge edge)
        {
            // If no coplanar, cannot intersect
            if(!edge.IsCoplanarTo(this.Origin, this.Direction)) { return null; }

            // Intersection can be an Edge or null
            if(this.Direction.IsParallelTo(edge.Direction)){

                bool containsStart = this.Contains(edge.StartVertex);
                bool containsEnd = this.Contains(edge.EndVertex);

                if(!containsStart && !containsEnd)
                {
                    return null;
                }
                else if (containsStart) {
                    return Edge.ByStartVertexEndVertex(edge.StartVertex, this.Origin);
                }
                else
                {
                    return Edge.ByStartVertexEndVertex(this.Origin, edge.EndVertex);
                }

            }

            // No coincident nor same extremes
            var b = this.Direction;
            var a = edge.Direction;
            var c = Vector.ByTwoVertices(edge.StartVertex, this.Origin);
            var cxb = c.Cross(b);
            var axb = a.Cross(b);
            var dot = cxb.Dot(axb);

            double t = (dot) / Math.Pow(axb.Length, 2);

            if (t < 0 && !t.AlmostEqualTo(0)) { return null; }
            if (t > 1 && !t.AlmostEqualTo(1)) { return null; }

            var intersection = edge.StartVertex.Translate(edge.Direction.Scale(t));
            return this.Contains(intersection) ? intersection : null;

        }

        public bool Intersects(Edge edge)
        {
            return this.Intersection(edge) is Geometry;
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
            return String.Format("Ray(Origin: {0}, Direction: {1})", Origin.ToString(), Direction.ToString());
        }
        #endregion
    }
}
