using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphical
{
    public static class Utils
    {
        #region Constants
        const double threshold = 0.0001;
        #endregion

        public static bool Threshold(double value1, double value2)
        {
            return Math.Abs(value1 - value2) <= threshold;
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
