#region namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphical.Geometry;
using System.Diagnostics;
#endregion

namespace Graphical.Graphs
{
    /// <summary>
    /// Construction of VisibilityGraph Graph
    /// </summary>
    public class VisibilityGraph : Graph, ICloneable
    {
        #region Variables

        internal Graph baseGraph { get; set; }

        #endregion

        #region Internal Constructors
        internal VisibilityGraph() : base()
        {
            baseGraph = new Graph();
        }

        internal VisibilityGraph(Graph _baseGraph, bool reducedGraph, bool halfScan = true) : base()
        {
            baseGraph = _baseGraph;

            List<gEdge> resultEdges = VisibilityAnalysis(baseGraph, baseGraph.vertices, reducedGraph, halfScan);

            foreach (gEdge edge in resultEdges)
            {
                this.AddEdge(edge);
            }
        }
        #endregion

        #region Public Constructors
        /// <summary>
        /// Creates a visibility graph by a set of polygons.
        /// </summary>
        /// <param name="polygons">Set of internal polygons</param>
        /// <returns name="visibilityGraph">VisibilityGraph graph</returns>
        //public static VisibilityGraph ByPolygons(DSPolygon[] polygons, bool reducedGraph = true)
        //{
        //    List<gPolygon> gPolygons = FromPolygons(polygons, false);
        //    Graph baseGraph = new Graph(gPolygons);
        //    VisibilityGraph g = new VisibilityGraph(baseGraph, reducedGraph, true);
        //    return g;
        //}

        /// <summary>
        /// Creates a visibility graph by a set of polygons and boundaries.
        /// </summary>
        /// <param name="polygons">Set of internal polygons</param>
        /// <param name="boundaries">Set of boundary polygons. These must not be present on the internal polygons list.</param>
        /// <returns name="visibilityGraph">VisibilityGraph graph</returns>
        //[MultiReturn(new[] { "visibilityGraph", "miliseconds" })]
        //public static Dictionary<string, object> ByPolygonsAndBoundaries(DSPolygon[] boundaries, [DefaultArgument("{}")]DSPolygon[] polygons = null, bool reducedGraph = true)
        //{
        //    var sw = new System.Diagnostics.Stopwatch();
        //    sw.Start();
        //    List<gPolygon> gPolygons = FromPolygons(boundaries, true);
        //    if(polygons.Any())
        //    {
        //        gPolygons.AddRange(FromPolygons(polygons, false));
        //    }
        //    Graph baseGraph = new Graph(gPolygons);

        //    var g = new VisibilityGraph(baseGraph, reducedGraph);

        //    sw.Stop();

        //    return new Dictionary<string, object>()
        //    {
        //        {"visibilityGraph", g },
        //        {"miliseconds", sw.ElapsedMilliseconds }
        //    };
        //}

        public static Graph VertexVisibility(gVertex origin, List<gPolygon> boundaries, List<gPolygon> internals = null, bool reducedGraph = true, bool halfScan = true)
        {
            List<gPolygon> polygons = new List<gPolygon>(boundaries);
            if (internals != null && internals.Any())
            {
                polygons.AddRange(internals);
            }

            VisibilityGraph visGraph = new VisibilityGraph()
            {
                baseGraph = new Graph(polygons)
            };

            gVertex o = origin;
            if(visGraph.baseGraph.Contains(origin)) { o = visGraph.baseGraph.vertices[visGraph.baseGraph.vertices.IndexOf(origin)]; }

            visGraph.edges = visGraph.VisibilityAnalysis(visGraph.baseGraph, new List<gVertex>() { o }, reducedGraph, halfScan);

            return visGraph;

        }

        //[MultiReturn(new[] { "visibilityGraph", "miliseconds" })]
        //public static Dictionary<string, object> ByPolygonsAndBoundariesFromPoint(DSPoint point,  DSPolygon[] boundaries, [DefaultArgument("{}")]DSPolygon[] polygons = null, bool reducedGraph = true)
        //{
        //    var sw = new System.Diagnostics.Stopwatch();
        //    sw.Start();
        //    List<gPolygon> gPolygons = FromPolygons(boundaries, true);
        //    if (polygons.Any())
        //    {
        //        gPolygons.AddRange(FromPolygons(polygons, false));
        //    }
        //    Graph _baseGraph = new Graph(gPolygons);

