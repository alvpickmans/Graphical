using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Graphical.Core;

namespace Graphical.Geometry
{
    /// <summary>
    /// Polygon class to hold graph´s polygon information in relation to its function on the graph
    /// like if it is internal or limit boundary.
    /// </summary>
    public class Polygon : Geometry, ICloneable
    {
        #region Internal Variables

        /// <summary>
        /// Flag to check _polygonsDict role: Internal or Boundary
        /// </summary>
        internal bool isBoundary { get; set; }

        /// <summary>
        /// Polygon's Edges
        /// </summary>
        internal List<Edge> edges = new List<Edge>();

        /// <summary>
        /// Polygon's Vertices
        /// </summary>
        internal List<Vertex> vertices = new List<Vertex>();
        #endregion

        #region Public Variables
        /// <summary>
        /// Polygon's Vertices
        /// </summary>
        public List<Vertex> Vertices
        {
            get { return vertices; }
        }

        /// <summary>
        /// Polygon's Edges
        /// </summary>
        public List<Edge> Edges
        {
            get
            {
                return edges;
            }
        }

        /// <summary>
        /// Determines if the Polygon is closed.
        /// </summary>
        public bool IsClosed
        {
            get
            {
                return this.edges.Count > 2 && (edges.First().StartVertex.OnEdge(edges.Last()) || edges.First().EndVertex.OnEdge(edges.Last()));
            }
        }
        #endregion

        #region Internal Constructors
        internal Polygon(bool isBoundary = false) : base() { }

        internal Polygon(List<Vertex> vertices, bool _isExternal = false) : base()
        {
            isBoundary = _isExternal;
            int vertexCount = vertices.Count;
            for (var j = 0; j < vertexCount; j++)
            {
                int next_index = (j + 1) % vertexCount;
                Vertex vertex = vertices[j];
                Vertex next_vertex = vertices[next_index];
                var edge = Edge.ByStartVertexEndVertex(vertex, next_vertex);

                vertex.Parent = this;
                edge.Parent = this;

                this.Vertices.Add(vertex);
                this.Edges.Add(edge);
            }
        }
        #endregion

        #region Public Constructos
        /// <summary>
        /// Creates a new Polygon by a list of ordered Vertices.
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="isExternal"></param>
        /// <returns></returns>
        public static Polygon ByVertices(List<Vertex> vertices, bool isExternal = false)
        {

            Polygon polygon;
            // Removing last vertex if is equal to first.
            if (vertices.First().Equals(vertices.Last()))
            {
                polygon = new Polygon(vertices.Take(vertices.Count - 1).ToList(), isExternal);
            }
            else
            {
                polygon = new Polygon(vertices, isExternal);
            }
            return polygon;
        }

        public static Polygon ByCenterRadiusAndSides(Vertex center, double radius, int sides)
        {
            // TODO: create polygon by plane?
            if(sides < 3) { throw new ArgumentOutOfRangeException("sides", "Any polygon must have at least 3 sides."); }
            List<Vertex> vertices = new List<Vertex>();
            double angle = (Math.PI * 2) / sides;
            for(var i = 0; i < sides; i++)
            {
                var vertex = Vertex.ByCoordinates(
                        (Math.Sin(i * angle) * radius) + center.X,
                        (Math.Cos(i * angle) * radius) + center.Y,
                        center.Z
                        );
                vertices.Add(vertex);
            }
            return new Polygon(vertices);
        }
        #endregion

        #region Internal Methods
        internal void AddVertex(Vertex vertex)
        {
            vertex.Parent = this;
            vertices.Add(vertex);
        }

