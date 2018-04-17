using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphical.Graphs;
using Graphical.Base;

namespace Graphical
{
    public static class Algorithms
    {
        
        internal static Graph Dijkstra(Graph graph, gVertex origin, gVertex destination, Graph tempGraph = null)
        {
            // TODO: Implement Heap queue
            Dictionary<gVertex, double> dist = new Dictionary<gVertex, double>();
            graph.vertices.Where(v => !v.Equals(origin)).ToList().ForEach(v => dist.Add(v, Double.PositiveInfinity));

            //If tempGraph is not null, means graph doesn't contain origin and/or destination vertices.
            if (graph.Contains(origin))
            {
                dist[origin] = 0;
            }else
            {
                dist.Add(origin, 0);
            }

            if (!graph.Contains(destination)) { dist.Add(destination, Double.PositiveInfinity); }

            Dictionary<gVertex, gVertex> ParentVertices = new Dictionary<gVertex, gVertex>();
            List<gVertex> Q = new List<gVertex>(dist.Keys.ToList());
            List<gVertex> S = new List<gVertex>();

            while (Q.Any())
            {
                Q = Q.OrderBy(v => dist[v]).ToList();
                gVertex vertex = Q.First();
                Q.RemoveAt(0);
                S.Add(vertex);

                if (vertex.Equals(destination)) { break; }

                List<gEdge> edges = new List<gEdge>();
                edges.AddRange(graph.GetVertexEdges(vertex));
                if(tempGraph != null && tempGraph.edges.Any())
                {
                    edges.AddRange(tempGraph.GetVertexEdges(vertex));
                }

                foreach(gEdge e in edges)
                {
                    gVertex w = e.GetVertexPair(vertex);
                    double newLength = dist[vertex] + e.length;
                    
                    if(newLength < dist[w])
                    {
                        dist[w] = newLength;
                        ParentVertices[w] = vertex;
                    }
                }

            }

            Graph path = new Graph();
            gVertex dest = destination;
            while (dest != origin)
            {
                gVertex parent = ParentVertices[dest];
                path.AddEdge(new gEdge(dest, parent));
                dest = parent;
            }

            return path;
            
        }
    }
}