        //    var g = new VisibilityGraph()
        //    {
        //        baseGraph = _baseGraph
        //    };

        //    gVertex origin = gVertex.ByCoordinates(point.X, point.Y, point.Z);
        //    if (g.baseGraph.Contains(origin)) { origin = g.baseGraph.vertices[g.baseGraph.vertices.IndexOf(origin)]; }
        //    g.edges = g.VisibilityAnalysis(g.baseGraph, new List<gVertex>() { origin }, reducedGraph, false);

        //    sw.Stop();

        //    return new Dictionary<string, object>()
        //    {
        //        {"visibilityGraph", g },
        //        {"miliseconds", sw.ElapsedMilliseconds }
        //    };
        //}


        public static VisibilityGraph Merge(List<VisibilityGraph> graphs)
        {
            VisibilityGraph newGraph = new VisibilityGraph();

            foreach (VisibilityGraph g in graphs)
            {
                foreach (gPolygon p in g.baseGraph.polygons.Values)
                {
                    p.id = newGraph.baseGraph.pId;
                    newGraph.baseGraph.polygons.Add(p.id, p);
                    newGraph.baseGraph.pId++;
                }               

                foreach (gEdge e in g.edges)
                {
                    newGraph.AddEdge(e);
                }
            }
            newGraph.baseGraph = new Graph(newGraph.baseGraph.polygons.Values.ToList());
            return newGraph;
        }
        #endregion

        #region Internal Methods

