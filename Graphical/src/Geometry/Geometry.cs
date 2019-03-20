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
        private UserData _userData;
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
        public UserData UserData
        {
            get
            {
                if(_userData == null) { _userData = new UserData(); }
                return _userData;
            }
            set { _userData = value; }
        }

        internal abstract gBoundingBox ComputeBoundingBox();

    }
}
