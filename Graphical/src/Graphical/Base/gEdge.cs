#region namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPoint = Autodesk.DesignScript.Geometry.Point;
using DSLine = Autodesk.DesignScript.Geometry.Line;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
#endregion


namespace Graphical.Base
{
    /// <summary>
    /// Representation of Edges on a graph
    /// </summary>
    [IsVisibleInDynamoLibrary(false)]
    public class gEdge : IGraphicItem, IDisposable
    {
        #region Variables
        /// <summary>
        /// StartVertex
        /// </summary>
        internal gVertex StartVertex { get; private set; }

        /// <summary>
        /// EndVertex
        /// </summary>
        internal gVertex EndVertex { get; private set; }
        

        internal double length { get { return LineGeometry().Length; } }

        #endregion

        #region Constructors
        internal gEdge(gVertex start, gVertex end)
        {
            StartVertex = start;
            EndVertex = end;
            //LineGeometry = Line.ByStartPointEndPoint(start.point, end.point);
        }

        /// <summary>
        /// gEdge constructor by start and end vertices
        /// </summary>
        /// <param name="start">Start vertex</param>
        /// <param name="end">End gVertex</param>
        /// <returns name="edge">edge</returns>
        public static gEdge ByStartVertexEndVertex(gVertex start, gVertex end)
        {
            return new gEdge(start, end);
        }

        /// <summary>
        /// gEdge constructor by line
        /// </summary>
        /// <param name="line">line</param>
        /// <returns name="edge">edge</returns>
        public static gEdge ByLine(Line line)
        {
            return new gEdge(gVertex.ByPoint(line.StartPoint), gVertex.ByPoint(line.EndPoint));
        }
        #endregion

        /// <summary>
        /// Returns the line associated with the gEdge
        /// </summary>
        public Line LineGeometry()
        {
            return Line.ByStartPointEndPoint(StartVertex.point, EndVertex.point);
        }

        /// <summary>
        /// Method to check if vertex belongs to edge
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        internal bool Contains(gVertex vertex)
        {
            return StartVertex.Equals(vertex) || EndVertex.Equals(vertex);
        }

        /// <summary>
        /// Method to return the other end vertex of the gEdge
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        internal gVertex GetVertexPair(gVertex vertex)
        {
            return (StartVertex.Equals(vertex)) ? EndVertex : StartVertex;
        }

        internal DSLine GetProjectionOnPlane(string plane = "xy")
        {
            return DSLine.ByStartPointEndPoint(StartVertex.GetProjectionOnPlane(plane), EndVertex.GetProjectionOnPlane(plane));
        }

        internal double DistanceTo(object obj)
        {
            if (GetType() == obj.GetType())
            {
                gEdge e = (gEdge)obj;
                using (DSLine line1 = this.LineGeometry())
                using (DSLine line2 = e.LineGeometry())
                {
                    return line1.DistanceTo(line2);
                }
            }
            else if (obj.GetType() == typeof(gVertex))
            {
                gVertex v = (gVertex)obj;
                using (DSPoint p = v.point)
                using (Line line = this.LineGeometry())
                {
                    return line.DistanceTo(p);
                }
            }
            else
            {
                using (Line line = this.LineGeometry())
                {
                    return line.DistanceTo(obj as Autodesk.DesignScript.Geometry.Geometry);
                }
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

            gEdge e= (gEdge)obj;
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
            return String.Format("gEdge(StartVertex: {0}, EndVertex: {1})", StartVertex, EndVertex);
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
            package.AddLineStripVertex(StartVertex.X, StartVertex.Y, StartVertex.Z);
            package.AddLineStripVertex(EndVertex.X, EndVertex.Y, EndVertex.Z);
            /*Colour addition can be done iteratively with a for loop,
             * but for just two elements might be better to save the overhead
             * variable declaration and all.
             */
            package.AddLineStripVertexColor(150, 200, 255, 255);
            package.AddLineStripVertexColor(150, 200, 255, 255);


        }

        /// <summary>
        /// Implementation of Dispose method
        /// </summary>
        public void Dispose()
        {
            //((IDisposable)LineGeometry).Dispose();
            ((IDisposable)StartVertex).Dispose();
            ((IDisposable)EndVertex).Dispose();
        }


        #endregion

    }

    
    
}