        internal List<gEdge> VisibilityAnalysis(Graph baseGraph, List<gVertex> vertices, bool reducedGraph, bool halfScan)
        {
            List<gEdge> visibleEdges = new List<gEdge>();

            foreach (gVertex v in vertices)
            {
                foreach (gVertex v2 in VisibleVertices(v, baseGraph, null, null, null, halfScan, reducedGraph))
                {
                    gEdge newEdge = new gEdge(v, v2);
                    if (!visibleEdges.Contains(newEdge)) { visibleEdges.Add(newEdge); }
                }
            }

            return visibleEdges;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="centre"></param>
        /// <param name="baseGraph"></param>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <param name="singleVertices"></param>
        /// <param name="scan"></param>
        /// <returns name="visibleVertices">List of vertices visible from the analysed vertex</returns>
        internal static List<gVertex> VisibleVertices(
            gVertex centre,
            Graph baseGraph,
            gVertex origin = null,
            gVertex destination = null,
            List<gVertex> singleVertices = null,
            bool halfScan = true,
            bool reducedGraph = true)
        {
            #region Initialize variables and sort vertices
            List<gEdge> edges = baseGraph.edges;
            List<gVertex> vertices = baseGraph.vertices;


            if (origin != null) { vertices.Add(origin); }
            if (destination != null) { vertices.Add(destination); }
            if (singleVertices != null) { vertices.AddRange(singleVertices); }


            gVertex maxVertex = vertices.OrderByDescending(v => v.DistanceTo(centre)).First();
            double maxDistance = centre.DistanceTo(maxVertex) * 1.5;
            //vertices = vertices.OrderBy(v => Point.RadAngle(centre.point, v.point)).ThenBy(v => centre.DistanceTo(v)).ToList();
            vertices = gVertex.OrderByRadianAndDistance(vertices, centre);

            #endregion

            #region Initialize openEdges
            //Initialize openEdges with any intersection edges on the half line 
            //from centre to maxDistance on the XAxis
            List<EdgeKey> openEdges = new List<EdgeKey>();
            double xMax = Math.Abs(centre.X) * maxDistance;
            gEdge halfEdge = gEdge.ByStartVertexEndVertex(centre, gVertex.ByCoordinates(xMax, centre.Y, centre.Z));
            foreach (gEdge e in edges)
            {
                if (e.Contains(centre)) { continue; }
                if (EdgeIntersect(halfEdge, e))
                {
                    if (e.StartVertex.OnEdge(halfEdge)) { continue; }
                    if (e.EndVertex.OnEdge(halfEdge)) { continue; }
                    EdgeKey k = new EdgeKey(halfEdge, e);
                    Core.List.AddItemSorted(openEdges, k);
                }
            }
           
            #endregion

            List<gVertex> visibleVertices = new List<gVertex>();
            gVertex prev = null;
            bool prevVisible = false;
            foreach (gVertex vertex in vertices)
            {
                if (vertex.Equals(centre) || vertex.Equals(prev)) { continue; }// v == to centre or to previous when updating graph
                //Check only half of vertices as eventually they will become 'v'
                if (halfScan && gVertex.RadAngle(centre, vertex) > Math.PI) { break; }
                //Removing clock wise edges incident on v
                if (openEdges.Count > 0 && baseGraph.graph.ContainsKey(vertex))
                {
                    foreach (gEdge edge in baseGraph.graph[vertex])
                    {
                        int orientation = gVertex.Orientation(centre, vertex, edge.GetVertexPair(vertex));

                        if (orientation == -1)
                        {
                            EdgeKey k = new EdgeKey(centre, vertex, edge);
                            int index = Core.List.Bisect(openEdges, k) - 1;
                            index = (index < 0) ? openEdges.Count - 1 : index;
                            if (openEdges.Count > 0 && openEdges.ElementAt(index).Equals(k))
                            {
                                openEdges.RemoveAt(index);
                            }
                        }
                    }
                }

                //Checking if p is visible from p.
                bool isVisible = false;

                //No collinear vertices
                if (prev == null || gVertex.Orientation(centre, prev, vertex) != 0 || !prev.OnEdge(centre, vertex))
                {
                    if (openEdges.Count == 0)
                    {
                        isVisible = true;
                    }
                    else if (!EdgeIntersect(centre, vertex, openEdges[0].Edge))
                    {
                        isVisible = true;
                    }
                }
                //For collinear vertices, if previous was not visible, vertex is not either
                else if (!prevVisible)
                {
                    isVisible = false;
                }
                //For collinear vertices, if prev was visible need to check that
                //the edge from prev to vertex does not intersect with any open edge
                else
                {
                    isVisible = true;
                    foreach (EdgeKey k in openEdges)
                    {
                        //if (!k.edge.Contains(prev) && EdgeIntersect(prev, vertex, k.edge))
                        if (EdgeIntersect(prev, vertex, k.Edge) && !k.Edge.Contains(prev))
                        {
                            isVisible = false;
                            break;
                        }
                    }
                    // If visible (doesn't intersect any open edge) and edge 'prev-vertex'
                    // is in any polygon, vertex is visible if it belongs to a external boundary
                    if (isVisible && EdgeInPolygon(prev, vertex, baseGraph, maxDistance))
                    {
                        isVisible = IsBoundaryVertex(vertex, baseGraph);
                    }

                    // If still visible (not inside polygon or is boundary vertex),
                    // if not on 'centre-prev' edge means there is a gap between prev and vertex
                    if (isVisible && !vertex.OnEdge(centre, prev))
                    {
                        isVisible = !IsBoundaryVertex(vertex, baseGraph);
                    }
                }

                //If vertex is visible and centre belongs to any polygon, checks
                //if the visible edge is interior to its polygon
                if (isVisible && centre.polygonId >= 0 && !baseGraph.GetAdjecentVertices(centre).Contains(vertex))
                {
                    if (IsBoundaryVertex(centre, baseGraph) && IsBoundaryVertex(vertex, baseGraph))
                    {
                        isVisible = EdgeInPolygon(centre, vertex, baseGraph, maxDistance);
                    }
                    else
                    {
                        isVisible = !EdgeInPolygon(centre, vertex, baseGraph, maxDistance);
                    }
                }


                prev = vertex;
                prevVisible = isVisible;


                if (isVisible)
                {
                    // Check reducedGraph if vertices belongs to different polygons
                    if (reducedGraph && centre.polygonId != vertex.polygonId) 
                    {
                        bool isOriginExtreme =  true;
                        bool isTargetExtreme = true;
                        // For reduced graphs, it is checked if the edge is extrem or not.
                        // For an edge to be extreme, the edges coincident at the start and end vertex
                        // will have the same orientation (both clock or counter-clock wise)
                        
                        // Vertex belongs to a polygon
                        if (centre.polygonId >= 0 && !IsBoundaryVertex(centre, baseGraph))
                        {
                            var orientationsOrigin = baseGraph.GetAdjecentVertices(centre).Select(otherVertex => gVertex.Orientation(vertex, centre, otherVertex)).ToList();
                            isOriginExtreme = orientationsOrigin.All(o => o == orientationsOrigin.First());
                        }

                        if(vertex.polygonId >= 0 && !IsBoundaryVertex(vertex, baseGraph))
                        {
                            var orientationsTarget = baseGraph.GetAdjecentVertices(vertex).Select(otherVertex => gVertex.Orientation(centre, vertex, otherVertex)).ToList();
                            isTargetExtreme = orientationsTarget.All(o => o == orientationsTarget.First());
                        }

                        if(isTargetExtreme && isOriginExtreme) { visibleVertices.Add(vertex); }
                    }
                    else
                    {
                        visibleVertices.Add(vertex);
                    }
                }

                if (baseGraph.Contains(vertex))
                {
                    foreach (gEdge e in baseGraph.graph[vertex])
                    {
                        if (!e.Contains(centre) && gVertex.Orientation(centre, vertex, e.GetVertexPair(vertex)) == 1)
                        {
                            EdgeKey k = new EdgeKey(centre, vertex, e);
                            Core.List.AddItemSorted(openEdges, k);
                        }
                    }
                }
            }

            return visibleVertices;
        }

        internal static bool EdgeIntersect(gEdge halfEdge, gEdge edge)
        {
            //For simplicity, it only takes into acount the 2d projection to the xy plane,
            //so the result will be based on a porjection even if points have z values.
            bool intersects = EdgeIntersectProjection(
                halfEdge.StartVertex,
                halfEdge.EndVertex,
                edge.StartVertex,
                edge.EndVertex,
                "xy");

            return intersects;
        }

        internal static bool EdgeIntersect(gVertex start, gVertex end, gEdge edge)
        {
            //For simplicity, it only takes into acount the 2d projection to the xy plane,
            //so the result will be based on a porjection even if points have z values.
            bool intersects = EdgeIntersectProjection(
                start,
                end,
                edge.StartVertex,
                edge.EndVertex,
                "xy");

            return intersects;
        }

        internal static bool EdgeIntersectProjection(
            gVertex p1,
            gVertex q1,
            gVertex p2,
            gVertex q2,
            string plane = "xy")
        {
            //For more details https://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/

            int o1 = gVertex.Orientation(p1, q1, p2, plane);
            int o2 = gVertex.Orientation(p1, q1, q2, plane);
            int o3 = gVertex.Orientation(p2, q2, p1, plane);
            int o4 = gVertex.Orientation(p2, q2, q1, plane);

            //General case
            if (o1 != o2 && o3 != o4) { return true; }

            //Special Cases
            // p1, q1 and p2 are colinear and p2 lies on segment p1q1
            if (o1 == 0 && gVertex.OnEdgeProjection(p1, p2, q1, plane)) { return true; }

            // p1, q1 and p2 are colinear and q2 lies on segment p1q1
            if (o2 == 0 && gVertex.OnEdgeProjection(p1, q2, q1, plane)) { return true; }

            // p2, q2 and p1 are colinear and p1 lies on segment p2q2
            if (o3 == 0 && gVertex.OnEdgeProjection(p2, p1, q2, plane)) { return true; }

            // p2, q2 and q1 are colinear and q1 lies on segment p2q2
            if (o4 == 0 && gVertex.OnEdgeProjection(p2, q1, q2, plane)) { return true; }

            return false; //Doesn't fall on any of the above cases


        }

        internal static bool EdgeInPolygon(gVertex v1, gVertex v2, Graph graph, double maxDistance)
        {
            //Not on the same polygon
            if (v1.polygonId != v2.polygonId) { return false; }
            //At least one doesn't belong to any polygon
            if (v1.polygonId == -1 || v2.polygonId == -1) { return false; }
            gVertex midVertex = gVertex.MidVertex(v1, v2);
            return VertexInPolygon(midVertex, graph.polygons[v1.polygonId].edges, maxDistance);
            //return DSVertexInPolygon(midVertex, graph.polygons[v1.polygonId].vertices);
        }

        internal static bool IsBoundaryVertex(gVertex vertex, Graph graph)
        {
            return (vertex.polygonId < 0) ? false : graph.polygons[vertex.polygonId].isBoundary;
        }

        public static bool VertexInPolygon(gVertex v1, List<gEdge> polygonEdges, double maxDistance)
        {
            gVertex v2 = gVertex.ByCoordinates(v1.X + maxDistance, v1.Y, v1.Z);
            gEdge ray = gEdge.ByStartVertexEndVertex(v1, v2);
            gVertex coincident = null;
            int intersections = 0;
            bool co_flag = false;
            int co_dir = 0;
            foreach (gEdge edge in polygonEdges)
            {
                //gVertex above or below edge
                if (v1.Y < edge.StartVertex.Y && v1.Y < edge.EndVertex.Y) { continue; }
                if (v1.Y > edge.StartVertex.Y && v1.Y > edge.EndVertex.Y) { continue; }
                //Vertices colinear to v1
                gBase intersection = ray.Intersection(edge);
                if (intersection != null)
                {
                    if (intersection.GetType() == typeof(gEdge))
                    {
                        gEdge edgeIntesection = (gEdge)intersection;
                        return edgeIntesection.StartVertex.OnEdge(edge) || edgeIntesection.EndVertex.OnEdge(edge);
                    }
                    else
                    {
                        gVertex vertexIntersection = (gVertex)intersection;
                        if (edge.Contains(vertexIntersection))
                        {
                            intersections += intersection.Equals(coincident) ? 0 : 1;
                            coincident = vertexIntersection;
                        }
                        else
                        {
                            intersections += 1;
                        }
                    }
                    

                }
            }

            //If intersections is odd, returns true, false otherwise
            return (intersections % 2 == 0) ? false : true;
        }
        
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds specific lines as gEdges to the visibility graph
        /// </summary>
        /// <param name="visibilityGraph">VisibilityGraph Graph</param>
        /// <param name="lines">Lines to add as new gEdges</param>
        /// <returns></returns>
        //[NodeCategory("Action")]
        //public static VisibilityGraph AddEdges(VisibilityGraph visibilityGraph, List<DSLine> lines)
        //{
        //    //TODO: implement Dynamo' Trace 
        //    if (lines == null) { throw new NullReferenceException("lines"); }
        //    List<DSPoint> singleVertices = new List<DSPoint>();
        //    List<gEdge> singleEdges = lines.Select(l => gEdge.ByLine(l)).ToList();

        //    foreach (gEdge e in singleEdges)
        //    {
        //        if (!singleVertices.Contains(e.StartVertex.point)) { singleVertices.Add(e.StartVertex.point); }
        //        if (!singleVertices.Contains(e.EndVertex.point)) { singleVertices.Add(e.EndVertex.point); }
        //    }
        //    VisibilityGraph updatedGraph = AddVertices(visibilityGraph, singleVertices);

        //    foreach (gEdge e in singleEdges) { updatedGraph.AddEdge(e); }

        //    return updatedGraph;
        //}

        /// <summary>
        /// Adds specific points as gVertices to the VisibilityGraph Graph
        /// </summary>
        /// <param name="visibilityGraph">VisibilityGraph Graph</param>
        /// <param name="points">Points to add as gVertices</param>
        /// <returns></returns>
        //[NodeCategory("Action")]
        //public static VisibilityGraph AddVertices(VisibilityGraph visibilityGraph, List<DSPoint> points, bool reducedGraph = true)
        //{
        //    //TODO: Seems that original graph gets updated as well
        //    if (points == null) { throw new NullReferenceException("points"); }

        //    VisibilityGraph newVisGraph = (VisibilityGraph)visibilityGraph.Clone();
        //    List<gVertex> singleVertices = new List<gVertex>();

        //    foreach (DSPoint p in points)
        //    {
        //        gVertex newVertex = gVertex.ByCoordinates(p.X, p.Y, p.Z);
        //        if (newVisGraph.Contains(newVertex)) { continue; }
        //        gEdge closestEdge = newVisGraph.baseGraph.edges.OrderBy(e => e.DistanceTo(newVertex)).First();

        //        if (closestEdge.DistanceTo(newVertex) > 0)
        //        {
        //            singleVertices.Add(newVertex);
        //        }
        //        else if (Point.OnLineProjection(closestEdge.StartVertex.point, p, closestEdge.EndVertex.point))
        //        {
        //            newVertex.polygonId = closestEdge.StartVertex.polygonId;
        //            newVisGraph.baseGraph.polygons[newVertex.polygonId] = newVisGraph.baseGraph.polygons[newVertex.polygonId].AddVertex(newVertex, closestEdge);
        //            singleVertices.Add(newVertex);
        //        }
        //    }
        //    //newVisGraph.baseGraph = new Graph(newVisGraph.polygons.Values.ToList());
        //    foreach (gVertex centre in singleVertices)
        //    {
        //        foreach (gVertex v in VisibleVertices(centre, newVisGraph.baseGraph, null, null, singleVertices, false, reducedGraph))
        //        {
        //            newVisGraph.AddEdge(new gEdge(centre, v));
        //        }
        //    }

        //    return newVisGraph;
        //}

        //[MultiReturn(new[] { "graph", "totalLength", "miliseconds" })]
        //public static Dictionary<string, object> ShortestPath(VisibilityGraph visibilityGraph, DSPoint origin, DSPoint destination)
        //{
        //    Graph shortest;
        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();

        //    if (visibilityGraph == null) { throw new ArgumentNullException("visibilityGraph"); }
        //    if (origin == null) { throw new ArgumentNullException("origin"); }
        //    if (destination == null) { throw new ArgumentNullException("destination"); }

        //    gVertex gOrigin = gVertex.ByCoordinates(origin.X, origin.Y, origin.Z);
        //    gVertex gDestination = gVertex.ByCoordinates(destination.X, destination.Y, destination.Z);

        //    bool containsOrigin = visibilityGraph.Contains(gOrigin);
        //    bool containsDestination = visibilityGraph.Contains(gDestination);
            
        //    if(containsOrigin && containsDestination)
        //    {
        //        shortest = Algorithms.Dijkstra(visibilityGraph, gOrigin, gDestination);
        //    }
        //    else
        //    {
        //        gVertex gO = (!containsOrigin) ? gOrigin : null;
        //        gVertex gD = (!containsDestination) ? gDestination : null;
        //        Graph tempGraph = new Graph();

        //        if (!containsOrigin)
        //        {
        //            foreach (gVertex v in VisibleVertices(gOrigin, visibilityGraph.baseGraph, null, gD, null, false, true))
        //            {
        //                tempGraph.AddEdge(new gEdge(gOrigin, v));
        //            }
        //        }
        //        if (!containsDestination)
        //        {
        //            foreach (gVertex v in VisibleVertices(gDestination, visibilityGraph.baseGraph, gO, null, null, false, true))
        //            {
        //                tempGraph.AddEdge(new gEdge(gDestination, v));
        //            }
        //        }
        //        shortest =  Algorithms.Dijkstra(visibilityGraph, gOrigin, gDestination, tempGraph);
        //    }

        //    sw.Stop();

        //    return new Dictionary<string, object>()
        //    {
        //        {"graph", shortest },
        //        {"totalLength", shortest.edges.Select(e => e.Length).Sum() },
        //        {"miliseconds", sw.ElapsedMilliseconds}
        //    };


        //}

        #endregion


        
        public new object Clone()
        {
            VisibilityGraph newGraph = new VisibilityGraph()
            {
                graph = new Dictionary<gVertex, List<gEdge>>(),
                edges = new List<gEdge>(this.edges),
                polygons = new Dictionary<int, gPolygon>(this.polygons),
                baseGraph = (Graph)this.baseGraph.Clone()
            };

            foreach (var item in this.graph)
            {
                newGraph.graph.Add(item.Key, new List<gEdge>(item.Value));
            }

            return newGraph;
        }
        
    }

