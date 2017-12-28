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
#endregion

namespace Graphical.Base
{
    /// <summary>
    /// Representation of a Graph.
    /// Graph contains a Dictionary where
    /// </summary>
    public class Graph //: IGraphicItem
    {
        #region Variables
        
        internal Dictionary<int, List<Edge>> polygons = new Dictionary<int, List<Edge>>();
        internal int pId = 0;
        internal Dictionary<Vertex, List<Edge>> graph = new Dictionary<Vertex, List<Edge>>();
        public List<Vertex> vertices { get { return graph.Keys.ToList(); } }
        public List<Edge> edges { get; private set; }

        #endregion

        #region Constructors
        internal Graph(List<Vertex[]> input = null)
        {
            edges = new List<Edge>();
            input = input ?? new List<Vertex[]>();
            //Setting up Graph instance by adding vertices, edges and polygons
            for(var i = 0; i < input.Count(); i++)
            {
                Vertex[] polygon = input[i];
                //If first and last point of polygon list are the same, remove last.
                if (polygon.First().Equals(polygon.Last()) && polygon.Count() > 1)
                {
                    polygon = polygon.Take(polygon.Count() - 1).ToArray();
                }
                int polygonCount = polygon.Count();

                //For each point, creates vertex and associated edge and adds them
                //to the polygons Dictionary
                for(var j = 0; j < polygonCount; j++)
                {
                    int next_index = (j + 1) % polygonCount;
                    Vertex vertex = polygon[j];
                    Vertex next_vertex = polygon[next_index];
                    Edge edge = new Edge(vertex, next_vertex);
                    //If is a valid polygon, add id to vertex and
                    //edge to polygon dictionary
                    if(polygonCount > 2)
                    {
                        vertex.polygonId = pId;
                        next_vertex.polygonId = pId;
                        List<Edge> polygonEdges = new List<Edge>();
                        if (polygons.TryGetValue(pId, out polygonEdges))
                        {
                            polygonEdges.Add(edge);
                        }else
                        {
                            polygons.Add(pId, new List<Edge>() { edge});
                        }
                    }
                    AddEdge(edge);
                }

                if (polygonCount > 2) { pId += 1; }
            }
        }

        public static Graph ByPolygons(Polygon[] polygons)
        {
            List<Vertex[]> input = new List<Vertex[]>();
            foreach(Polygon pol in polygons)
            {
                Vertex[] vArray = pol.Points.Select(pt => Vertex.ByPoint(pt)).ToArray();
                input.Add(vArray);
            }

            return new Graph(input);
        }
        #endregion

        #region Methods

        /// <summary>
        /// Contains mathod for vertex in graph
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        internal bool Contains(Vertex vertex)
        {
            return graph.ContainsKey(vertex);
        }

        /// <summary>
        /// Contains method for edges in graph
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        internal bool Contains(Edge edge)
        {
            return edges.Contains(edge);
        }

        internal List<Edge> GetVertexEdges(Vertex vertex)
        {
            List<Edge> edgesList = new List<Edge>();
            if(graph.TryGetValue(vertex, out edgesList))
            {
                return edgesList;
            }else
            {
                graph.Add(vertex, new List<Edge>());
                return graph[vertex];
            }
        }

        internal List<Vertex> GetAdjecentVertices(Vertex v)
        {
            return graph[v].Select(edge => edge.GetVertexPair(v)).ToList();
        }
        /// <summary>
        /// Add edge to the analisys graph
        /// </summary>
        /// <param name="edge">New edge</param>
        internal void AddEdge(Edge edge)
        {
            List<Edge> startEdgesList = new List<Edge>();
            List<Edge> endEdgesList = new List<Edge>();
            if (graph.TryGetValue(edge.StartVertex, out startEdgesList))
            {
                startEdgesList.Add(edge);
            }
            else
            {
                graph.Add(edge.StartVertex, new List<Edge>() { edge });
            }

            if (graph.TryGetValue(edge.EndVertex, out endEdgesList))
            {
                endEdgesList.Add(edge);
            }
            else
            {
                graph.Add(edge.EndVertex, new List<Edge>() { edge });
            }
            
            if (!edges.Contains(edge)) { edges.Add(edge); }
        }

        /// <summary>
        /// Get polygon boundaries on graph
        /// </summary>
        /// <returns name="polygons[]"></returns>
        public List<Edge>[] GetBoundaryPolygons()
        {
            return polygons.Values.ToArray();
        }


        #endregion

        #region Override Methods
        //TODO: Improve overriding equality methods as per http://www.loganfranken.com/blog/687/overriding-equals-in-c-part-1/

        /// <summary>
        /// Override of Equals method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Boolean</returns>
        //public override bool Equals(object obj)
        //{

        //}

        /// <summary>
        /// Override of GetHashCode method
        /// </summary>
        /// <returns></returns>
        //public override int GetHashCode()
        //{
        //}

        /// <summary>
        /// Override of ToStringMethod
        /// </summary>
        /// <returns></returns>
        //public override string ToString()
        //{

        //}

        /// <summary>
        /// Customizing the render of Vertex
        /// </summary>
        /// <param name="package"></param>
        /// <param name="parameters"></param>
        //[IsVisibleInDynamoLibrary(false)]
        //public void Tessellate(IRenderPackage package, TessellationParameters parameters)
        //{
        //}
        #endregion

    }
}
