using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphical.Geometry
{
    /// <summary>
    /// gPolygon class to hold graph´s polygon information in relation to its function on the graph
    /// like if it is internal or limit boundary.
    /// </summary>
    public class gPolygon : gBase, ICloneable
    {
        #region Variables

        /// <summary>
        /// Polygon's id
        /// </summary>
        internal int id { get; set; }

        /// <summary>
        /// Flag to check polygons role: Internal or Boundary
        /// </summary>
        internal bool isBoundary { get; set; }

        /// <summary>
        /// Polygon's edges
        /// </summary>
        internal List<gEdge> edges = new List<gEdge>();

        /// <summary>
        /// Polygon's Vertices
        /// </summary>
        internal List<gVertex> vertices = new List<gVertex>();

        public List<gVertex> Vertices
        {
            get { return vertices; }
        }

        public List<gEdge> Edges
        {
            get
            {
                return edges;
            }
        }

        public bool IsClosed
        {
            get
            {
                return this.edges.Count > 2 && (edges.First().StartVertex.OnEdge(edges.Last()) || edges.First().EndVertex.OnEdge(edges.Last()));
            }
        }
        #endregion

        #region Internal Constructors
        internal gPolygon() { }

        internal gPolygon(int _id, bool _isExternal)
        {
            id = _id;
            isBoundary = _isExternal;
        }
        #endregion

        #region Public Constructos
        public static gPolygon ByVertices(List<gVertex> vertices, bool isExternal)
        {
            gPolygon polygon = new gPolygon(-1, isExternal);
            polygon.vertices = vertices;
            int vertexCount = vertices.Count;
            for (var j = 0; j < vertexCount; j++)
            {
                int next_index = (j + 1) % vertexCount;
                gVertex vertex = vertices[j];
                gVertex next_vertex = vertices[next_index];
                polygon.edges.Add( new gEdge(vertex, next_vertex));
            }
            return polygon;
        }
        #endregion

        #region Internal Methods
        internal void AddVertex(gVertex vertex)
        {
            vertex.polygonId = this.id;
            vertices.Add(vertex);
        }

        internal gPolygon AddVertex(gVertex v, gEdge intersectingEdge)
        {
            //Assumes that vertex v intersects one of polygons edges.
            gPolygon newPolygon = (gPolygon)this.Clone();

            // Assign the polygon Id to the new vertex.
            v.polygonId = this.id;

            // Getting the index of the intersecting edge's start vertex and
            // inserting the new vertex at the following index.
            int index = newPolygon.vertices.IndexOf(intersectingEdge.StartVertex);
            newPolygon.vertices.Insert(index + 1, v);

            // Rebuilding edges.
            newPolygon.edges.Clear();
            int verticesCount = newPolygon.vertices.Count;
            for (var i = 0; i < verticesCount; i++)
            {
                int nextIndex = (i + 1) % verticesCount;
                newPolygon.edges.Add(new gEdge(newPolygon.vertices[i], newPolygon.vertices[nextIndex]));
            }

            return newPolygon;
        }
        #endregion

        #region Public Methods

        public bool ContainsVertex(gVertex vertex)
        {
            gVertex maxVertex = vertices.OrderByDescending(v => v.DistanceTo(vertex)).First();
            double maxDistance = vertex.DistanceTo(maxVertex) * 1.5;
            gVertex v2 = gVertex.ByCoordinates(vertex.X + maxDistance, vertex.Y, vertex.Z);
            gEdge ray = gEdge.ByStartVertexEndVertex(vertex, v2);
            gVertex coincident = null;
            int intersections = 0;
            foreach (gEdge edge in edges)
            {
                ////gVertex above or below edge
                //if (!Threshold(vertex.Y, edge.StartVertex.Y) && !Threshold(vertex.Y, edge.EndVertex.Y))
                //{
                //    if (vertex.Y < edge.StartVertex.Y && vertex.Y < edge.EndVertex.Y) { continue; }
                //    if (vertex.Y > edge.StartVertex.Y && vertex.Y > edge.EndVertex.Y) { continue; }
                //}
                ////if((edge.StartVertex.Y > vertex.Y && edge.EndVertex.Y <= vertex.Y)) { continue; }
                ////if(edge.StartVertex.Y <= vertex.Y && edge.EndVertex.Y > vertex.Y) { continue; }
                ////Vertices colinear to v1
                //gBase intersection = ray.Intersection(edge);
                //if (intersection != null)
                //{
                //    if (intersection.GetType() == typeof(gEdge))
                //    {
                //        gEdge edgeIntesection = (gEdge)intersection;
                //        if (!edgeIntesection.Equals(edge))
                //        {
                //            return edgeIntesection.StartVertex.OnEdge(edge) || edgeIntesection.EndVertex.OnEdge(edge);
                //        }
                //        else
                //        {
                //            intersections++;
                //        }
                //    }
                //    else
                //    {
                //        gVertex vertexIntersection = (gVertex)intersection;
                //        if (edge.Contains(vertexIntersection))
                //        {
                //            intersections += intersection.Equals(coincident) ? 0 : 1;
                //            coincident = vertexIntersection;
                //        }
                //        else
                //        {
                //            intersections++;
                //        }
                //    }
                gBase intersection = ray.Intersection(edge);
                if (edge.StartVertex.Y <= vertex.Y)
                {
                    
                    if (edge.EndVertex.Y > vertex.Y && intersection != null && intersection.GetType() == typeof(gVertex))
                    {
                        ++intersections;
                    }
                }
                else
                {
                    if (edge.EndVertex.Y <= vertex.Y && intersection != null && intersection.GetType() == typeof(gVertex))
                    {
                        --intersections;
                    }
                }

                
            }

            //If intersections is odd, returns true, false otherwise
            //return (intersections % 2 == 0) ? false : true;
            return intersections != 0;
        }
        #endregion

        public object Clone()
        {
            gPolygon newPolygon = new gPolygon(this.id, this.isBoundary);
            newPolygon.edges = new List<gEdge>(this.edges);
            newPolygon.vertices = new List<gVertex>(this.vertices);
            return newPolygon;
        }
    }
}