    /// <summary>
    /// VisibilityGraph graph's EdgeKey class to create a tree data structure.
    /// </summary>
    public class EdgeKey : IComparable<EdgeKey>
    {
        internal gVertex Centre { get; private set; }
        internal gVertex Vertex { get; private set; }
        internal gEdge Edge { get; private set; }
        internal gEdge RayEdge { get; private set; }

        internal EdgeKey(gEdge rayEdge, gEdge e)
        {
            RayEdge = rayEdge;
            Edge = e;
            Centre = RayEdge.StartVertex;
            Vertex = RayEdge.EndVertex;
        }
        
        internal EdgeKey(gVertex centre, gVertex end, gEdge e)
        {
            Centre = centre;
            Vertex = end;
            Edge = e;
            RayEdge = gEdge.ByStartVertexEndVertex(centre, end);
        }

        internal static double DistanceToIntersection(gVertex centre, gVertex maxVertex, gEdge e)
        {
            var centreProj = gVertex.ByCoordinates(centre.X, centre.Y, 0);
            var maxProj = gVertex.ByCoordinates(maxVertex.X, maxVertex.Y, 0);
            var startProj = gVertex.ByCoordinates(e.StartVertex.X, e.StartVertex.Y, 0);
            var endProj = gVertex.ByCoordinates(e.EndVertex.X, e.EndVertex.Y, 0);
            gEdge rayEdge = gEdge.ByStartVertexEndVertex(centreProj, maxProj);
            gEdge edgeProj = gEdge.ByStartVertexEndVertex(startProj, endProj);
            gBase intersection = rayEdge.Intersection(edgeProj);
            if(intersection != null && intersection.GetType() == typeof(gVertex))
            {
                return centre.DistanceTo((gVertex)intersection);
            }
            else
            {
                return 0;
            }
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
            return Centre.GetHashCode() ^ Vertex.GetHashCode();
        }


