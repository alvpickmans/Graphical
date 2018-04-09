using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Geometry;
using Autodesk.Dynamo.MeshToolkit;
using Dynamo.Graph.Nodes;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;

namespace Graphical.Core.SphereTree
{
    public class SphereNode : IGraphicItem
    {
        #region Internal Variables
        internal double[] center = new double[3];
        internal double radius { get; private set; }
        internal int level { get; private set; }
        internal List<int> triangle_Idx = new List<int>();
        internal SphereNode[] children;
        internal SphereNode parent;
        #endregion

        #region Public Variables
        public double Radius => this.radius;
        public List<int> TriangleIndices => this.triangle_Idx;

        #endregion

        #region Internal Constructor
        internal SphereNode(double[] _center, double _radius)
        {
            center = _center;
            radius = _radius;
        } 
        #endregion
        //TODO: Improve getting center coordinate
        public static SphereNode ByThreePoints(Point point1, Point point2, Point point3)
        {
            var points = new Point[3] { point1, point2, point3 };
            using (Polygon pol = Polygon.ByPoints(points))
            using (Point c = pol.Center())
            {
                var center = new double[3] { c.X, c.Y, c.Z };
                double radius = points.Select(pt => c.DistanceTo(pt)).Max();
                return new SphereNode(center, radius);
            }
        }
        

        [NodeCategory("Query")]
        public static Point Center(SphereNode sphereNode)
        {
            return Point.ByCoordinates(sphereNode.center[0], sphereNode.center[1], sphereNode.center[2]);
        }


        [IsVisibleInDynamoLibrary(false)]
        public void Tessellate(IRenderPackage package, TessellationParameters parameters)
        {
            package.RequiresPerVertexColoration = true;
            Point center = SphereNode.Center(this);
            Autodesk.DesignScript.Geometry.Geometry geometry = Sphere.ByCenterPointRadius(center, this.radius);
            byte[] color = new byte[] { 0, 200, 200, 50 };
            // As you add more data to the render package, you need
            // to keep track of the index where this coloration will 
            // start from.

            geometry.Tessellate(package, parameters);

            if (package.MeshVertexCount > 0)
            {
                package.ApplyMeshVertexColors(CreateColorByteArrayOfSize(package.MeshVertexCount, color[0], color[1], color[2], color[3]));
            }
            center.Dispose();
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
    }
}
