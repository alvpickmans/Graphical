#region namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphical.Geometry;
#endregion

namespace Graphical.Graphs
{
    
    /// <summary>
    /// Representation of a Graph.
    /// Graph contains a Dictionary where
    /// </summary>
    public class Graph : ICloneable
    {
        #region Variables

        /// <summary>
        /// GUID to verify uniqueness of graph when cloned
        /// </summary>
        internal Guid graphID { get; private set; }

        /// <summary>
        /// Polygons dictionary with their Id as dictionary key
        /// </summary>
        internal Dictionary<int, Polygon> polygons = new Dictionary<int, Polygon>();

        /// <summary>
        /// Dictionary with vertex as key and values edges associated with the vertex.
        /// </summary>
        internal Dictionary<Vertex, List<Edge>> graph = new Dictionary<Vertex, List<Edge>>();

        /// <summary>
        /// Graph's vertices
        /// </summary>
        public List<Vertex> vertices { get { return graph.Keys.ToList(); } }

        /// <summary>
        /// Graph's edges
        /// </summary>
        public List<Edge> edges { get; internal set; }

        public List<Polygon> Polygons
        {
            get { return polygons.Values.ToList(); }
        }

        #endregion

        #region Internal Constructors
        public Graph()
        {
            edges = new List<Edge>();
            graphID = Guid.NewGuid();
        }

        public Graph(List<Polygon> polygonList)
        {
            edges = new List<Edge>();
            graphID = Guid.NewGuid();

            // Adding polygons to Graph
            for (int i = 0; i < polygonList.Count; i++)
            {
                var polygon = polygonList[i];
                this.polygons.Add(polygon.Id, polygon);

                polygon.Edges.ForEach(edge => this.AddEdge(edge));
            }
        }

        #endregion

        #region Internal Methods

