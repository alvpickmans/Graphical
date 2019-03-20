using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphical.Geometry
{
    /// <summary>
    /// Base abstract class for all spatial geometries
    /// </summary>
    public abstract class Geometry
    {

        #region Properties
        private gBoundingBox _boundingBox;
        private Dictionary<string, object> _userData;
        #endregion

        /// <summary>
        /// Geometry's Axis Aligned Bounding Box
        /// </summary>
        public gBoundingBox BoundingBox
        {
            get
            {
                if(_boundingBox == null) { _boundingBox = ComputeBoundingBox(); }
                return _boundingBox;
            }
            set { _boundingBox = value; }
        }

        /// <summary>
        /// Geometry's Custom data dictionary.
        /// </summary>
        public Dictionary<string, object> UserData
        {
            get
            {
                if(_userData == null) { _userData = new Dictionary<string, object>(); }
                return _userData;
            }
            set { _userData = value; }
        }

        internal abstract gBoundingBox ComputeBoundingBox();

    }
}
