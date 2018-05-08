using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Runtime;
using Graphical.Geometry;

namespace Graphical.Base
{
    /// <summary>
    /// gPolygon class to hold graph´s polygon information in relation to its function on the graph
    /// like if it is internal or limit boundary.
    /// </summary>
    [IsVisibleInDynamoLibrary(false)]
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
        #endregion

        #region Constructors
        internal gPolygon() { }

        internal gPolygon(int _id, bool _isExternal)
        {
            id = _id;
            isBoundary = _isExternal;
        } 
        #endregion

        public static gPolygon ByVertices(List<gVertex> vertices, bool isExternal)
        {
            gPolygon polygon = new gPolygon(-1, isExternal);
            polygon.vertices = vertices;
            return polygon;
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
        
        public object Clone()
        {
            gPolygon newPolygon = new gPolygon(this.id, this.isBoundary);
            newPolygon.edges = new List<gEdge>(this.edges);
            newPolygon.vertices = new List<gVertex>(this.vertices);
            return newPolygon;
        }
    }
}