        internal void ResetEdgesFromPolygons()
        {
            this.edges.Clear();
            this.graph.Clear();

            foreach(Polygon polygon in polygons.Values)
            {
                foreach(Edge edge in polygon.edges)
                {
                    this.AddEdge(edge);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Contains mathod for vertex in graph
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public bool Contains(Vertex vertex)
        {
            return graph.ContainsKey(vertex);
        }

        /// <summary>
        /// Contains method for edges in graph
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool Contains(Edge edge)
        {
            return edges.Contains(edge);
        }

        public List<Edge> GetVertexEdges(Vertex vertex)
        {
            List<Edge> edgesList = new List<Edge>();
            if(graph.TryGetValue(vertex, out edgesList))
            {
                return edgesList;
            }else
            {
                //graph.Add(vertex, new List<Edge>());
                return new List<Edge>();
            }
        }

        public List<Vertex> GetAdjecentVertices(Vertex v)
        {
            return graph[v].Select(edge => edge.GetVertexPair(v)).ToList();
        }

        /// <summary>
        /// Add edge to the analisys graph
        /// </summary>
        /// <param name="edge">New edge</param>
        public void AddEdge(Edge edge)
        {
            List<Edge> startEdgesList = new List<Edge>();
            List<Edge> endEdgesList = new List<Edge>();
            if (graph.TryGetValue(edge.StartVertex, out startEdgesList))
            {
                if (!startEdgesList.Contains(edge)) { startEdgesList.Add(edge); }
            }
            else
            {
                graph.Add(edge.StartVertex, new List<Edge>() { edge });
            }

            if (graph.TryGetValue(edge.EndVertex, out endEdgesList))
            {
                if (!endEdgesList.Contains(edge)) { endEdgesList.Add(edge); }
            }
            else
            {
                graph.Add(edge.EndVertex, new List<Edge>() { edge });
            }
            
            if (!edges.Contains(edge)) { edges.Add(edge); }
        }

        /// <summary>
        /// Computes edges and creates polygons from those connected by vertices.
        /// </summary>
        public void BuildPolygons()
        {
            var computedVertices = new List<Vertex>();
            
            foreach(Vertex v in vertices)
            {
                // If already belongs to a polygon or is not a polygon vertex or already computed
                if( computedVertices.Contains(v) || graph[v].Count > 2) { continue; }

                computedVertices.Add(v);
                Polygon polygon = new Polygon( false);
                
                polygon.AddVertex(v);
                foreach(Edge edge in GetVertexEdges(v))
                {
                    Edge currentEdge = edge;
                    Vertex currentVertex = edge.GetVertexPair(v);
                    while (!polygon.vertices.Contains(currentVertex) || !computedVertices.Contains(currentVertex))
                    {
                        polygon.AddVertex(currentVertex);
                        polygon.edges.Add(currentEdge);

                        var connectedEdges = graph[currentVertex];
                        //It is extreme vertex, polygon not closed
                        if(connectedEdges.Count < 2)
                        {
                            break;
                        }
                        // If just two edges, select the one that is not current nextEdge
                        else if(connectedEdges.Count == 2)
                        {
                            currentEdge = connectedEdges[0].Equals(currentEdge) ? connectedEdges[1] : connectedEdges[0];
                        }
                        // If 4, is self intersection
                        else if(connectedEdges.Count == 4)
                        {
                            var edgesWithVertexAlreadyInPolygon = connectedEdges
                                .Where(e => !e.Equals(currentEdge) && polygon.Vertices.Contains(e.GetVertexPair(currentVertex)))
                                .ToList();
                            //If any of them connects to a vertex already on the current polygon,
                            // If only one, set as current and it will close the polygon on the next iteration
                            if (edgesWithVertexAlreadyInPolygon.Count == 1)
                            {
                                currentEdge = edgesWithVertexAlreadyInPolygon.First();
                            }
                            // If two, it means that is a intersection with two previous edges computed,
                            // so set the next to the one that is not parallel to current
                            else if(edgesWithVertexAlreadyInPolygon.Count == 2)
                            {
                                currentEdge = edgesWithVertexAlreadyInPolygon[0].Direction.IsParallelTo(currentEdge.Direction) ?
                                    edgesWithVertexAlreadyInPolygon[1] :
                                    edgesWithVertexAlreadyInPolygon[0] ; 
                            }
                            // More than two, none on the current polygon so select one of those not yet computed
                            else
                            {
                                polygon.edges.Reverse();
                                polygon.Vertices.Reverse();
                                break; //it will go the other way around
                            }
                        }
                        else
                        {
                            throw new Exception("WARNING. Something unexepected happend with the polygons...");
                        }
                        computedVertices.Add(currentVertex);
                        currentVertex = currentEdge.GetVertexPair(currentVertex);
                    }
                    if (!polygon.edges.Last().Equals(currentEdge))
                    {
                        polygon.edges.Add(currentEdge);
                    }
                }
                this.polygons.Add(polygon.Id, polygon);
            }
        }

        #endregion

        #region Override Methods
        //TODO: Improve overriding equality methods as per http://www.loganfranken.com/blog/687/overriding-equals-in-c-part-1/

        /// <summary>
        /// Customizing the render of gVertex
        /// </summary>
        /// <param name="package"></param>
        /// <param name="parameters"></param>
        //[IsVisibleInDynamoLibrary(false)]
        //public void Tessellate(IRenderPackage package, TessellationParameters parameters)
        //{
        //    foreach(Vertex v in vertices)
        //    {
        //        v.Tessellate(package, parameters);
        //    }
        //    foreach(Edge e in edges)
        //    {
        //        e.Tessellate(package, parameters);
        //    }
        //}

        /// <summary>
        /// Implementation of IClonable interface
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            Graph newGraph = new Graph()
            {
                graph = new Dictionary<Vertex, List<Edge>>(),
                edges = new List<Edge>(this.edges),
                polygons = new Dictionary<int, Polygon>(this.polygons)
            };

            foreach(var item in this.graph)
            {
                newGraph.graph.Add(item.Key, new List<Edge>(item.Value));
            }

            return newGraph;
        }
        #endregion

    }
}
