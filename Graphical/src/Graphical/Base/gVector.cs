using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Runtime;

namespace Graphical.Base
{
    // Resources:
    // https://www.mathsisfun.com/algebra/vectors-cross-product.html
    // http://www.analyzemath.com/stepbystep_mathworksheets/vectors/vector3D_angle.html
    // https://betterexplained.com/articles/cross-product/
    // http://mathworld.wolfram.com/CrossProduct.html

    [IsVisibleInDynamoLibrary(false)]
    public class gVector
    {
        #region Public Properties
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public double Length { get; private set; } 
        #endregion


        #region Constructors

        private gVector(double x, double y, double z, double length = Double.PositiveInfinity)
        {
            X = x;
            Y = y;
            Z = z;
            Length = (Double.IsPositiveInfinity(length)) ? Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2)) : length;
            Length = Math.Round(Length, 6, MidpointRounding.AwayFromZero);
        }

        public static gVector ByCoordinates(double x, double y, double z)
        {
            return new gVector(x, y, z);
        }

        public static gVector ByTwoVertices(gVertex start, gVertex end)
        {
            var x = end.X - start.X;
            var y = end.Y - start.Y;
            var z = end.Z - start.Z;
            var length = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
            return new gVector(x, y, z, length);
        }

        public static gVector XAxis()
        {
            return new gVector(1, 0, 0, 1);
        }

        public static gVector YAxis()
        {
            return new gVector(0, 1, 0, 1);
        }

        public static gVector ZAxis()
        {
            return new gVector(0, 0, 1, 1);
        }
        #endregion

        #region Public Methods
        public double Dot(gVector vector)
        {
            return (this.X * vector.X) + (this.Y * vector.Y) + (this.Z * vector.Z);
        }

        public double Angle(gVector vector)
        {
            double dot = this.Dot(vector);
            double cos = dot / (this.Length * vector.Length);
            return ToDegrees(Math.Acos(cos));
        }

        public gVector Cross(gVector vector)
        {
            double x = (this.Y * vector.Z) - (this.Z * vector.Y);
            double y = (this.Z * vector.X) - (this.X * vector.Z);
            double z = (this.X * vector.Y) - (this.Y * vector.X);
            double angle = ToRadians(this.Angle(vector));
            double length = this.Length * vector.Length * Math.Sin(angle);
            return new gVector(x, y, z, length);
        }
        #endregion

        #region Internal Methods

        internal static double ToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }

        internal static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        } 
        #endregion

    }
}
