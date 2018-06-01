using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphical.Geometry
{
    public abstract class gBase
    {
        #region Constants
        const double EPS = 1e-5;
        #endregion

        #region Properties
        internal double? thresholdOverride { get; private set; }
        internal int thresholdDecimals { get; private set; }
        #endregion

        public  void ThresholdOverride(double value)
        {
            thresholdOverride = value;
            decimal d = Convert.ToDecimal(thresholdOverride);
            thresholdDecimals = BitConverter.GetBytes(decimal.GetBits(d)[3])[2];
        }

        public static bool Threshold(double value1, double value2)
        {
            if(thresholdOverride == null)
            {
                return Math.Abs(value1 - value2) <= EPS;
            }else
            {
                bool eq = Math.Abs(value1 - value2) <= thresholdOverride;
                return eq;
            }
        }

        public static double Round(double value, int decimals = 6)
        {
            return Math.Round(value, decimals);
        }

        public static double ToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }

        public static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}
