using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphical.Geometry;

namespace Graphical.Graphs
{
    public class ConvexGraph : Graph
    {
        #region Private Properties

        private Graph baseGraph { get; set; }
        private Vertex origin { get; set; }
        private Vertex destination { get; set; }

        #endregion

        #region Private Constructors
        private ConvexGraph(Graph basegraph)
        {
            this.baseGraph = basegraph;
        }
        #endregion

        #region Public Constructors
        public static ConvexGraph ByGraphOriginAndDestination(Graph basegraph, Vertex origin, Vertex destination)
        {
            var convexgraph = new ConvexGraph(basegraph)
            {
                origin = origin,
                destination = destination
            };

            convexgraph.ComputeConvexGraph();

            return convexgraph;
        } 
        #endregion

        private void ComputeConvexGraph()
        {
            Stack<Edge> edgeQ = new Stack<Edge>();

            edgeQ.Push(Edge.ByStartVertexEndVertex(this.origin, this.destination));
            int polygonCount = this.baseGraph.Polygons.Count();

            while (edgeQ.Any())
            {
                var edge = edgeQ.Pop();
                bool doesIntersect = false;

                for (int i = 0; i < polygonCount; i++)
                {
                    var polygon = this.baseGraph.Polygons[i];
                    // On some cases, particulary with concave polygons, the intersections
                    // can be a mixture of edges and vertices
                    var intersections = polygon.Intersection(edge);
                    int count = intersections.Count();
                    if (count > 1 && intersections.Any(intr => !polygon.Vertices.Contains(intr)))
                    {
                        doesIntersect = true;

                        // If it is pair, it means that the edge fully intersects the polygon
                        if(count % 2 == 0)
                        {
                            var edges = this.GetConvexEdges(edge.StartVertex, edge.EndVertex, polygon);
                            edges["clean"].ForEach(e => this.AddEdge(e));
                            edges["check"].ForEach(e => edgeQ.Push(e));
                        }
                        else
                        {
                            throw new Exception("Part of the edge is inside a polygon.");
                        }
                       
                    }
                }

                if (!doesIntersect)
                {
                    this.AddEdge(edge);
                }
            }


        }

        /// <summary>
        /// This method will return a List with the first item being edges belonging to the original polygon
        /// and the second those that will need to be check for further intersection.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <param name="polygon"></param>
        /// <returns></returns>
        private Dictionary<string, List<Edge>> GetConvexEdges(Vertex origin, Vertex destination, Polygon polygon)
        {
            var vertices = new List<Vertex>(polygon.Vertices);
            if (!vertices.Contains(origin)) { vertices.Add(origin); }
            if (!vertices.Contains(destination)) { vertices.Add(destination); }

            var cleanEdges = new List<Edge>();
            var toCheckEdges = new List<Edge>();

            var convexVertices = Vertex.ConvexHull(vertices);

            for (int i = 0; i < convexVertices.Count; i++)
            {
                int nextIndex = (i + 1) % convexVertices.Count;

                var edge = Edge.ByStartVertexEndVertex(convexVertices[i], convexVertices[nextIndex]);

                if(!polygon.Belongs(edge) || edge.Contains(origin) || edge.Contains(destination))
                {
                    toCheckEdges.Add(edge);
                }
                else
                {
                    cleanEdges.Add(edge);
                }

            }

            return new Dictionary<string, List<Edge>>()
            {
                {"clean", cleanEdges },
                {"check", toCheckEdges }
            };
        }
    }
}
