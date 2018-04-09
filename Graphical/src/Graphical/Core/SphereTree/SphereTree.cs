using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Geometry;
using Autodesk.Dynamo.MeshToolkit;
using Dynamo.Graph.Nodes;
using MeshToolkit = Autodesk.Dynamo.MeshToolkit.Mesh;

namespace Graphical.Core.SphereTree
{
    public class SphereTree
    {

        #region Constants

        #endregion

        #region Internal Variables
        internal MeshToolkit _mesh;
        internal IList<Point> vertices;
        internal List<List<int>> vertexIndexByTri;
        internal IList<SphereNode> nodes = new List<SphereNode>();
        internal int maxLevel;
        #endregion

        #region Public Variables
        public IList<SphereNode> SphereNodes => this.nodes;
        #endregion

        #region Internal Constructor

        internal SphereTree(MeshToolkit mesh)
        {
            this._mesh = mesh;
            this.vertices = _mesh.Vertices();
            this.vertexIndexByTri = List.Chop<int>(_mesh.VertexIndicesByTri(), 3);
            CreateSphereNodes();
        }

        #endregion

        #region Public Constructor


        public static SphereTree ByMesh (MeshToolkit meshToolkit)
        {
            return new SphereTree(meshToolkit);
        }


        #endregion


        #region Internal Methods

        internal bool CreateSphereNodes()
        {
            for(int i = 0; i < this._mesh.TriangleCount; i++)
            {
                List<int> triIndexes = vertexIndexByTri[i];
                SphereNode sp = SphereNode.ByThreePoints(this.vertices[triIndexes[0]], this.vertices[triIndexes[1]], this.vertices[triIndexes[2]]);
                SaveTriangleToNode(sp, i);
                this.nodes.Add(sp);
            }
            return true;
        }
        internal bool SaveTriangleToNode(SphereNode sn, int idx)
        {
            if (!sn.triangle_Idx.Contains(idx))
            {
                sn.triangle_Idx.Add(idx);
            }
            return true;
        }



        #endregion

        #region Public Methods
        public static IList<SphereNode> SphereNodesByMesh(MeshToolkit mesh)
        {
            List<Point> vertices = mesh.Vertices();
            List<int> vertexIndex = mesh.VertexIndicesByTri();
            var setOfIndexes = Core.List.Chop<int>(vertexIndex, 3);
            List<SphereNode> sphereNodes = new List<SphereNode>();

            foreach (var ind in setOfIndexes)
            {
                sphereNodes.Add(SphereNode.ByThreePoints(vertices[ind[0]], vertices[ind[1]], vertices[ind[2]]));
            }

            return sphereNodes;
        }

        //TODO: Simplify method to translate point without using point geometries.
        public static SphereNode BoundingSphereNode(SphereNode sphere1, SphereNode sphere2)
        {
            SphereNode minSp = (sphere1.radius < sphere2.radius) ? sphere1 : sphere2;
            SphereNode maxSp = (sphere1.radius < sphere2.radius) ? sphere2 : sphere1;
            double centersDistance = DistanceBetweenCenters(minSp, maxSp);
            double[] vectorCoord = minSp.center.Zip(maxSp.center, (sp1, sp2) => sp2 - sp1).ToArray();
            using (Point minCenter = SphereNode.Center(minSp))
            using (Point maxCenter = SphereNode.Center(maxSp))
            {
                Vector v = Vector.ByTwoPoints(minCenter, maxCenter);
                double translationDist = (centersDistance + (maxSp.radius - minSp.radius)) * 0.5;

                Point center = (Point)minCenter.Translate(v, translationDist);
                double radius = (centersDistance + minSp.radius + maxSp.radius) * 0.5;
                return new SphereNode(new double[3] { center.X, center.Y, center.Z }, radius);
            }
        }

        public static double DistanceBetweenCenters(SphereNode sphere1, SphereNode sphere2)
        {
            double[] sq = sphere1.center.Zip(sphere2.center, (s1, s2) => Math.Pow((s2 - s1), 2)).ToArray();
            return Math.Sqrt(sq.Sum());
        }

        #endregion

    } 
   
}
