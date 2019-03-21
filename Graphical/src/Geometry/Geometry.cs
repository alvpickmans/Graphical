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
    public abstract class Geometry : IEquatable<Geometry>
    {
        #region Static Properties
        private static int _nextId = 1;

        public static int NextId
        {
            get => Geometry._nextId++;
        }
        #endregion

        #region Properties
        private BoundingBox _boundingBox;
        private UserData _userData;

        private Geometry _parent;
        #endregion

        #region Public Properties
        public int Id { get; set; }

        /// <summary>
        /// Geometry's Axis Aligned Bounding Box
        /// </summary>
        public BoundingBox BoundingBox
        {
            get
            {
                if (_boundingBox == null) { _boundingBox = ComputeBoundingBox(); }
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
                if (_userData == null) { _userData = new UserData(); }
                return _userData;
            }
            set { _userData = value; }
        }

        public Geometry Parent
        {
            get => this._parent;
            set => this._parent = value;
        }
        #endregion

        #region Public Constructor
        public Geometry()
        {
            this.Id = Geometry.NextId;
        }
        #endregion

        internal abstract BoundingBox ComputeBoundingBox();

        public bool Equals(Geometry other)
        {
            return this.GetType() == other.GetType()
                && this.Id == other.Id;
        }
    }
}
