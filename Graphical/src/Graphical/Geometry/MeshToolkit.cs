using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DS = Autodesk.DesignScript.Geometry;
using MT = Autodesk.Dynamo.MeshToolkit;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;

namespace Graphical.Geometry
{
    /// <summary>
    /// Additional tools for MeshToolKit
    /// </summary>
    public static class MeshToolkit
    {
        /// <summary>
        /// Returns the BoundingBox of a MeshToolkit Mesh
        /// </summary>
        /// <param name="mesh">MeshToolkit Mesh</param>
        /// <returns name="BoundingBox">Mesh's BoundingBox</returns>
        public static DS.BoundingBox BoundingBox(MT.Mesh mesh)
        {
            IEnumerable<double> x = mesh.Vertices().Select(pt => pt.X);
            IEnumerable<double> y = mesh.Vertices().Select(pt => pt.Y);
            IEnumerable<double> z = mesh.Vertices().Select(pt => pt.Z);
            return DS.BoundingBox.ByCorners(
                DS.Point.ByCoordinates(x.Min(), y.Min(), z.Min()),
                DS.Point.ByCoordinates(x.Max(), y.Max(), z.Max())
            );
        }

        /// <summary>
        /// Creates a MeshToolkit Mesh by converting a Dynamo Mesh
        /// </summary>
        /// <param name="mesh">Dynamo Mesh</param>
        /// <returns name = "meshToolkit">MeshToolkit Mesh</returns>
        public static MT.Mesh ByDynamoMesh(DS.Mesh mesh)
        {
            var vertices = mesh.VertexPositions;
            var indexGroups = mesh.FaceIndices;
            List<int> indexes = new List<int>();

            foreach (var ind in indexGroups)
            {
                indexes.Add((int)ind.A);
                indexes.Add((int)ind.B);
                indexes.Add((int)ind.C);
            }

            return MT.Mesh.ByVerticesAndIndices(vertices, indexes);
        }

    }

    

}
