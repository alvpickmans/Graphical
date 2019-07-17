#region namespaces
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
    /// Representation of Edges on a graph
    /// </summary>
    public class Edge : Geometry
    {
        #region Variables
        /// <summary>
        /// StartVertex
        /// </summary>
        public Vertex StartVertex { get; private set; }

        /// <summary>
        /// EndVertex
        /// </summary>
        public Vertex EndVertex { get; private set; }


        public double Length { get; private set; }

        public Vector Direction { get; private set; }

        #endregion


        #region Private Constructor
        private Edge(Vertex start, Vertex end) : base()
        {
            if (start.Equals(end))
            {
                throw new Exception("Edge cannot be created with Vertex with same coordinates");
            }
            StartVertex = start;
            EndVertex = end;
            Length = start.DistanceTo(end);
            Direction = Vector.ByTwoVertices(StartVertex, EndVertex);
        } 
        #endregion

        #region Public Constructors
        /// <summary>
        /// Edge constructor by start and end Vertices
        /// </summary>
        /// <param name="start">Start vertex</param>
        /// <param name="end">End Vertex</param>
        /// <returns name="edge">edge</returns>
        public static Edge ByStartVertexEndVertex(Vertex start, Vertex end)
        {
            return new Edge(start, end);
        }

        /// <summary>
        /// Edge constructor by an array of coordinates
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns name="edge">edge</returns>
        public static Edge ByCoordinatesArray(double[] coordinates)
        {
            if(coordinates.Count() != 6) { throw new Exception("Not 6 coordinates provided"); }
            Vertex start = Vertex.ByCoordinates(coordinates[0], coordinates[1], coordinates[2]);
            Vertex end = Vertex.ByCoordinates(coordinates[3], coordinates[4], coordinates[5]);
            return new Edge(start, end);
        }
        #endregion

        /// <summary>
        /// Method to check if vertex is either the start or end vertex
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public bool Contains(Vertex vertex)
        {
            return StartVertex.Equals(vertex) || EndVertex.Equals(vertex);
        }

        /// <summary>
        /// Method to return the other end vertex of the Edge
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public Vertex GetVertexPair(Vertex vertex)
        {
            return (StartVertex.Equals(vertex)) ? EndVertex : StartVertex;
        }

        public bool IsCoplanarTo(Edge edge)
        {
            return this.IsCoplanarTo(edge.StartVertex, edge.Direction);
        }

        public bool IsCoplanarTo(Vertex vertex, Vector direction)
        {
            // http://mathworld.wolfram.com/Coplanar.html
            var a = this.Direction;
            var b = direction;
            var c = Vector.ByTwoVertices(this.StartVertex, vertex);

            return c.Dot(a.Cross(b)).AlmostEqualTo(0);
        }

        public Geometry Intersection(Edge other)
        {
            // http://mathworld.wolfram.com/Line-LineIntersection.html
            if (!this.BoundingBox.Intersects(other.BoundingBox))
                return null;

            if (this.Equals(other))
                return this;

            var ray = Ray.ByTwoVertices(this.StartVertex, this.EndVertex);

            if (!ray.TryIntersectionOffset(other, out double offset, true) || !offset.InRange(-1, 1))
                return null;

            if (Double.IsInfinity(offset))
            {
                if (!ray.TryIntersectionOffset(other.StartVertex, out double startOffset, true))
                    return null;

                if (!ray.TryIntersectionOffset(other.EndVertex, out double endOffset, true))
                    return null;

                return Edge.ByStartVertexEndVertex(
                    ray.Origin.Translate(ray.Direction.Scale(startOffset)),
                    ray.Origin.Translate(ray.Direction.Scale(endOffset))
                );
            }

            if (!Double.IsNaN(offset))
            {
                var vertex = ray.Origin.Translate(ray.Direction.Scale(offset));

                return vertex.OnEdge(other) ? vertex : null;
            }

            return null;
        }

        public bool Intersects(Edge edge)
        {
            if(this.StartVertex.OnEdge(edge) || this.EndVertex.OnEdge(edge))
            {
                if (this.Direction.IsParallelTo(edge.Direction))
                {
                    return true;
                }
            }
            return this.Intersection(edge) != null;
        }

        public double DistanceTo(Vertex vertex)
        {
            return vertex.DistanceTo(this);
        }

        public double DistanceTo(Edge edge)
        {
            // http://mathworld.wolfram.com/Line-LineDistance.html
            if (this.IsCoplanarTo(edge))
            {
                var distances = new double[4]{
                    StartVertex.DistanceTo(edge),
                    EndVertex.DistanceTo(edge),
                    edge.StartVertex.DistanceTo(this),
                    edge.EndVertex.DistanceTo(this)
                };
                return distances.Min();
            }else
            {
                var a = this.Direction;
                var b = edge.Direction;
                var c = Vector.ByTwoVertices(this.StartVertex, edge.StartVertex);
                Vector cross = a.Cross(b);
                double numerator = c.Dot(cross);
                double denominator = cross.Length;
                return Math.Abs(numerator) / Math.Abs(denominator);

            }
            
        }

        #region override methods
        //TODO: Improve overriding equality methods as per http://www.loganfranken.com/blog/687/overriding-equals-in-c-part-1/

        /// <summary>
        /// Override of Equal Method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) { return false; }

            Edge e= (Edge)obj;
            if (StartVertex.Equals(e.StartVertex) && EndVertex.Equals(e.EndVertex)) { return true; }
            if (StartVertex.Equals(e.EndVertex) && EndVertex.Equals(e.StartVertex)) { return true; }
            return false;

        }

        /// <summary>
        /// Override of GetHashCode Method
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return StartVertex.GetHashCode() ^ EndVertex.GetHashCode();
        }


        /// <summary>
        /// Override of ToString method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Edge(StartVertex: {0}, EndVertex: {1})", StartVertex, EndVertex);
        }

        internal override BoundingBox ComputeBoundingBox()
        {
            return BoundingBox.ByMinVertexMaxVertex(StartVertex, EndVertex);
        }

        #endregion

    }

    
    
}
