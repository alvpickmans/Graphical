using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using BoundingBox = Autodesk.DesignScript.Geometry.BoundingBox;

namespace Graphical.Core.Octree
{
    public class OctreeNode :IGraphicItem
    {
        #region Internal Variables
        internal BoundingBox bbox;
        internal int depth;
        internal IList<int> triangle_Idx = new List<int>();
        internal OctreeNode[] children = null;
        #endregion

        #region Public Variables
        public BoundingBox BoundingBox => this.bbox;
        public int Depth => depth;
        public IList<int> TriangleIndexes => triangle_Idx;
        public OctreeNode[] Children => children;
        public bool isLeaf() => children == null;

        public bool isEmpty() => !triangle_Idx.Any();

        public bool isValid() => isLeaf() ? !isEmpty() : isEmpty();

        public int itemCount() => triangle_Idx.Count();
        #endregion


        #region Constructor
        internal OctreeNode(int depth, BoundingBox bbox)
        {
            this.depth = depth;
            this.bbox = bbox;
        }
        #endregion

        #region Public Methods
        
        public double DistanceTo(Autodesk.DesignScript.Geometry.Geometry geometry)
        {
           return geometry.DistanceTo(Geometry.Point.MidPoint(bbox.MinPoint, bbox.MaxPoint));
           
        }
        
        #endregion

        #region Internal Methods
        internal List<OctreeNode> getNodes_recursive()
        {
            List<OctreeNode> nodes = new List<OctreeNode>() { this };
            if (this.isLeaf()) { return nodes; }
            foreach (OctreeNode child in children)
            {
                if (child != null)
                {
                    nodes.AddRange(child.getNodes_recursive());
                }
            }
            return nodes;
        }

        [IsVisibleInDynamoLibrary(false)]
        public void Tessellate(IRenderPackage package, TessellationParameters parameters)
        {
            package.RequiresPerVertexColoration = true;
            Autodesk.DesignScript.Geometry.Geometry geometry = bbox.ToCuboid();
            byte[] color = new byte[] { 0, 200, 200, 50 };
            // As you add more data to the render package, you need
            // to keep track of the index where this coloration will 
            // start from.

            geometry.Tessellate(package, parameters);

            if (package.MeshVertexCount > 0)
            {
                package.ApplyMeshVertexColors(CreateColorByteArrayOfSize(package.MeshVertexCount, color[0], color[1], color[2], color[3]));
            }

            geometry.Dispose();
        }

        private static byte[] CreateColorByteArrayOfSize(int size, byte red, byte green, byte blue, byte alpha)
        {
            var arr = new byte[size * 4];
            for (var i = 0; i < arr.Count(); i += 4)
            {
                arr[i] = red;
                arr[i + 1] = green;
                arr[i + 2] = blue;
                arr[i + 3] = alpha;
            }
            return arr;
        }



        #endregion

    }
}
