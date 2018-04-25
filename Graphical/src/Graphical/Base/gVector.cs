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
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public double Length { get; private set; }

        public gVector(double x, double y, double z, double length = Double.PositiveInfinity)
        {
            X = x;
            Y = y;
            Z = z;
            Length = (Double.IsPositiveInfinity(length)) ? Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2)) : length;
        }

        public gVector(gVertex start, gVertex end)
        {
            X = end.X - start.X;
            Y = end.Y - start.Y;
            Z = end.Z - start.Z;
            Length = Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
        }

        public double Dot(gVector vector)
        {
            return (this.X * vector.X) + (this.Y * vector.Y) + (this.Z * vector.Z);
        }

        public double Angle (gVector vector)
        {
            double dot = this.Dot(vector);
            return Math.Acos(dot / (this.Length * vector.Length));
        }

        public gVector Cross(gVector vector)
        {
            double x = (this.Y * vector.Z) - (this.Z * vector.Y);
            double y = (this.Z * vector.X) - (this.X * vector.Z);
            double z = (this.X * vector.Y) - (this.Y * vector.X);
            double angle = this.Angle(vector);
            double length = this.Length * vector.Length * Math.Sin(angle);
            return new gVector(x, y, z, length);
        }

    }
}