        /// <summary>
        /// Implementation of IComparable interaface
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(EdgeKey other)
        {
            if (other == null) { return 1; }
            if (Edge.Equals(other.Edge)) { return 1; }
            if (!VisibilityGraph.EdgeIntersect(RayEdge, other.Edge)){ return -1; }

            double selfDist = DistanceToIntersection(Centre, Vertex, Edge);
            double otherDist = DistanceToIntersection(Centre, Vertex, other.Edge);

            if(selfDist > otherDist) { return 1; }
            else if(selfDist < otherDist) { return -1; }
            else
            {
                gVertex sameVertex = null;
                if (other.Edge.Contains(Edge.StartVertex)) { sameVertex = Edge.StartVertex; }
                else if (other.Edge.Contains(Edge.EndVertex)) { sameVertex = Edge.EndVertex; }
                double aslf = gVertex.ArcRadAngle( Vertex, Centre, Edge.GetVertexPair(sameVertex));
                double aot = gVertex.ArcRadAngle( Vertex, Centre, other.Edge.GetVertexPair(sameVertex));

                if(aslf < aot) { return -1; }
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
            return String.Format("EdgeKey: (gEdge={0}, centre={1}, vertex={2})", Edge.ToString(), Centre.ToString(), Vertex.ToString());
        }
    }
}