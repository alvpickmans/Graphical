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
        #endregion

        #region Private Constructors
        private ConvexGraph(Graph basegraph)
        {
            this.baseGraph = basegraph;
            this.directObstacles = new Dictionary<int, Polygon>();
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
            this
                .SetDirectImpedingObstacles()
                .EvaluateDIO();

        }

        private ConvexGraph SetDirectImpedingObstacles()
        {
            this.directObstacles = new Dictionary<int, Polygon>();

            foreach (Polygon polygon in this.baseGraph.Polygons)
            {
                var intersections = polygon.Intersection(this.path);

                if (intersections.Any())
                    this.directObstacles.Add(polygon.Id, polygon);
            }

            return this;
        }

        /// <summary>
        /// This method will evaluate Directly Impeding Obstacles.
        /// </summary>
        private ConvexGraph EvaluateDIO()
        {
            var edges = GetConvexEdges(this.path, this.directObstacles.Values);

            foreach (Edge edge in edges)
            {
                this.AddEdge(edge);
            }

            return this;
        }

        //private void ComputeConvexGraphOld()
        //{
        //    Stack<Edge> stack = new Stack<Edge>();

        //    // Generate ConvexHull for each intersecting obstacle
        //    foreach (Polygon polygon in this.directObstacles.Values)
        //    {
        //        var edges = this.GetConvexEdges(this.path, this.directObstacles.Values, polygon);
        //        foreach (Edge edge in edges)
        //        {
        //            if (polygon.Belongs(edge))
        //                this.AddEdge(edge);
        //            else
        //                stack.Push(edge);
        //        }
        //    }
        //    int polygonCount = this.baseGraph.Polygons.Count();

        //    while (stack.Any())
        //    {
        //        var edge = stack.Pop();

        //        if (this.Edges.Contains(edge))
        //            continue;

        //        bool doesIntersect = false;

        //        for (int i = 0; i < polygonCount; i++)
        //        {
        //            //Check if this polygon has already been intersected before!!!
        //            var polygon = this.baseGraph.Polygons[i];
        //            // On some cases, particulary with concave polygons, the intersections
        //            // can be a mixture of edges and vertices
        //            var intersections = polygon.Intersection(edge);
        //            int count = intersections.Count();
        //            if (count > 1)
        //            {
        //                doesIntersect = true;

        //                // If it is pair, it means that the edge fully intersects the polygon
        //                if(count % 2 == 0)
        //                {
        //                    this.GetConvexEdges(edge.StartVertex, edge.EndVertex, polygon, out List<Edge> clean, out List<Edge> check);
        //                    clean.ForEach(e => this.AddEdge(e));
        //                    check.ForEach(e => stack.Push(e));
        //                }
        //                else
        //                {
        //                    throw new Exception("Part of the edge is inside a polygon.");
        //                }
        //            }
        //        }

        //        if (!doesIntersect)
        //        {
        //            this.AddEdge(edge);
        //        }
        //    }
        //}

        /// <summary>
        /// This method will return a list of the convex hull edges between polygon, origin and destination vertices
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <param name="polygon"></param>
        /// <returns></returns>
        private static List<Edge> GetConvexEdges(Edge edge, IEnumerable<Polygon> dios, Polygon obstacle = null)
        {
            List<Edge> cleanEdges = new List<Edge>();
            List<Edge> possibleintersections = new List<Edge>();

            // Getting ConvexHull Edges if any obstacle
            if(obstacle == null)
            {
                possibleintersections.Add(edge);
            }
            else
            {
                var vertices = new List<Vertex>(obstacle.Vertices);
                vertices.Add(edge.StartVertex);
                vertices.Add(edge.EndVertex);

                var convexVertices = Vertex.ConvexHull(vertices);

                for (int i = 0; i < convexVertices.Count; i++)
                {
                    int nextIndex = (i + 1) % convexVertices.Count;
                    var convexEdge = Edge.ByStartVertexEndVertex(convexVertices[i], convexVertices[nextIndex]);

                    if (obstacle.Belongs(edge))
                        cleanEdges.Add(edge);
                    else
                        possibleintersections.Add(convexEdge);
                }
            }

            // Checking possible intersections with existing DIOs
            foreach (Edge edgeCheck in possibleintersections)
            {
                bool intersectsAny = false;
                foreach (Polygon polygon in dios)
                {
                    if (obstacle != null && polygon.Id == obstacle.Id)
                        continue;

                    var intersections = polygon.Intersection(edgeCheck);


                    if (intersections.Count > 1 && intersections.Count % 2 != 0 && intersections.First() is Vertex)
                        throw new Exception("Start or End Point might be inside a polygon");

                    if (intersections.Count >= 2)
                    {
                        intersectsAny = true;
                        cleanEdges.AddRange(GetConvexEdges(edgeCheck, dios, polygon));
                    }
                }

                if (!intersectsAny)
                    cleanEdges.Add(edgeCheck);
            }
            
            return cleanEdges;
        }
    }
}
