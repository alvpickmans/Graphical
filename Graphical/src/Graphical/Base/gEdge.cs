#region namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPoint = Autodesk.DesignScript.Geometry.Point;
//using DSLine = Autodesk.DesignScript.Geometry.Line;
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
    public class gEdge : IGraphicItem
    {
        #region Variables
        /// <summary>
        /// StartVertex
        /// </summary>
        public gVertex StartVertex { get; private set; }

        /// <summary>
        /// EndVertex
        /// </summary>
        public gVertex EndVertex { get; private set; }


        public double Length { get; private set; }

        public gVector Direction { get; private set; }

        #endregion

        #region Constructors
        internal gEdge(gVertex start, gVertex end)
        {
            StartVertex = start;
            EndVertex = end;
            Length = StartVertex.DistanceTo(EndVertex);
            Direction = gVector.ByTwoVertices(StartVertex, EndVertex);
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
            gVertex start = gVertex.ByCoordinates(line.StartPoint.X, line.StartPoint.Y, line.StartPoint.Z);
            gVertex end = gVertex.ByCoordinates(line.EndPoint.X, line.EndPoint.Y, line.EndPoint.Z);
            return new gEdge(start, end);
        }
        #endregion

        /// <summary>
        /// Method to check if vertex belongs to edge
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public bool Contains(gVertex vertex)
        {
            return StartVertex.Equals(vertex) || EndVertex.Equals(vertex);
        }

        /// <summary>
        /// Method to return the other end vertex of the gEdge
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public gVertex GetVertexPair(gVertex vertex)
        {
            return (StartVertex.Equals(vertex)) ? EndVertex : StartVertex;
        }

        public bool IsCoplanarTo(gEdge edge)
        {
            // http://mathworld.wolfram.com/Coplanar.html
            gVector a = this.Direction;
            gVector b = edge.Direction;
            gVector c = gVector.ByTwoVertices(this.StartVertex, edge.StartVertex);

            return c.Dot(a.Cross(b)) == 0;
        }

        public gVertex Intersection(gEdge edge)
        {
            // http://mathworld.wolfram.com/Line-LineIntersection.html
            if (!this.IsCoplanarTo(edge)) { return null; }
            if (edge.Contains(this.StartVertex)) { return StartVertex; }
            if (edge.Contains(this.EndVertex)) { return EndVertex; }

            var a = this.Direction;
            var b = edge.Direction;
            var c = gVector.ByTwoVertices(this.StartVertex, edge.StartVertex);
            var cxb = c.Cross(b);
            var axb = a.Cross(b);

            double s = (cxb.Dot(axb)) / Math.Pow(axb.Length, 2);

            // s > 1, means that "intersection" vertex is not on either edge
            // s == NaN means they are parallels so never intersect
            if (s < 0 || s > 1 || Double.IsNaN(s)) { return null; }

            gVertex intersection = this.StartVertex.Translate(a.Scale(s));

            if (intersection.Equals(edge.StartVertex)){ return edge.StartVertex; }
            if (intersection.Equals(edge.EndVertex)) { return edge.EndVertex; }
            if (!intersection.OnEdge(edge))
            {
                return null;
            }

            return intersection;
        }

        public bool Intersects(gEdge edge)
        {
            return this.Intersection(edge) != null;
        }

        public double DistanceTo(gVertex vertex)
        {
            return vertex.DistanceTo(this);
        }

        public double DistanceTo(gEdge edge)
        {
            // http://mathworld.wolfram.com/Line-LineDistance.html
            if (this.IsCoplanarTo(edge))
            {
                var distances = new double[4]{
                    StartVertex.DistanceTo(edge),
                    EndVertex.DistanceTo(edge),
                    edge.StartVertex.DistanceTo(this),
                    edge.EndVertex.DistanceTo(this)
                };
                return distances.Min();
            }else
            {
                var a = this.Direction;
                var b = edge.Direction;
                var c = gVector.ByTwoVertices(this.StartVertex, edge.StartVertex);
                gVector cross = a.Cross(b);
                double numerator = c.Dot(cross);
                double denominator = cross.Length;
                return Math.Abs(numerator) / Math.Abs(denominator);

            }
            
        }

        public Line AsLine()
        {
            return Line.ByStartPointEndPoint(StartVertex.AsPoint(), EndVertex.AsPoint());
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

        #endregion

    }

    
    
}
