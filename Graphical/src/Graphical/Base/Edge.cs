#region namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPoint = Autodesk.DesignScript.Geometry.Point;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
#endregion


namespace Graphical.Base
{
    /// <summary>
    /// Representation of Edges on a graph
    /// </summary>
    public class Edge : IGraphicItem
    {
        #region Variables
        /// <summary>
        /// Edge StartVertex
        /// </summary>
        public Vertex StartVertex { get; private set; }

        /// <summary>
        /// Edge EndVertex
        /// </summary>
        public Vertex EndVertex { get; private set; }

        public Line LineGeometry { get; private set; }
        #endregion

        #region Constructors
        internal Edge(Vertex start, Vertex end)
        {
            StartVertex = start;
            EndVertex = end;
            LineGeometry = Line.ByStartPointEndPoint(start.point, end.point);
        }

        /// <summary>
        /// Edge constructor by start and end vertices
        /// </summary>
        /// <param name="start">Start vertex</param>
        /// <param name="end">End Vertex</param>
        /// <returns name="edge">edge</returns>
        public static Edge ByStartVertexEndVertex(Vertex start, Vertex end)
        {
            return new Edge(start, end);
        }

        /// <summary>
        /// Edge constructor by line
        /// </summary>
        /// <param name="line">line</param>
        /// <returns name="edge">edge</returns>
        public static Edge ByLine(Line line)
        {
            return new Edge(Vertex.ByPoint(line.StartPoint), Vertex.ByPoint(line.EndPoint));
        } 
        #endregion

        /// <summary>
        /// Method to check if vertex belongs to edge
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        internal bool Contains(Vertex vertex)
        {
            return StartVertex.Equals(vertex) || EndVertex.Equals(vertex);
        }

        /// <summary>
        /// Method to return the other end vertex of the Edge
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        internal Vertex GetVertexPair(Vertex vertex)
        {
            if (StartVertex.Equals(vertex))
            {
                return EndVertex;
            }else
            {
                return StartVertex;
            }
        }

        #region override methods
        //TODO: Improve overriding equality methods as per http://www.loganfranken.com/blog/687/overriding-equals-in-c-part-1/

        /// <summary>
        /// Override of Equal Method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) { return false; }

            Edge e= (Edge)obj;
            if (StartVertex.Equals(e.StartVertex) && EndVertex.Equals(e.EndVertex)) { return true; }
            if (StartVertex.Equals(e.EndVertex) && EndVertex.Equals(e.StartVertex)) { return true; }
            return false;

        }

        /// <summary>
        /// Override of GetHashCode Method
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return StartVertex.GetHashCode() ^ EndVertex.GetHashCode();
        }


        /// <summary>
        /// Override of ToString method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Edge(StartVertex: {0}, EndVertex: {1})", StartVertex, EndVertex);
        }

        /// <summary>
        /// Implementation of Tessellation render method
        /// </summary>
        /// <param name="package"></param>
        /// <param name="parameters"></param>
        [IsVisibleInDynamoLibrary(false)]
        public void Tessellate(IRenderPackage package, TessellationParameters parameters)
        {
            //throw new NotImplementedException();
            //package.AddLineStripVertexCount(2);
            package.AddLineStripVertex(StartVertex.point.X, StartVertex.point.Y, StartVertex.point.Z);
            package.AddLineStripVertex(EndVertex.point.X, EndVertex.point.Y, EndVertex.point.Z);
            /*Colour addition can be done iteratively with a for loop,
             * but for just two elements might be better to save the overhead
             * variable declaration and all.
             */
            package.AddLineStripVertexColor(150, 200, 255, 255);
            package.AddLineStripVertexColor(150, 200, 255, 255);


        } 


        #endregion

    }

    
    
}
