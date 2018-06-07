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
        internal Dictionary<int, gPolygon> polygons = new Dictionary<int, gPolygon>();

        /// <summary>
        /// Polygon's Id counter.
        /// </summary>
        internal int pId { get; private set; }

        /// <summary>
        /// Dictionary with vertex as key and values edges associated with the vertex.
        /// </summary>
        internal Dictionary<gVertex, List<gEdge>> graph = new Dictionary<gVertex, List<gEdge>>();

        /// <summary>
        /// Graph's vertices
        /// </summary>
        public List<gVertex> vertices { get { return graph.Keys.ToList(); } }

        /// <summary>
        /// Graph's edges
        /// </summary>
        public List<gEdge> edges { get; internal set; }

        public List<gPolygon> Polygons
        {
            get { return polygons.Values.ToList(); }
        }

        #endregion

        #region Internal Constructors
        public Graph()
        {
            edges = new List<gEdge>();
            graphID = Guid.NewGuid();
        }

        public Graph(List<gPolygon> gPolygonsSet)
        {
            edges = new List<gEdge>();
            graphID = Guid.NewGuid();
            //Setting up Graph instance by adding vertices, edges and polygons
            foreach(gPolygon gPolygon in gPolygonsSet)
            {
                List<gVertex> vertices = gPolygon.vertices;

                // Clear pre-existing edges in the case this is an updating process.
                gPolygon.edges.Clear();

                //If there is only one polygon, treat it as boundary
                if(gPolygonsSet.Count() == 1)
                {
                    gPolygon.isBoundary = true;
                }

                //If first and last point of vertices list are the same, remove last.
                if (vertices.First().Equals(vertices.Last()) && vertices.Count() > 1)
                {
                    vertices = vertices.Take(vertices.Count() - 1).ToList();
                }

                //For each point, creates vertex and associated edge and adds them
                //to the polygons Dictionary
                int vertexCount = vertices.Count();

                // If valid polygon
                if (vertexCount >= 3)
                {
                    int newId = GetNextId();
                    for (var j = 0; j < vertexCount; j++)
                    {
                        int next_index = (j + 1) % vertexCount;
                        gVertex vertex = vertices[j];
                        gVertex next_vertex = vertices[next_index];
                        gEdge edge = new gEdge(vertex, next_vertex);

                        //If is a valid polygon, add id to vertex and
                        //edge to vertices dictionary
                        if (vertexCount > 2)
                        {
                            vertex.polygonId = newId;
                            next_vertex.polygonId = newId;
                            gPolygon gPol = new gPolygon();
                            if (polygons.TryGetValue(newId, out gPol))
                            {
                                gPol.edges.Add(edge);
                            }
                            else
                            {
                                gPolygon.edges.Add(edge);
                                gPolygon.id = newId;
                                polygons.Add(newId, gPolygon);
                            }
                        }
                        AddEdge(edge);
                    }
                }

            }
        }

        #endregion

        #region Internal Methods

        internal int GetNextId()
        {
            if(this.pId == null)
            {
                this.pId = 0;
            }
            else
            {
                pId++;
            }
            return pId;
        }

        internal void ResetEdgesFromPolygons()
        {
            this.edges.Clear();
            this.graph.Clear();

            foreach(gPolygon polygon in polygons.Values)
            {
                foreach(gEdge edge in polygon.edges)
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
        public bool Contains(gVertex vertex)
        {
            return graph.ContainsKey(vertex);
        }

        /// <summary>
        /// Contains method for edges in graph
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool Contains(gEdge edge)
        {
            return edges.Contains(edge);
        }

        public List<gEdge> GetVertexEdges(gVertex vertex)
        {
            List<gEdge> edgesList = new List<gEdge>();
            if(graph.TryGetValue(vertex, out edgesList))
            {
                return edgesList;
            }else
            {
                //graph.Add(vertex, new List<gEdge>());
                return new List<gEdge>();
            }
        }

        public List<gVertex> GetAdjecentVertices(gVertex v)
        {
            return graph[v].Select(edge => edge.GetVertexPair(v)).ToList();
        }

        /// <summary>
        /// Add edge to the analisys graph
        /// </summary>
        /// <param name="edge">New edge</param>
        public void AddEdge(gEdge edge)
        {
            List<gEdge> startEdgesList = new List<gEdge>();
            List<gEdge> endEdgesList = new List<gEdge>();
            if (graph.TryGetValue(edge.StartVertex, out startEdgesList))
            {
                if (!startEdgesList.Contains(edge)) { startEdgesList.Add(edge); }
            }
            else
            {
                graph.Add(edge.StartVertex, new List<gEdge>() { edge });
            }

            if (graph.TryGetValue(edge.EndVertex, out endEdgesList))
            {
                if (!endEdgesList.Contains(edge)) { endEdgesList.Add(edge); }
            }
            else
            {
                graph.Add(edge.EndVertex, new List<gEdge>() { edge });
            }
            
            if (!edges.Contains(edge)) { edges.Add(edge); }
        }

        /// <summary>
        /// Computes edges and creates polygons from those connected by vertices.
        /// </summary>
        public void BuildPolygons()
        {
            var computedVertices = new List<gVertex>();
            
            foreach(gVertex v in vertices)
            {
                // If already belongs to a polygon or is not a polygon vertex or already computed
                if( computedVertices.Contains(v) || v.polygonId >= 0 || graph[v].Count > 2) { continue; }

                computedVertices.Add(v);
                gPolygon polygon = new gPolygon(GetNextId(), false);
                
                polygon.AddVertex(v);
                foreach(gEdge edge in GetVertexEdges(v))
                {
                    gEdge nextEdge = edge;
                    gVertex nextVertex = edge.GetVertexPair(v);
                    while (!polygon.vertices.Contains(nextVertex))
                    {
                        computedVertices.Add(nextVertex);
                        polygon.AddVertex(nextVertex);
                        polygon.edges.Add(nextEdge);

                        //It is extreme vertex, polygon not closed
                        if(graph[nextVertex].Count < 2) { break; }

                        nextEdge = graph[nextVertex].Where(e => !e.Equals(nextEdge)).First();
                        nextVertex = nextEdge.GetVertexPair(nextVertex);
                    }
                    if (!polygon.edges.Last().Equals(nextEdge))
                    {
                        polygon.edges.Add(nextEdge);
                    }
                }
                this.polygons.Add(polygon.id, polygon);
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
        //    foreach(gVertex v in vertices)
        //    {
        //        v.Tessellate(package, parameters);
        //    }
        //    foreach(gEdge e in edges)
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
                graph = new Dictionary<gVertex, List<gEdge>>(),
                edges = new List<gEdge>(this.edges),
                polygons = new Dictionary<int, gPolygon>(this.polygons)
            };

            foreach(var item in this.graph)
            {
                newGraph.graph.Add(item.Key, new List<gEdge>(item.Value));
            }

            return newGraph;
        }
        #endregion

    }
}
