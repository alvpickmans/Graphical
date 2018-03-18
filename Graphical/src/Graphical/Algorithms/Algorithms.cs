using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphical.Graphs;
using Graphical.Base;

namespace Graphical.Algorithms
{
    public class Dijkstra
    {
        public Dictionary<gVertex, double> dist = new Dictionary<gVertex, double>();

        public Dijkstra(Graph graph, gVertex origin, gVertex destination)
        {
            dist.Add(origin, 0);
            graph.vertices.ForEach(v => { if (!v.Equals(origin)) { dist.Add(v, Double.PositiveInfinity); } });
        }
    }
}
