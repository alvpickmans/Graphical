using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DS = Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

namespace Graphical.Geometry
{
    /// <summary>
    /// Class to perform Voxelation of Meshes
    /// </summary>
    public static class Voxelate
    {
        #region Public Methods
        /// <summary>
        /// Returns the maximize BoundinBox by the vector size.
        /// </summary>
        /// <param name="boundingBox"></param>
        /// <param name="vectorSize"></param>
        /// <returns></returns>
        public static DS.BoundingBox MaxBoundingBox(DS.BoundingBox boundingBox, DS.Vector vectorSize)
        {
            return DS.BoundingBox.ByCorners(
                MaxMinBound(Math.Floor, boundingBox.MinPoint, vectorSize),
                MaxMinBound(Math.Ceiling, boundingBox.MaxPoint, vectorSize)
            );
        }

        /// <summary>
        /// Returns the maximum vector size for the given BoundingBox
        /// </summary>
        /// <param name="boundingBox"></param>
        /// <param name="vectorSize"></param>
        /// <returns name="vectorSize">Maximum vecotr size for the given BoundinBox</returns>
        public static DS.Vector MaxVectorSize(DS.BoundingBox boundingBox, DS.Vector vectorSize)
        {
            using(DS.Vector diagonal = DS.Vector.ByTwoPoints(boundingBox.MinPoint, boundingBox.MaxPoint))
            {
                double x = diagonal.X / (Math.Ceiling(diagonal.X / vectorSize.X));
                double y = diagonal.Y / (Math.Ceiling(diagonal.Y / vectorSize.Y));
                double z = diagonal.Z / (Math.Ceiling(diagonal.Z / vectorSize.Z));
                return DS.Vector.ByCoordinates(x, y, z);
            }
        }

        /// <summary>
        /// Divides a BoundingBox into a grid of voxels.
        /// </summary>
        /// <param name="boundingBox"></param>
        /// <param name="vectorSize"></param>
        /// <returns></returns>
        public static List<DS.Point> BoundingBoxToVoxels(DS.BoundingBox boundingBox, DS.Vector vectorSize)
        {
            DS.Point minPt = boundingBox.MinPoint;
            List<DS.Point> VoxelPoints = new List<DS.Point>();
            using(DS.Vector diagonal = DS.Vector.ByTwoPoints(boundingBox.MinPoint, boundingBox.MaxPoint))
            {
                int xMax = Convert.ToInt32(Math.Abs(diagonal.X / vectorSize.X));
                int yMax = Convert.ToInt32(Math.Abs(diagonal.Y / vectorSize.Y));
                int zMax = Convert.ToInt32(Math.Abs(diagonal.Z / vectorSize.Z));
                var coordinates = from x in Enumerable.Range(0, xMax)
                                  from y in Enumerable.Range(0, yMax)
                                  from z in Enumerable.Range(0, zMax)
                                  select new { x, y, z };
                foreach(var coord in coordinates)
                {
                    double x = (coord.x * vectorSize.X + minPt.X) + vectorSize.X * 0.5;
                    double y = (coord.y * vectorSize.Y + minPt.Y) + vectorSize.Y * 0.5;
                    double z = (coord.z * vectorSize.Z + minPt.Z) + vectorSize.Z * 0.5;
                    VoxelPoints.Add(DS.Point.ByCoordinates(x, y, z));
                }
            }

            return VoxelPoints;
        }



        #endregion

        #region Private Methods
        /// <summary>
        /// Returns the maximum or minimum bounding box corner depending of the rounding function.
        /// </summary>
        /// <param name="roundingFunction">Mat.Ceiling or Math.Floor. Any other will throw and exception</param>
        /// <param name="point"></param>
        /// <param name="vectorSize"></param>
        /// <returns></returns>
        private static DS.Point MaxMinBound(Func<double, double> roundingFunction, DS.Point point, DS.Vector vectorSize)
        {
            //if any vector component less or equal 0, original coordinated is used.
            double x = (vectorSize.X <= 0) ? point.X : roundingFunction(point.X / vectorSize.X) * vectorSize.X;
            double y = (vectorSize.Y <= 0) ? point.Y : roundingFunction(point.Y / vectorSize.Y) * vectorSize.Y;
            double z = (vectorSize.Z <= 0) ? point.Z : roundingFunction(point.Z / vectorSize.Z) * vectorSize.Z;

            return DS.Point.ByCoordinates(x, y, z);
        } 
        #endregion
    }
}
