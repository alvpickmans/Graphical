using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphical.Geometry;

namespace Graphical.Graphs
{

    /// <summary>
    /// VisibilityGraph graph's EdgeKey class to create a tree data structure.
    /// </summary>
    public class EdgeKey : IComparable<EdgeKey>
    {
        internal Edge Edge { get; private set; }
        internal Ray Ray { get; private set; }

        internal EdgeKey(Ray ray, Edge e)
        {
            Ray = ray;
            Edge = e;
        }

        internal EdgeKey(Vertex centre, Vertex end, Edge e)
        {
            Edge = e;
            Ray = Ray.ByTwoVertices(centre, end);
        }

        public double DistanceToIntersection(Edge edge)
        {
            if (this.Ray.Intersection(edge) is Vertex intersection)
            {
                return this.Ray.Origin.DistanceTo(intersection);
            }

            return Double.NaN;
        }


        /// <summary>
        /// Override of Equals method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) { return false; }

            EdgeKey k = (EdgeKey)obj;
            return Edge.Equals(k.Edge);
        }

        /// <summary>
        /// Override of GetHashCode method
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Ray.GetHashCode() ^ Edge.GetHashCode();
        }

        /// <summary>
        /// Implementation of IComparable interface
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(EdgeKey other)
        {
            if (other == null) { return 1; }
            if (this.Edge.Equals(other.Edge)) { return 1; }
            if (this.Ray.Intersection(other.Edge) == null) { return -1; }

            double selfDist = this.DistanceToIntersection(Edge);
            double otherDist = this.DistanceToIntersection(other.Edge);

            if (selfDist > otherDist) { return 1; }
            else if (selfDist < otherDist) { return -1; }
            else
            {
                Vertex sameVertex = null;
                if (other.Edge.Contains(Edge.StartVertex)) { sameVertex = Edge.StartVertex; }
                else if (other.Edge.Contains(Edge.EndVertex)) { sameVertex = Edge.EndVertex; }

                // TODO: To be replaced with Vector angle
                double aslf = Vertex.ArcRadAngle(sameVertex, this.Ray.Origin, Edge.GetVertexPair(sameVertex));
                double aot = Vertex.ArcRadAngle(sameVertex, this.Ray.Origin, other.Edge.GetVertexPair(sameVertex));

                if (aslf < aot) { return -1; }
                else { return 1; }
            }

        }

        /// <summary>
        /// Implementaton of IComparable interface
        /// </summary>
        /// <param name="k1"></param>
        /// <param name="k2"></param>
        /// <returns></returns>
        public static bool operator <(EdgeKey k1, EdgeKey k2)
        {
            return k1.CompareTo(k2) < 0;
        }

        /// <summary>
        /// Implementation of IComparable interface
        /// </summary>
        /// <param name="k1"></param>
        /// <param name="k2"></param>
        /// <returns></returns>
        public static bool operator >(EdgeKey k1, EdgeKey k2)
        {
            return k1.CompareTo(k2) > 0;
        }

        /// <summary>
        /// Override of ToString method.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("EdgeKey: (Edge={0}, Ray={1})", Edge.ToString(), Ray.ToString());
        }
    }
}
