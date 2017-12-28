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
#endregion

namespace Graphical.Graphs
{
    /// <summary>
    /// Construction of Visibility Graph
    /// </summary>
    public class Visibility
    {
        #region Variables
        internal Graph graph { get; set; }
        public Graph visGraph { get; set; }

        const int Infinite = 10000;

        #endregion

        internal Visibility(Graph analysisGraph)
        {
            graph = analysisGraph;
            visGraph = new Graph();

            List<Edge> resultEdges = VisibilityAnalysis(graph, graph.vertices);

            foreach (Edge edge in resultEdges)
            {
                visGraph.AddEdge(edge);
            }

        }

        public static Visibility ByPolygons(DSPolygon[] polygons)
        {
            Graph graph = Graph.ByPolygons(polygons);
            return new Visibility(graph);
        }

        internal List<Edge> VisibilityAnalysis(Graph graph, List<Vertex> vertices)
        {
            int totalVertices = vertices.Count;
            List<Edge> visibleEdges = new List<Edge>();

            foreach (Vertex v in vertices)
            {
                foreach (Vertex v2 in VisibleVertices(v, graph, null, null,"half"))
                {
                    Edge newEdge = new Edge(v, v2);
                    if (!visibleEdges.Contains(newEdge)) { visibleEdges.Add(newEdge); }
                    //visibleEdges.Add(newEdge);
                }
                //break;
            }

            return visibleEdges;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="centre"></param>
        /// <param name="analysisGraph"></param>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <param name="scan"></param>
        /// <returns name="visibleVertices">List of vertices visible from the analysed vertex</returns>
        internal List<Vertex> VisibleVertices(
            Vertex centre,
            Graph analysisGraph,
            Vertex origin = null,
            Vertex destination = null,
            string scan = "full")
        {
            #region Initialize variables and sort vertices
            List<Edge> edges = analysisGraph.edges;
            List<Vertex> vertices = analysisGraph.vertices;
            Vertex maxVertex = vertices.OrderByDescending(v => v.DistanceTo(centre)).First();
            double maxDistance = centre.DistanceTo(maxVertex) * 1.5;

            Debug.Write(maxVertex.ToString());


            if (origin != null) { vertices.Add(origin); }
            if (destination != null) { vertices.Add(destination); }
            vertices = vertices.OrderBy(v => Point.RadAngle(centre.point, v.point)).ThenBy(v => centre.DistanceTo(v)).ToList();
            //return vertices;
            #endregion

            #region Initialize openEdges
            //Initialize openEddges with any intersection edges on the half line 
            //from centre to maxDistance on th XAxis
            List<EdgeKey> openEdges = new List<EdgeKey>();
            DSVector xAxis = DSVector.XAxis();
            using (DSLine halfLine = DSLine.ByStartPointDirectionLength(centre.point, xAxis, maxDistance))
            {
                foreach (Edge e in edges)
                {
                    if (e.Contains(centre)) { continue; }
                    if (EdgeIntersect(halfLine, e))
                    {
                        if (Point.OnLine(e.StartVertex.point, halfLine)) { continue; }
                        if (Point.OnLine(e.EndVertex.point, halfLine)) { continue; }
                        EdgeKey k = new EdgeKey(halfLine, e);
                        Core.List.AddItemSorted(openEdges, k);
                    }

                }
            } //
            #endregion

            List<Vertex> visibleVertices = new List<Vertex>();
            Vertex prev = null;
            bool prevVisible = false;
            foreach(Vertex vertex in vertices)
            {
                if (vertex.Equals(centre)) { continue; }// v == to centre
                //Check only half of vertices as eventually they will become 'v'
                if (scan == "half" && Point.RadAngle(centre.point, vertex.point) > Math.PI) { break; }

                //Removing clock wise edges incident on v
                if(openEdges.Count > 0)
                {
                    foreach(Edge edge in analysisGraph.graph[vertex])
                    {
                        int orientation = Point.Orientation(centre.point, vertex.point, edge.GetVertexPair(vertex).point);
                        if (orientation == -1)
                        {
                            EdgeKey k = new EdgeKey(centre, vertex, edge);
                            int index = Core.List.Bisect(openEdges, k) - 1;
                            index = (index < 0) ? openEdges.Count - 1 : index;
                            if(openEdges.Count > 0 && openEdges.ElementAt(index).Equals(k))
                            {
                                openEdges.RemoveAt(index);
                            }
                        }
                    }
                }

                //Checking if p is visible from p.
                bool isVisible = false;
                //No collinear vertices
                if(prev == null || Point.Orientation(centre.point,prev.point, vertex.point) != 0 || !Point.OnLine(centre.point, prev.point, vertex.point))
                {
                    if(openEdges.Count == 0)
                    {
                        isVisible = true;
                    }else if(!EdgeIntersect(centre, vertex, openEdges[0].edge))
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
                    foreach(EdgeKey k in openEdges)
                    {
                        if(!k.edge.Contains(prev) && EdgeIntersect(prev, vertex, k.edge))
                        {
                            isVisible = false;
                            break;
                        }
                    }
                    if (isVisible && EdgeInPolygon(prev, vertex, analysisGraph, maxDistance))
                    {
                        isVisible = false;
                    }
                }

                //Check if the visible edge is interior to its polygon
                List<Vertex> contains = analysisGraph.GetAdjecentVertices(centre);
                bool c = contains.Contains(vertex);//.Contains(vertex);
                if (isVisible && !analysisGraph.GetAdjecentVertices(centre).Contains(vertex))
                {
                    isVisible = !EdgeInPolygon(centre, vertex, analysisGraph, maxDistance);
                }

                if (isVisible) { visibleVertices.Add(vertex); }

                foreach(Edge e in analysisGraph.graph[vertex])
                {
                    if(!e.Contains(centre) && Vertex.Orientation(centre, vertex, e.GetVertexPair(vertex)) == 1){
                        EdgeKey k = new EdgeKey(centre, vertex, e);
                        Core.List.AddItemSorted(openEdges, k);
                    }
                }
                prev = vertex;
                prevVisible = isVisible;
            }

            return visibleVertices;
        }

        internal static bool EdgeIntersect(DSLine halfLine, Edge edge)
        {
            //bool intersects = true;
            //string[] planes = new string[] { "xy", "xz", "yz" };
            //foreach(string plane in planes)
            //{
            //    intersects = EdgeIntersectProjection(
            //        halfLine.StartPoint,
            //        halfLine.EndPoint, 
            //        edge.StartVertex.point, 
            //        edge.EndVertex.point,
            //        plane);
            //    if (!intersects) { break; }
            //}
            return edge.LineGeometry.DoesIntersect(halfLine);
        }

        internal static bool EdgeIntersect(Vertex start, Vertex end, Edge edge)
        {
            using(DSLine line = DSLine.ByStartPointEndPoint(start.point, end.point))
            {
                return edge.LineGeometry.DoesIntersect(line);
            }
        }

        internal bool EdgeIntersectProjection(
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

        internal bool EdgeInPolygon(Vertex v1, Vertex v2, Graph graph, double maxDistance)
        {
            //Not on the same polygon
            if(v1.polygonId != v2.polygonId) { return false; }
            //At least one doesn´t belong to any polygon
            if(v1.polygonId == -1 || v2.polygonId == -1) { return false; }
            Vertex midVertex = Vertex.MidVertex(v1, v2);
            return VertexInPolygon(midVertex, graph.polygons[v1.polygonId], maxDistance);
        }

        internal bool VertexInPolygon(Vertex v1, List<Edge> polygonEdges, double maxDistance)
        {
            Vertex v2 = Vertex.ByCoordinates(v1.X + maxDistance, v1.Y, v1.Z);
            int intersections = 0;
            bool co_flag = false;
            int co_dir = 0;
            foreach(Edge edge in polygonEdges)
            {
                //Vertex above or below edge
                if(v1.Y < edge.StartVertex.Y && v1.Y < edge.EndVertex.Y) { continue; }
                if(v1.Y > edge.StartVertex.Y && v1.Y > edge.EndVertex.Y) { continue; }
                //Vertices colinear to v1
                bool co0 = Vertex.Orientation(v1, edge.StartVertex, v2) == 0 && (edge.StartVertex.X > v1.X);
                bool co1 = Vertex.Orientation(v1, edge.EndVertex, v2) == 0 && (edge.EndVertex.X > v1.X);
                Vertex co_vertex = (co0) ? edge.StartVertex : edge.EndVertex;
                if(co0 || co1)
                {
                    co_dir += (edge.GetVertexPair(co_vertex).Y > v1.Y) ? 1 : -1;
                    if (co_flag)
                    {
                        intersections += (co_flag) ? 1 : 0;
                        co_flag = false;
                        co_dir = 0;
                    }else
                    {
                        co_flag = true;
                    }
                }else if(EdgeIntersect(v1, v2, edge))
                {
                    intersections += 1;
                }
            }

            //If intersections is odd, returns true, false otherwise
            return (intersections % 2 == 0) ? false : true;
        }
    }

    [IsVisibleInDynamoLibrary(false)]
    public class EdgeKey : IComparable<EdgeKey>
    {
        internal Vertex centre { get; private set; }
        internal Vertex vertex { get; private set; }
        internal Edge edge { get; private set; }
        internal DSLine LineGeometry { get; private set; }

        internal EdgeKey(DSLine line, Edge e)
        {
            LineGeometry = line;
            edge = e;
            centre = Vertex.ByPoint(line.StartPoint);
            vertex = Vertex.ByPoint(line.EndPoint);
        }
        
        internal EdgeKey(Vertex _centre, Vertex end, Edge e)
        {
            centre = _centre;
            vertex = end;
            edge = e;
            LineGeometry = DSLine.ByStartPointEndPoint(_centre.point, end.point);
        }

        internal double DistanceToInteserction(Vertex centre, Vertex end, Edge e)
        {
            using (DSLine line = DSLine.ByStartPointEndPoint(centre.point, end.point))
            {
                bool doesIntersect = line.DoesIntersect(edge.LineGeometry);
                double dot = Math.Abs(line.Direction.Normalized().Dot(edge.LineGeometry.Direction.Normalized()));
                //If they intersect or not coincident(parallel)
                if (doesIntersect && dot != 1)
                {
                    using (DSPoint intersection = line.Intersect(edge.LineGeometry).First() as DSPoint)
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


        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) { return false; }

            EdgeKey k = (EdgeKey)obj;
            return edge.Equals(k.edge);
        }

        

        public int CompareTo(EdgeKey other)
        {
            if (other == null) { return 1; }
            if (edge.Equals(other.edge)) { return 1; }
            if (!Visibility.EdgeIntersect(LineGeometry, other.edge)){ return -1; }

            double selfDist = DistanceToInteserction(centre, vertex, edge);
            double otherDist = DistanceToInteserction(centre, vertex, other.edge);

            if(selfDist > otherDist) { return 1; }
            else if(selfDist < otherDist) { return -1; }
            else
            {
                Vertex sameVertex = null;
                if (other.edge.Contains(edge.StartVertex)) { sameVertex = edge.StartVertex; }
                else if (other.edge.Contains(edge.EndVertex)) { sameVertex = edge.EndVertex; }
                double aslf = Point.RadiansArcStartCentreEnd(centre.point, vertex.point, edge.GetVertexPair(sameVertex).point);
                double aot = Point.RadiansArcStartCentreEnd(centre.point, vertex.point, other.edge.GetVertexPair(sameVertex).point);

                if(aslf < aot) { return -1; }
                else { return 1; }
            }

        }

        public static bool operator <(EdgeKey k1, EdgeKey k2)
        {
            return k1.CompareTo(k2) < 0;
        }

        public static bool operator >(EdgeKey k1, EdgeKey k2)
        {
            return k1.CompareTo(k2) > 0;
        }

        public override string ToString()
        {
            return String.Format("{}(Edge={0}, centre={1}, infVertex={2}", edge.ToString(), centre.ToString(), vertex.ToString());
        }
    }
}