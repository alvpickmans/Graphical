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

                polygon.Edges.ForEach(edge => this.AddEdge(edge));
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

        /// <summary>
        /// Add edge to the analisys graph
        /// </summary>
        /// <param name="edge">New edge</param>
        public void AddEdge(Edge edge)
        {
            List<Edge> startEdgesList;
            List<Edge> endEdgesList;
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

        /// <summary>
        /// Computes Edges and creates _polygonsDict from those connected by Vertices.
        /// </summary>
        public void BuildPolygons()
        {
            var computedVertices = new List<Vertex>();

            foreach (Vertex v in Vertices)
            {
                // If already belongs to a polygon or is not a polygon vertex or already computed
                if (computedVertices.Contains(v) || _vertexEdgesDict[v].Count > 2) { continue; }

                computedVertices.Add(v);
                Polygon polygon = new Polygon(false);

                polygon.AddVertex(v);
                foreach (Edge edge in GetVertexEdges(v))
                {
                    Edge currentEdge = edge;
                    Vertex currentVertex = edge.GetVertexPair(v);
                    while (!polygon.vertices.Contains(currentVertex) || !computedVertices.Contains(currentVertex))
                    {
                        polygon.AddVertex(currentVertex);
                        polygon.edges.Add(currentEdge);

                        var connectedEdges = _vertexEdgesDict[currentVertex];
                        //It is extreme vertex, polygon not closed
                        if (connectedEdges.Count < 2)
                        {
                            break;
                        }
                        // If just two Edges, select the one that is not current nextEdge
                        else if (connectedEdges.Count == 2)
                        {
                            currentEdge = connectedEdges[0].Equals(currentEdge) ? connectedEdges[1] : connectedEdges[0];
                        }
                        // If 4, is self intersection
                        else if (connectedEdges.Count == 4)
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
                            // If two, it means that is a intersection with two previous Edges computed,
                            // so set the next to the one that is not parallel to current
                            else if (edgesWithVertexAlreadyInPolygon.Count == 2)
                            {
                                currentEdge = edgesWithVertexAlreadyInPolygon[0].Direction.IsParallelTo(currentEdge.Direction) ?
                                    edgesWithVertexAlreadyInPolygon[1] :
                                    edgesWithVertexAlreadyInPolygon[0];
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
                            throw new Exception("WARNING. Something unexepected happend with the _polygonsDict...");
                        }
                        computedVertices.Add(currentVertex);
                        currentVertex = currentEdge.GetVertexPair(currentVertex);
                    }
                    if (!polygon.edges.Last().Equals(currentEdge))
                    {
                        polygon.edges.Add(currentEdge);
                    }
                }
                this._polygonsDict.Add(polygon.Id, polygon);
            }
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
