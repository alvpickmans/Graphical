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
        #region Private Properties

        /// <summary>
        /// Polygons dictionary with their Id as dictionary key
        /// </summary>
        internal Dictionary<int, Polygon> _polygonsDict = new Dictionary<int, Polygon>();

        /// <summary>
        /// Dictionary with vertex as key and values Edges associated with the vertex.
        /// </summary>
        internal Dictionary<Vertex, List<Edge>> _vertexEdgesDict = new Dictionary<Vertex, List<Edge>>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Graph's Vertices
        /// </summary>
        public List<Vertex> Vertices { get { return _vertexEdgesDict.Keys.ToList(); } }

        /// <summary>
        /// Graph's Edges
        /// </summary>
        public List<Edge> Edges { get; internal set; }

        /// <summary>
        /// Graph's Polygons
        /// </summary>
        public List<Polygon> Polygons
        {
            get { return _polygonsDict.Values.ToList(); }
        }

        #endregion

        #region Public Constructors
        public Graph()
        {
            Edges = new List<Edge>();
        }

        public Graph(List<Polygon> polygonList)
        {
            Edges = new List<Edge>();

            // Adding _polygonsDict to Graph
            for (int i = 0; i < polygonList.Count; i++)
            {
                var polygon = polygonList[i];
                this._polygonsDict.Add(polygon.Id, polygon);

                polygon.Vertices.ForEach(v => this._vertexEdgesDict.Add(v, new List<Edge>()));
            }
        }

        #endregion

        #region Internal Methods

        internal void ResetEdgesFromPolygons()
        {
            this.Edges.Clear();
            this._vertexEdgesDict.Clear();

            foreach(Polygon polygon in _polygonsDict.Values)
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
            return _vertexEdgesDict.ContainsKey(vertex);
        }

        /// <summary>
        /// Contains method for Edges in graph
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool Contains(Edge edge)
        {
            return Edges.Contains(edge);
        }

        public List<Edge> GetVertexEdges(Vertex vertex)
        {
            List<Edge> edgesList = new List<Edge>();
            if(_vertexEdgesDict.TryGetValue(vertex, out edgesList))
            {
                return edgesList;
            }else
            {
                //graph.Add(vertex, new List<Edge>());
                return new List<Edge>();
            }
        }

        //public List<Vertex> GetAdjecentVertices(Vertex v)
        //{
        //    return _vertexEdgesDict[v].Select(edge => edge.GetVertexPair(v)).ToList();
        //}

        /// <summary>
        /// Add edge to the analisys graph
        /// </summary>
        /// <param name="edge">New edge</param>
        public void AddEdge(Edge edge)
        {
            List<Edge> startEdgesList = new List<Edge>();
            List<Edge> endEdgesList = new List<Edge>();
            if (_vertexEdgesDict.TryGetValue(edge.StartVertex, out startEdgesList))
            {
                if (!startEdgesList.Contains(edge)) { startEdgesList.Add(edge); }
            }
            else
            {
                _vertexEdgesDict.Add(edge.StartVertex, new List<Edge>() { edge });
            }

            if (_vertexEdgesDict.TryGetValue(edge.EndVertex, out endEdgesList))
            {
                if (!endEdgesList.Contains(edge)) { endEdgesList.Add(edge); }
            }
            else
            {
                _vertexEdgesDict.Add(edge.EndVertex, new List<Edge>() { edge });
            }
            
            if (!Edges.Contains(edge)) { Edges.Add(edge); }
        }

        #endregion

        #region Override Methods
        /// <summary>
        /// Implementation of IClonable interface
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            Graph newGraph = new Graph()
            {
                _vertexEdgesDict = new Dictionary<Vertex, List<Edge>>(),
                Edges = new List<Edge>(this.Edges),
                _polygonsDict = new Dictionary<int, Polygon>(this._polygonsDict)
            };

            foreach(var item in this._vertexEdgesDict)
            {
                newGraph._vertexEdgesDict.Add(item.Key, new List<Edge>(item.Value));
            }

            return newGraph;
        }
        #endregion

    }
}
