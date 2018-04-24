#region namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphical.Base;
using Graphical.Geometry;
using DSVector = Autodesk.DesignScript.Geometry.Vector;
using DSPoint = Autodesk.DesignScript.Geometry.Point;
using DSLine = Autodesk.DesignScript.Geometry.Line;
using DSPolygon = Autodesk.DesignScript.Geometry.Polygon;
using System.Diagnostics;
using Autodesk.DesignScript.Runtime;
using Dynamo.Graph.Nodes;
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
        [IsVisibleInDynamoLibrary(false)]
        internal VisibilityGraph() : base()
        {
            baseGraph = new Graph();
        }

        internal VisibilityGraph(Graph _baseGraph, bool reducedGraph) : base()
        {
            baseGraph = _baseGraph;

            List<gEdge> resultEdges = VisibilityAnalysis(baseGraph, baseGraph.vertices, reducedGraph);

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
        public static VisibilityGraph ByPolygons(DSPolygon[] polygons, bool reducedGraph = true)
        {
            List<gPolygon> gPolygons = FromPolygons(polygons, false);
            Graph baseGraph = new Graph(gPolygons);
            VisibilityGraph g = new VisibilityGraph(baseGraph, reducedGraph);
            return g;
        }

        /// <summary>
        /// Creates a visibility graph by a set of polygons and boundaries.
        /// </summary>
        /// <param name="polygons">Set of internal polygons</param>
        /// <param name="boundaries">Set of boundary polygons. These must not be present on the internal polygons list.</param>
        /// <returns name="visibilityGraph">VisibilityGraph graph</returns>
        public static VisibilityGraph ByPolygonsAndBoundaries(DSPolygon[] boundaries, [DefaultArgument("{}")]DSPolygon[] polygons = null, bool reducedGraph = true)
        {
            List<gPolygon> gPolygons = FromPolygons(boundaries, true);
            if(polygons.Any())
            {
                gPolygons.AddRange(FromPolygons(polygons, false));
            }
            Graph baseGraph = new Graph(gPolygons);
            return new VisibilityGraph(baseGraph, reducedGraph);
        }


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

        internal List<gEdge> VisibilityAnalysis(Graph baseGraph, List<gVertex> vertices, bool reducedGraph)
        {
            List<gEdge> visibleEdges = new List<gEdge>();

            foreach (gVertex v in vertices)
            {
                foreach (gVertex v2 in VisibleVertices(v, baseGraph, null, null, null, "half", reducedGraph))
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
            string scan = "full",
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
            using (DSLine halfLine = DSLine.ByStartPointDirectionLength(centre.point, DSVector.XAxis(), maxDistance))
            {
                foreach (gEdge e in edges)
                {
                    if (e.Contains(centre)) { continue; }
                    if (EdgeIntersect(halfLine, e))
                    {
                        if (gVertex.OnLine(e.StartVertex, halfLine)) { continue; }
                        if (gVertex.OnLine(e.EndVertex, halfLine)) { continue; }
                        EdgeKey k = new EdgeKey(halfLine, e);
                        Core.List.AddItemSorted(openEdges, k);
                    }
                }
            } //
            #endregion

            List<gVertex> visibleVertices = new List<gVertex>();
            gVertex prev = null;
            bool prevVisible = false;
            foreach (gVertex vertex in vertices)
            {
                if (vertex.Equals(centre) || vertex.Equals(prev)) { continue; }// v == to centre or to previous when updating graph
                //Check only half of vertices as eventually they will become 'v'
                if (scan == "half" && gVertex.RadAngle(centre, vertex) > Math.PI) { break; }
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
                if (prev == null || gVertex.Orientation(centre, prev, vertex) != 0 || !gVertex.OnLine(centre, prev, vertex))
                {
                    if (openEdges.Count == 0)
                    {
                        isVisible = true;
                    }
                    else if (!EdgeIntersect(centre, vertex, openEdges[0].edge))
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
                //the edge from prev to p does not intersect with any open edge
                else
                {
                    isVisible = true;
                    foreach (EdgeKey k in openEdges)
                    {
                        //if (!k.edge.Contains(prev) && EdgeIntersect(prev, vertex, k.edge))
                        if (EdgeIntersect(prev, vertex, k.edge))
                        {
                            isVisible = false;
                            break;
                        }
                    }
                    if (isVisible && EdgeInPolygon(prev, vertex, baseGraph, maxDistance))
                    {
                        isVisible = false;
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

        internal static bool EdgeIntersect(DSLine halfLine, gEdge edge)
        {
            //For simplicity, it only takes into acount the 2d projection to the xy plane,
            //so the result will be based on a porjection even if points have z values.
            bool intersects = EdgeIntersectProjection(
                halfLine.StartPoint,
                halfLine.EndPoint,
                edge.StartVertex.point,
                edge.EndVertex.point,
                "xy");

            return intersects;
        }

        internal static bool EdgeIntersect(gVertex start, gVertex end, gEdge edge)
        {
            //For simplicity, it only takes into acount the 2d projection to the xy plane,
            //so the result will be based on a porjection even if points have z values.
            bool intersects = EdgeIntersectProjection(
                start.point,
                end.point,
                edge.StartVertex.point,
                edge.EndVertex.point,
                "xy");

            return intersects;
        }

        internal static bool EdgeIntersectProjection(
            DSPoint p1,
            DSPoint q1,
            DSPoint p2,
            DSPoint q2,
            string plane = "xy")
        {
            //For more details https://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/

            int o1 = Point.Orientation(p1, q1, p2, plane);
            int o2 = Point.Orientation(p1, q1, q2, plane);
            int o3 = Point.Orientation(p2, q2, p1, plane);
            int o4 = Point.Orientation(p2, q2, q1, plane);

            //General case
            if (o1 != o2 && o3 != o4) { return true; }

            //Special Cases
            // p1, q1 and p2 are colinear and p2 lies on segment p1q1
            if (o1 == 0 && Point.OnLineProjection(p1, p2, q1, plane)) { return true; }

            // p1, q1 and p2 are colinear and q2 lies on segment p1q1
            if (o2 == 0 && Point.OnLineProjection(p1, q2, q1, plane)) { return true; }

            // p2, q2 and p1 are colinear and p1 lies on segment p2q2
            if (o3 == 0 && Point.OnLineProjection(p2, p1, q2, plane)) { return true; }

            // p2, q2 and q1 are colinear and q1 lies on segment p2q2
            if (o4 == 0 && Point.OnLineProjection(p2, q1, q2, plane)) { return true; }

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

        internal static bool VertexInPolygon(gVertex v1, List<gEdge> polygonEdges, double maxDistance)
        {
            gVertex v2 = gVertex.ByCoordinates(v1.X + maxDistance, v1.Y, v1.Z);
            int intersections = 0;
            bool co_flag = false;
            int co_dir = 0;
            foreach (gEdge edge in polygonEdges)
            {
                //gVertex above or below edge
                if (v1.Y < edge.StartVertex.Y && v1.Y < edge.EndVertex.Y) { continue; }
                if (v1.Y > edge.StartVertex.Y && v1.Y > edge.EndVertex.Y) { continue; }
                //Vertices colinear to v1
                bool co0 = gVertex.Orientation(v1, edge.StartVertex, v2) == 0 && (edge.StartVertex.X > v1.X);
                bool co1 = gVertex.Orientation(v1, edge.EndVertex, v2) == 0 && (edge.EndVertex.X > v1.X);
                gVertex co_vertex = (co0) ? edge.StartVertex : edge.EndVertex;
                if (co0 || co1)
                {
                    co_dir += (edge.GetVertexPair(co_vertex).Y > v1.Y) ? 1 : -1;
                    if (co_flag)
                    {
                        intersections += (co_dir == 0) ? 1 : 0;
                        co_flag = false;
                        co_dir = 0;
                    }
                    else
                    {
                        co_flag = true;
                    }
                }
                else if (EdgeIntersect(v1, v2, edge))
                {
                    intersections += 1;
                }
            }

            //If intersections is odd, returns true, false otherwise
            return (intersections % 2 == 0) ? false : true;
        }

        internal static bool DSVertexInPolygon(gVertex v1, List<gVertex> polygonVertices)
        {
            using (DSPolygon polygon = DSPolygon.ByPoints(polygonVertices.Select(v => v.point)))
            {
                return polygon.ContainmentTest(v1.point);
            }
        } 
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds specific lines as gEdges to the visibility graph
        /// </summary>
        /// <param name="visibilityGraph">VisibilityGraph Graph</param>
        /// <param name="lines">Lines to add as new gEdges</param>
        /// <returns></returns>
        [NodeCategory("Action")]
        public static VisibilityGraph AddEdges(VisibilityGraph visibilityGraph, List<DSLine> lines)
        {
            //TODO: implement Dynamo' Trace 
            if (lines == null) { throw new NullReferenceException("lines"); }
            List<DSPoint> singleVertices = new List<DSPoint>();
            List<gEdge> singleEdges = lines.Select(l => gEdge.ByLine(l)).ToList();

            foreach (gEdge e in singleEdges)
            {
                if (!singleVertices.Contains(e.StartVertex.point)) { singleVertices.Add(e.StartVertex.point); }
                if (!singleVertices.Contains(e.EndVertex.point)) { singleVertices.Add(e.EndVertex.point); }
            }
            VisibilityGraph updatedGraph = AddVertices(visibilityGraph, singleVertices);

            foreach (gEdge e in singleEdges) { updatedGraph.AddEdge(e); }

            return updatedGraph;
        }

        /// <summary>
        /// Adds specific points as gVertices to the VisibilityGraph Graph
        /// </summary>
        /// <param name="visibilityGraph">VisibilityGraph Graph</param>
        /// <param name="points">Points to add as gVertices</param>
        /// <returns></returns>
        [NodeCategory("Action")]
        public static VisibilityGraph AddVertices(VisibilityGraph visibilityGraph, List<DSPoint> points, bool reducedGraph = true)
        {
            //TODO: Seems that original graph gets updated as well
            if (points == null) { throw new NullReferenceException("points"); }

            VisibilityGraph newVisGraph = (VisibilityGraph)visibilityGraph.Clone();
            List<gVertex> singleVertices = new List<gVertex>();

            foreach (DSPoint p in points)
            {
                gVertex newVertex = gVertex.ByPoint(p);
                if (newVisGraph.Contains(newVertex)) { continue; }
                gEdge closestEdge = newVisGraph.baseGraph.edges.OrderBy(e => e.DistanceTo(p)).First();

                if (closestEdge.DistanceTo(p) > 0)
                {
                    singleVertices.Add(newVertex);
                }
                else if (Point.OnLineProjection(closestEdge.StartVertex.point, p, closestEdge.EndVertex.point))
                {
                    newVertex.polygonId = closestEdge.StartVertex.polygonId;
                    newVisGraph.baseGraph.polygons[newVertex.polygonId] = newVisGraph.baseGraph.polygons[newVertex.polygonId].AddVertex(newVertex, closestEdge);
                    singleVertices.Add(newVertex);
                }
            }
            //newVisGraph.baseGraph = new Graph(newVisGraph.polygons.Values.ToList());
            foreach (gVertex centre in singleVertices)
            {
                foreach (gVertex v in VisibleVertices(centre, newVisGraph.baseGraph, null, null, singleVertices, "full", reducedGraph))
                {
                    newVisGraph.AddEdge(new gEdge(centre, v));
                }
            }

            return newVisGraph;
        }

        public static Graph ShortestPath(VisibilityGraph visibilityGraph, DSPoint origin, DSPoint destination)
        {
            if (visibilityGraph == null) { throw new ArgumentNullException("visibilityGraph"); }
            if (origin == null) { throw new ArgumentNullException("origin"); }
            if (destination == null) { throw new ArgumentNullException("destination"); }

            gVertex gOrigin = gVertex.ByPoint(origin);
            gVertex gDestination = gVertex.ByPoint(destination);

            bool containsOrigin = visibilityGraph.Contains(gOrigin);
            bool containsDestination = visibilityGraph.Contains(gDestination);
            
            if(containsOrigin && containsDestination)
            {
                return Algorithms.Dijkstra(visibilityGraph, gOrigin, gDestination);
            }

            gVertex gO = (!containsOrigin) ? gOrigin : null;
            gVertex gD = (!containsDestination) ? gDestination : null;
            Graph tempGraph = new Graph();

            if (!containsOrigin)
            {
                foreach(gVertex v in VisibleVertices(gOrigin, visibilityGraph.baseGraph, null, gD, null, "full", true))
                {
                    tempGraph.AddEdge(new gEdge(gOrigin, v));
                }
            }
            if (!containsDestination)
            {
                foreach(gVertex v in VisibleVertices(gDestination, visibilityGraph.baseGraph, gO, null, null, "full", true))
                {
                    tempGraph.AddEdge(new gEdge(gDestination, v));
                }
            }

            return Algorithms.Dijkstra(visibilityGraph, gOrigin, gDestination, tempGraph);

        }

        #endregion


        [IsVisibleInDynamoLibrary(false)]
        public new object Clone()
        {
            VisibilityGraph newGraph = new VisibilityGraph()
            {
                graph = new Dictionary<gVertex, List<gEdge>>(this.graph),
                edges = new List<gEdge>(this.edges),
                polygons = new Dictionary<int, gPolygon>(this.polygons),
                baseGraph = (Graph)this.baseGraph.Clone()
            };
            return newGraph;
        }
        
    }

    /// <summary>
    /// VisibilityGraph graph's EdgeKey class to create a tree data structure.
    /// </summary>
    [IsVisibleInDynamoLibrary(false)]
    public class EdgeKey : IComparable<EdgeKey>
    {
        internal gVertex centre { get; private set; }
        internal gVertex vertex { get; private set; }
        internal gEdge edge { get; private set; }
        internal DSLine LineGeometry { get; private set; }

        internal EdgeKey(DSLine line, gEdge e)
        {
            LineGeometry = line;
            edge = e;
            centre = gVertex.ByPoint(line.StartPoint);
            vertex = gVertex.ByPoint(line.EndPoint);
        }
        
        internal EdgeKey(gVertex _centre, gVertex end, gEdge e)
        {
            centre = _centre;
            vertex = end;
            edge = e;
            LineGeometry = DSLine.ByStartPointEndPoint(_centre.point, end.point);
        }

        internal static double DistanceToInteserction(gVertex centre, gVertex end, gEdge e)
        {
            //This intersetion is using the 2d representation of the vertices and edge,
            //as using the 3D geometry might cause wrong results.

            using (DSPoint p1 = centre.GetProjectionOnPlane())
            using (DSPoint p2 = end.GetProjectionOnPlane())
            using (DSLine line = DSLine.ByStartPointEndPoint(p1, p2))
            using (DSLine edgeLine = e.GetProjectionOnPlane())
            {
                bool doesIntersect = line.DoesIntersect(edgeLine);
                double dot = Math.Abs(line.Direction.Normalized().Dot(edgeLine.Direction.Normalized()));
                //If they intersect or not coincident(parallel)
                if (doesIntersect && dot != 1)
                {
                    using (DSPoint intersection = line.Intersect(edgeLine).First() as DSPoint)
                    {
                        return centre.DistanceTo(intersection);
                    }
                }
                else
                {
                    return 0;
                }
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
            return edge.Equals(k.edge);
        }

        /// <summary>
        /// Override of GetHashCode method
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return centre.GetHashCode() ^ vertex.GetHashCode();
        }


        /// <summary>
        /// Implementation of IComparable interaface
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(EdgeKey other)
        {
            if (other == null) { return 1; }
            if (edge.Equals(other.edge)) { return 1; }
            if (!VisibilityGraph.EdgeIntersect(LineGeometry, other.edge)){ return -1; }

            double selfDist = DistanceToInteserction(centre, vertex, edge);
            double otherDist = DistanceToInteserction(centre, vertex, other.edge);

            if(selfDist > otherDist) { return 1; }
            else if(selfDist < otherDist) { return -1; }
            else
            {
                gVertex sameVertex = null;
                if (other.edge.Contains(edge.StartVertex)) { sameVertex = edge.StartVertex; }
                else if (other.edge.Contains(edge.EndVertex)) { sameVertex = edge.EndVertex; }
                double aslf = Point.ArcRadAngle( vertex.point, centre.point, edge.GetVertexPair(sameVertex).point);
                double aot = Point.ArcRadAngle( vertex.point, centre.point, other.edge.GetVertexPair(sameVertex).point);

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
            return String.Format("EdgeKey: (gEdge={0}, centre={1}, vertex={2})", edge.ToString(), centre.ToString(), vertex.ToString());
        }
    }
}