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
    /// like is internal or limit boundary.
    /// </summary>
    [IsVisibleInDynamoLibrary(false)]
    public class gPolygon :IDisposable, ICloneable
    {
        #region Variables
        internal int id { get; set; }
        internal bool isBoundary { get; set; }
        internal List<gEdge> edges = new List<gEdge>();
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

        internal gPolygon AddVertex(gVertex v, gEdge intersectingEdge)
        {
            //Assumes that vertex v intersects one of polygons edges.
            gPolygon newPolygon = (gPolygon)this.Clone();
            v.polygonId = this.id;
            int index = newPolygon.vertices.IndexOf(intersectingEdge.StartVertex);
            newPolygon.vertices.Insert(index + 1, v);
            //newPolygon.vertices = gVertex.OrderByRadianAndDistance(newPolygon.vertices);
            newPolygon.edges.Clear();
            int verticesCount = newPolygon.vertices.Count;
            for (var i = 0; i < verticesCount; i++)
            {
                int nextIndex = (i + 1) % verticesCount;
                newPolygon.edges.Add(new gEdge(newPolygon.vertices[i], newPolygon.vertices[nextIndex]));
            }

            return newPolygon;
        }

        /// <summary>
        /// Implementation of Dispose() method
        /// </summary>
        public void Dispose()
        {
            foreach(gEdge e in edges)
            {
                ((IDisposable)e).Dispose();
            }
            foreach(gVertex v in vertices)
            {
                ((IDisposable)v).Dispose();
            }
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
