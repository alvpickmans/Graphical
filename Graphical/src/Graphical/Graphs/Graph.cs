#region namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using System.Globalization;
using Graphical.Base;
#endregion

namespace Graphical.Graphs
{
    
    /// <summary>
    /// Representation of a Graph.
    /// Graph contains a Dictionary where
    /// </summary>
    public class Graph : IGraphicItem, ICloneable
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
        internal int pId = 0;

        /// <summary>
        /// Dictionary with vertex as key and values edges associated with the vertex.
        /// </summary>
        internal Dictionary<gVertex, List<gEdge>> graph = new Dictionary<gVertex, List<gEdge>>();

        /// <summary>
        /// Graph's vertices
        /// </summary>
        internal List<gVertex> vertices { get { return graph.Keys.ToList(); } }

        /// <summary>
        /// Graph's edges
        /// </summary>
        internal List<gEdge> edges { get; set; }

        #endregion

        #region Internal Constructors
        internal Graph()
        {
            edges = new List<gEdge>();
            graphID = Guid.NewGuid();
        }

        internal Graph(List<gPolygon> gPolygonsSet)
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
                            vertex.polygonId = pId;
                            next_vertex.polygonId = pId;
                            gPolygon gPol = new gPolygon();
                            if (polygons.TryGetValue(pId, out gPol))
                            {
                                gPol.edges.Add(edge);
                            }
                            else
                            {
                                gPolygon.edges.Add(edge);
                                gPolygon.id = pId;
                                polygons.Add(pId, gPolygon);
                            }
                        }
                        AddEdge(edge);
                    }
                    pId += 1;
                }

            }
        }

        
        /// <summary>
        /// Create a list of gPolygons from a set of DS polygons.
        /// </summary>
        /// <param name="polygons"></param>
        /// <param name="isExternal"></param>
        /// <returns></returns>
        internal static List<gPolygon> FromPolygons(Polygon[] polygons, bool isExternal)
        {
            if(polygons == null) { throw new NullReferenceException("polygons"); }
            List<gPolygon> input = new List<gPolygon>();
            foreach(Polygon pol in polygons)
            {
                gPolygon gPol = new gPolygon(-1, isExternal);
                gPol.vertices = pol.Points.Select(pt => gVertex.ByCoordinates(pt.X, pt.Y, pt.Z)).ToList();
                input.Add(gPol);
            }

            return input;
        }


        #endregion

        #region Internal Methods

        /// <summary>
        /// Contains mathod for vertex in graph
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        internal bool Contains(gVertex vertex)
        {
            return graph.ContainsKey(vertex);
        }

        /// <summary>
        /// Contains method for edges in graph
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        internal bool Contains(gEdge edge)
        {
            return edges.Contains(edge);
        }

        internal List<gEdge> GetVertexEdges(gVertex vertex)
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

        internal List<gVertex> GetAdjecentVertices(gVertex v)
        {
            return graph[v].Select(edge => edge.GetVertexPair(v)).ToList();
        }
        /// <summary>
        /// Add edge to the analisys graph
        /// </summary>
        /// <param name="edge">New edge</param>
        internal void AddEdge(gEdge edge)
        {
            List<gEdge> startEdgesList = new List<gEdge>();
            List<gEdge> endEdgesList = new List<gEdge>();
            if (graph.TryGetValue(edge.StartVertex, out startEdgesList))
            {
                startEdgesList.Add(edge);
            }
            else
            {
                graph.Add(edge.StartVertex, new List<gEdge>() { edge });
            }

            if (graph.TryGetValue(edge.EndVertex, out endEdgesList))
            {
                endEdgesList.Add(edge);
            }
            else
            {
                graph.Add(edge.EndVertex, new List<gEdge>() { edge });
            }
            
            if (!edges.Contains(edge)) { edges.Add(edge); }
        }

        #endregion

        /// <summary>
        /// Get graph edges as lines
        /// </summary>
        /// <returns name="lines"></returns>
        public List<Line> GetAsLines()
        {
            return edges.Select(e => e.AsLine()).ToList();
        }

        public List<int> EdgesPerVertex()
        {
            return graph.Values.Select(v => v.Count).ToList();
        }

        #region Override Methods
        //TODO: Improve overriding equality methods as per http://www.loganfranken.com/blog/687/overriding-equals-in-c-part-1/


        /// <summary>
        /// Override of ToStringMethod
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Graph:(gVertices: {0}, gEdges: {1})", vertices.Count.ToString(), edges.Count.ToString());
        }

        /// <summary>
        /// Customizing the render of gVertex
        /// </summary>
        /// <param name="package"></param>
        /// <param name="parameters"></param>
        [IsVisibleInDynamoLibrary(false)]
        public void Tessellate(IRenderPackage package, TessellationParameters parameters)
        {
            foreach(gVertex v in vertices)
            {
                v.Tessellate(package, parameters);
            }
            foreach(gEdge e in edges)
            {
                e.Tessellate(package, parameters);
            }
        }

        /// <summary>
        /// Implementation of IClonable interface
        /// </summary>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
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
