using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DS = Autodesk.DesignScript.Geometry;

namespace Graphical.Geometry
{
    public static class Visibility
    {

        public static List<DS.Surface> SurfacesFromPointAndDirection (List<DS.Surface> surfaces, DS.Point viewPoint, DS.Point targetPoint)
        {
            DS.Vector viewVector = DS.Vector.ByTwoPoints(viewPoint, targetPoint).Normalized();
            List<DS.Surface> visibleSurfaces = new List<DS.Surface>();

            foreach(var surface in surfaces)
            {
                DS.Vector normal = surface.NormalAtParameter(0.5, 0.5);
                double dot = normal.Dot(viewVector);
                if(Math.Round(dot, 6) <= 0) { visibleSurfaces.Add(surface); }
            }

            return visibleSurfaces;
        }

    }
}
