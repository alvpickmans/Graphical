using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphical.Extensions;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Graphical.Geometry
{
    /// <summary>
    /// Ray object
    /// </summary>
    public class Ray
    {
        #region Public Properties
        /// <summary>
        /// Ray's origin coordinates
        /// </summary>
        public Vertex Origin { get; private set; }

        /// <summary>
        /// Ray's coordinates defining the direction
        /// </summary>
        public Vector Direction { get; private set; }
        #endregion

        #region Private Constructors
        internal Ray(Vertex origin, Vector direction)
        {
            this.Origin = origin;
            this.Direction = direction;
        }
        #endregion

        #region Public Constructors
        /// <summary>
        /// Creates a Ray by defining it's origin and a second vertex defining its direction
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public static Ray ByTwoVertices(Vertex origin, Vertex vertex)
        {
            return new Ray(
                    origin,
                    Vector.ByTwoVertices(origin, vertex)
                );
        }

        /// <summary>
        /// Creates a Ray parallel to the X Axis through the given origin vertex
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static Ray XAxis(Vertex origin)
        {
            return new Ray(
                    origin,
                    Vector.XAxis()
                );
        }

        /// <summary>
        /// Creates a Ray parallel to the Y Axis through the given origin vertex
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static Ray YAxis(Vertex origin)
        {
            return new Ray(
                    origin,
                    Vector.YAxis()
                );
        } 

        /// <summary>
        /// Creates a Ray parallel to the Z Axis through the given origin vertex
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static Ray ZAxis(Vertex origin)
        {
            return new Ray(
                    origin,
                    Vector.ZAxis()
                );
        }

        #endregion

        #region Public Methods

        public Vertex Intersection(Edge edge)
        {
            if (this.Direction.IsParallelTo(edge.Direction)) { return null; }

            // Ray weigth equation
            // R(t) = (1-t)C + t*P
            double t;
            double m;
            var Cx = this.Origin.X;
            var Cy = this.Origin.Y;
            var Cz = this.Origin.Z;
            var Px = this.Direction.X + Cx;
            var Py = this.Direction.Y + Cy;
            var Pz = this.Direction.Z + Cz;

            // Edge equations, with origin O(x, y, z) and direction d(a, b, c)
            // I(m) = O + m*d

            var eqMatrix = DenseMatrix.OfArray(new double[2, 2]{
                { edge.Direction.X, Cx - Px},
                { edge.Direction.Y, Cy - Py}
            });

            var resultMatrix = DenseMatrix.OfArray(new double[2,1]
            {
                {Cx - edge.StartVertex.X },{ Cy - edge.StartVertex.Y }
            });

            var inverse = eqMatrix.Inverse();

            var solution = inverse * resultMatrix;

            m = solution.At(0, 0);
            t = solution.At(1, 0);

            var z = edge.StartVertex.Z + m * edge.Direction.Z;

            if(!z.AlmostEqualTo((1-t)*Cz + t * Pz))
            {
                return null;
            }

            if(m.AlmostEqualTo(0)) { return edge.StartVertex; }
            else if (m.AlmostEqualTo(1)) { return edge.EndVertex; }
            else if(m > 0 && m < 1)
            {
                double x = edge.StartVertex.X + m * edge.Direction.X;
                double y = edge.StartVertex.Y + m * edge.Direction.Y;

                return Vertex.ByCoordinates(x, y, z);
            }

            return null;
        }
        #endregion
    }
}
