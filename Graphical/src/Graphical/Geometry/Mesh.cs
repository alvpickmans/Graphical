using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DS = Autodesk.DesignScript.Geometry;
using MT = Autodesk.Dynamo.MeshToolkit;

namespace Graphical.Geometry
{
    /// <summary>
    /// Additional tools for Dynamo Mesh
    /// </summary>
    public static class Mesh
    {
        /// <summary>
        /// Returns the BoundingBox of a Dynamo DesignScript Mesh
        /// </summary>
        /// <param name="mesh">Dynamo DesignScript Mesh</param>
        /// <returns name="BoundingBox">Mesh's BoundingBox</returns>
        public static DS.BoundingBox BoundingBox(DS.Mesh mesh)
        {
            IEnumerable<double> x = mesh.VertexPositions.Select(pt => pt.X);
            IEnumerable<double> y = mesh.VertexPositions.Select(pt => pt.Y);
            IEnumerable<double> z = mesh.VertexPositions.Select(pt => pt.Z);
            return DS.BoundingBox.ByCorners(
                DS.Point.ByCoordinates(x.Min(), y.Min(), z.Min()),
                DS.Point.ByCoordinates(x.Max(), y.Max(), z.Max())
            );

        }

        /// <summary>
        /// Creates a Dynamo Mesh by converting a MeshToolkit Mesh
        /// </summary>
        /// <param name="meshToolkit">MeshToolkit Mesh</param>
        /// <returns name = "mesh">Dynamo Mesh</returns>
        public static DS.Mesh ByMeshToolkit(MT.Mesh meshToolkit)
        {
            List<DS.Point> vertices = meshToolkit.Vertices();
            List<int> vertexIndex = meshToolkit.VertexIndicesByTri();
            var setOfIndexes = vertexIndex
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / 3)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
            List<DS.IndexGroup> indexGroups = new List<DS.IndexGroup>();
            foreach (var ind in setOfIndexes)
            {
                indexGroups.Add(DS.IndexGroup.ByIndices((uint)ind[0], (uint)ind[1], (uint)ind[2]));
            }

            return DS.Mesh.ByPointsFaceIndices(vertices, indexGroups);
        }

    }
}