        internal Polygon AddVertex(Vertex vertex, Edge intersectinEdge)
        {
            //Assumes that vertex v intersects one of _polygonsDict Edges.
            Polygon newPolygon = (Polygon)this.Clone();

            // Assign the polygon Id to the new vertex.
            vertex.Parent = this;

            // Getting the index of the intersecting edge's start vertex and
            // inserting the new vertex at the following index.
            int index = newPolygon.vertices.IndexOf(intersectinEdge.StartVertex);
            newPolygon.vertices.Insert(index + 1, vertex);

            // Rebuilding Edges.
            newPolygon.edges.Clear();
            int verticesCount = newPolygon.vertices.Count;
            for (var i = 0; i < verticesCount; i++)
            {
                int nextIndex = (i + 1) % verticesCount;
                newPolygon.edges.Add(Edge.ByStartVertexEndVertex(newPolygon.vertices[i], newPolygon.vertices[nextIndex]));
            }

            return newPolygon;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines if a Vertex is inside the Polygon using Fast Winding Number method
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public bool ContainsVertex(Vertex vertex)
        {
            // http://geomalgorithms.com/a03-_inclusion.html
            Vertex maxVertex = vertices.OrderByDescending(v => v.DistanceTo(vertex)).First();
            double maxDistance = vertex.DistanceTo(maxVertex) * 1.5;
            Vertex v2 = Vertex.ByCoordinates(vertex.X + maxDistance, vertex.Y, vertex.Z);
            Edge ray = Edge.ByStartVertexEndVertex(vertex, v2);
            int windNumber = 0;
            foreach (Edge edge in edges)
            {
                if(vertex.OnEdge(edge)) { return true; }
                Vertex intersection = ray.Intersection(edge) as Vertex;
                if (intersection is Vertex)
                {
                    if (edge.StartVertex.Y <= vertex.Y)
                    {
                        if (edge.EndVertex.Y > vertex.Y)
                        {
                            if(vertex.IsLeftFrom(edge) > 0) {
                                ++windNumber;
                            }
                        }
                    }
                    else
                    {
                        if (edge.EndVertex.Y < vertex.Y)
                        {
                            if(vertex.IsLeftFrom(edge) < 0)
                            {
                                --windNumber;
                            }
                        }
                    }
                }
            }

            // If windNumber is different from 0, vertex is in polygon
            return windNumber != 0;
        }

        /// <summary>
        /// Determines if a Edge is inside the Polygon by comparing
        /// it's start, end and mid Vertices.
        /// Note: Prone to error if polygon has Edges intersecting the edge not at mid vertex?
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool ContainsEdge(Edge edge)
        {
            // TODO: Check if edge intersects polygon in Vertices different than start/end.
            return this.ContainsVertex(edge.StartVertex)
                && this.ContainsVertex(edge.EndVertex)
                && this.ContainsVertex(Vertex.MidVertex(edge.StartVertex, edge.EndVertex));
        }
        /// <summary>
        /// Checks if a polygon is planar
        /// </summary>
        /// <returns>boolean</returns>
        public bool IsPlanar()
        {
            return Vertex.Coplanar(this.Vertices);
        }

        /// <summary>
        /// Checks if two Polygons are coplanar.
        /// </summary>
        /// <param name="polygon">Other Polygon</param>
        /// <returns></returns>
        public bool AreCoplanar(Polygon polygon)
        {
            List<Vertex> joinedVertices = new List<Vertex>(this.Vertices);
            joinedVertices.AddRange(polygon.Vertices);

            return Vertex.Coplanar(joinedVertices);
        }


        ///// <summary>
        ///// Determines if two _polygonsDict are intersecting
        ///// </summary>
        ///// <param name="polygon"></param>
        ///// <returns></returns>
        //public bool Intersects(Polygon polygon)
        //{
        //    if (!this.BoundingBox.Intersects(polygon.BoundingBox)) { return false; }
        //    var sw = new SweepLine(this, polygon, SweepLineType.Intersects);
        //    return sw.HasIntersection();
        //}
        
        ///// <summary>
        ///// Performes a Union boolean operation between this polygon and a clipping one.
        ///// </summary>
        ///// <param name="clip"></param>
        ///// <returns></returns>
        //public List<Polygon> Union(Polygon clip)
        //{
            
        //    var swLine = new SweepLine(this, clip, SweepLineType.Boolean);

        //    return swLine.ComputeBooleanOperation(BooleanType.Union);
        //}

        //public static List<Polygon> Union(List<Polygon> subjects, List<Polygon> clips)
        //{
        //    List<Polygon> result = new List<Polygon>(subjects);
        //    int count = 0;
        //    foreach (Polygon clip in clips)
        //    {
        //        for (var i = count; i < result.Count; i++)
        //        {
        //            result.AddRange(result[i].Union(clip));
        //            count++;
        //        }
        //    }
        //    return result;
        //}

        ///// <summary>
        ///// Performes a Difference boolean operation between this polygon and a clipping one.
        ///// </summary>
        ///// <param name="clip"></param>
        ///// <returns></returns>
        //public List<Polygon> Difference(Polygon clip)
        //{
        //    var swLine = new SweepLine(this, clip, SweepLineType.Boolean);

        //    return swLine.ComputeBooleanOperation(BooleanType.Differenece);
        //}

        //public static List<Polygon> Difference(List<Polygon> subjects, List<Polygon> clips)
        //{
        //    List<Polygon> result = new List<Polygon>(subjects);
        //    int count = 0;
        //    foreach (Polygon clip in clips)
        //    {
        //        for(var i = count; i < result.Count; i++)
        //        {
        //            result.AddRange(result[i].Difference(clip));
        //            count++;
        //        }
        //    }
        //    return result;
        //}

        ///// <summary>
        ///// Performes a Intersection boolean operation between this polygon and a clipping one.
        ///// </summary>
        ///// <param name="clip"></param>
        ///// <returns></returns>
        //public List<Polygon> Intersection(Polygon clip)
        //{
        //    var swLine = new SweepLine(this, clip, SweepLineType.Boolean);

        //    return swLine.ComputeBooleanOperation(BooleanType.Intersection);
        //}

        //public static List<Polygon> Intersection(List<Polygon> subjects, List<Polygon> clips)
        //{
        //    List<Polygon> result = new List<Polygon>(subjects);
        //    int count = 0;
        //    foreach (Polygon clip in clips)
        //    {
        //        for (var i = count; i < result.Count; i++)
        //        {
        //            result.AddRange(result[i].Intersection(clip));
        //            count++;
        //        }
        //    }
        //    return result;
        //}

        #endregion

        /// <summary>
        /// Clone method for Polygon
        /// </summary>
        /// <returns>Cloned Polygon</returns>
        public object Clone()
        {
            Polygon newPolygon = new Polygon(this.vertices, this.isBoundary);
            newPolygon.edges = new List<Edge>(this.edges);
            return newPolygon;
        }

        internal override BoundingBox ComputeBoundingBox()
        {
            var xCoord = new List<double>(this.vertices.Count);
            var yCoord = new List<double>(this.vertices.Count);
            var zCoord = new List<double>(this.vertices.Count);
            foreach(Vertex v in vertices)
            {
                xCoord.Add(v.X);
                yCoord.Add(v.Y);
                zCoord.Add(v.Z);
            }
            return new BoundingBox(xCoord, yCoord, zCoord);
        }
    }
}
