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
        private Edge path { get; set; }

        private Dictionary<int, Polygon> directObstacles { get; set; }
        private Dictionary<Edge, List<int>> computedEdgePolygons { get; set; }

        #endregion

        #region Private Constructors
        private ConvexGraph(Graph basegraph)
        {
            this.baseGraph = basegraph;
            this.directObstacles = new Dictionary<int, Polygon>();
            this.computedEdgePolygons = new Dictionary<Edge, List<int>>();
        }
        #endregion

        #region Public Constructors
        public static ConvexGraph ByGraphOriginAndDestination(Graph basegraph, Vertex origin, Vertex destination)
        {
            var convexgraph = new ConvexGraph(basegraph)
            {
                path = Edge.ByStartVertexEndVertex(origin, destination)
            };

            convexgraph.ComputeConvexGraph();

            return convexgraph;
        } 
        #endregion

        private void ComputeConvexGraph()
        {
            var dioConvexEdges = SetDirectImpedingObstacles();
            var cleanEdges = EvaluateDIO(dioConvexEdges);

            foreach (Edge edge in cleanEdges)
            {
                this.AddEdge(edge);
            }
        }

        private List<Edge> SetDirectImpedingObstacles()
        {
            this.directObstacles = new Dictionary<int, Polygon>();
            List<Edge> edges = new List<Edge>();

            foreach (Polygon polygon in this.baseGraph.Polygons)
            {
                if (!FullyIntersects(this.path, polygon))
                    continue;

                this.directObstacles.Add(polygon.Id, polygon);
                edges.AddRange(GetConvexEdges(polygon, this.path.StartVertex, this.path.EndVertex));
            }

            return edges;
        }

        /// <summary>
        /// This method will evaluate Directly Impeding Obstacles.
        /// </summary>
        private List<Edge> EvaluateDIO(List<Edge> convexEdges)
        {
            List<Edge> edges = new List<Edge>();
            foreach (Edge edge in convexEdges)
            {
                RecursiveConvexEdges(edge, ref edges);
            }

            return edges;
        }

        private void RecursiveConvexEdges(Edge edge, ref List<Edge> edges, Polygon obstacle = null)
        {
            List<Edge> possibleintersections = new List<Edge>();

            // Getting ConvexHull Edges if any obstacle
            if(obstacle == null)
            {
                possibleintersections.Add(edge);
            }
            else
            {
                if (IntersectionAlreadyComputed(edge, obstacle.Id))
                    return;

                this.computedEdgePolygons[edge].Add(obstacle.Id);
                possibleintersections.AddRange(GetConvexEdges(obstacle, edge.StartVertex, edge.EndVertex));
            }

            // Checking possible intersections with existing DIOs
            foreach (Edge edgeCheck in possibleintersections)
            {
                if (edges.Contains(edgeCheck))
                    continue;

                bool intersectsAny = false;
                foreach (Polygon polygon in this.directObstacles.Values)
                {
                    if (obstacle != null && polygon.Id == obstacle.Id)
                        continue;

                    if (!FullyIntersects(edgeCheck, polygon))
                        continue;

                    intersectsAny = true;
                    RecursiveConvexEdges(edgeCheck, ref edges, polygon);
                }

                if (!intersectsAny)
                    edges.Add(edgeCheck);
            }
            
            return;
        }


        private static List<Edge> GetConvexEdges(Polygon polygon, params Vertex[] vertices)
        {
            var contextVertices = new List<Vertex>(polygon.Vertices);
            contextVertices.AddRange(vertices);

            var convexVertices = Vertex.ConvexHull(contextVertices);

            List<Edge> convexHull = new List<Edge>();

            for (int i = 0; i < convexVertices.Count; i++)
            {
                int nextIndex = (i + 1) % convexVertices.Count;
                var convexEdge = Edge.ByStartVertexEndVertex(convexVertices[i], convexVertices[nextIndex]);

                convexHull.Add(convexEdge);
            }

            return convexHull;
        }

        private bool FullyIntersects(Edge edge, Polygon polygon)
        {
            var intersections = polygon.Intersection(edge);

            if (intersections.Count > 1 && intersections.Count % 2 != 0 && intersections.First() is Vertex)
                throw new Exception("Start or End Point might be inside a polygon");

            return intersections.Count >= 2;
        }

        private bool IntersectionAlreadyComputed(Edge edge, int polygonId)
        {
            if (this.computedEdgePolygons.TryGetValue(edge, out List<int> ids))
            {
                if (ids.Contains(polygonId))
                    return true;
            }
            else
            {
                this.computedEdgePolygons.Add(edge, new List<int>());
            }

            return false;
        }
    }
}
