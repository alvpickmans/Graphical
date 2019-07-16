using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphical.Extensions;

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

        #region Public Constructors
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
                        (Math.Cos(i * angle) * radius) + center.X,
                        (Math.Sin(i * angle) * radius) + center.Y,
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

        internal Ray RayByVertexIndex(int start, int end, bool shiftVertices = false)
        {
            var vertexCount = this.Vertices.Count;
            if(start < 0 || start > vertexCount - 1) throw new ArgumentOutOfRangeException(nameof(start));
            if (end < 0 || end > vertexCount - 1) throw new ArgumentOutOfRangeException(nameof(end));
            if (start == end) throw new ArgumentException("Cannot create a Ray out of the same vertex");

            if (shiftVertices)
            {
                start = (start + 1) % this.Vertices.Count();
                end = (end + 1) % this.vertices.Count();
            }

            return Ray.ByTwoVertices(this.Vertices[start], this.Vertices[end]);
        }


        /// <summary>
        /// Returns the previous and next vertices of the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        internal Vertex[] GetAdjacentVertices(Vertex vertex)
        {
            int index = this.vertices.IndexOf(vertex);
            if (index == -1)
            {
                throw new ArgumentException("Vertex does not belong to polygon", "vertex");
            }

            int nextIndex = (index + 1) % vertices.Count();
            int prevIndex = index == 0 ? vertices.Count() - 1 : index - 1;

            return new Vertex[2] { this.vertices[prevIndex], this.vertices[nextIndex] };
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
            Ray ray = Ray.XAxis(vertex);
            int windNumber = 0;
            foreach (Edge edge in edges)
            {
                if(vertex.OnEdge(edge)) { return true; }
                Vertex intersection = ray.Intersection(edge) as Vertex;
                if (intersection != null)
                {
                    if (edge.StartVertex.Y <= vertex.Y)
                    {
                        if (edge.EndVertex.Y > vertex.Y)
                        { 
                            if(vertex.IsCounterClockwise(edge.StartVertex, edge.Direction)) 
                                ++windNumber;
                        }
                    }
                    else
                    {
                        if (edge.EndVertex.Y < vertex.Y)
                        {
                            if(vertex.IsClockwise(edge.StartVertex, edge.Direction))
                                --windNumber;
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

        /// <summary>
        /// Checks if the polygon is convex
        /// </summary>
        /// <returns></returns>
        public bool IsConvex()
        {
            //https://math.stackexchange.com/questions/1743995/determine-whether-a-polygon-is-convex-based-on-its-vertices/1745427#1745427
            double wSign = 0; // First  non-zero orientation

            int xSign = 0;
            int xFirstSign = 0;
            int xFlips = 0; 

            int ySign = 0;
            int yFirstSign = 0;
            int yFlips = 0;

            int vertexCount = this.Vertices.Count();
            Vertex current = this.Vertices[vertexCount - 2];
            Vertex next = this.Vertices[vertexCount - 1];

            for (int i = 0; i < vertexCount; i++)
            {
                var prev = current;
                current = next;
                next = this.Vertices[i];

                // Previous edge vector
                double prevX = current.X - prev.X;
                double prevY = current.Y - prev.Y;

                // Next edge vector
                double nextX = next.X - current.X;
                double nextY = next.Y - current.Y;

                if (!nextX.AlmostEqualTo(0))
                {
                    if(nextX > 0)
                    {
                        if(xSign == 0)
                             xFirstSign = 1;

                        else if(xSign < 0)
                            xFlips += 1;

                        xSign = 1;
                    }
                    else // nextX < 0
                    {
                        if(xSign == 0)
                            xFirstSign = -1;

                        else if(xSign > 0)
                            xFlips += 1;

                        xSign = -1;
                   }
                }

                if(xFlips > 2) { return false; }

                if (!nextY.AlmostEqualTo(0))
                {
                    if(nextY > 0)
                    {
                        if(ySign == 0)
                            yFirstSign = 1;

                        else if(ySign < 0)
                            yFlips += 1;

                        ySign = 1;
                    }
                    else // nextY < 0
                    {
                        if (ySign == 0)
                            yFirstSign = -1;

                        else if (ySign > 0)
                            yFlips += 1;

                        ySign = -1;
                    }
                }

                if(yFlips > 2)
                    return false;

                // Find out the orientation of this pair of edges
                // and ensure ir does not differ from previous ones

                double w = prevX * nextY - nextX * prevY;
                bool wIsZero = w.AlmostEqualTo(0);
                bool wSignIsZero = wSign.AlmostEqualTo(0);

                if(wSignIsZero && !wIsZero)
                {
                    wSign = w;
                }
                else if(!wSignIsZero && !wIsZero)
                {
                    if((wSign > 0 && w < 0) || (wSign > 0 && w < 0))
                        return false;
                }
            }

            // Final/wraparound sign flips
            if(xSign != 0 && xFirstSign != 0 && xSign != xFirstSign)
                xFlips += 1;

            if(ySign != 0 && yFirstSign != 0 && ySign != yFirstSign)
                yFlips += 1;

            // Concave polygons have two sign flips along each axis
            if( xFlips != 2 || yFlips != 2)
                return false;

            // This is a convex polygon
            return true;
        }

        private List<Geometry> IntersectionNaive(Edge edge)
        {
            List<Geometry> intersections = new List<Geometry>();

            if (!this.BoundingBox.Intersects(edge.BoundingBox))
                return intersections;

            var vertexIntersections = new List<Vertex>();
            var edgeIntersections = new List<Edge>();

            for (int i = 0; i < this.Edges.Count; i++)
            {
                var side = this.Edges[i];
                var intersection = edge.Intersection(side);

                if (intersection is Edge edgeInt && !edgeIntersections.Contains(edgeInt))
                    edgeIntersections.Add(edgeInt);

                if (intersection is Vertex vertexInt && !vertexIntersections.Contains(intersection))
                    vertexIntersections.Add(vertexInt);
            }

            intersections.AddRange(edgeIntersections);
            // These clean vertices are those that aren't included in any intersecting edge.
            var cleanVertices = vertexIntersections.Where(v => !edgeIntersections.Any(e => e.Contains(v)));
            intersections.AddRange(cleanVertices);

            return intersections;
        }

        /// <summary>
        /// Returns a list of geometries representing the intersection of the edge with the polygon.
        /// If polygon is concave, the returned list can be a mixture of several edges and vertices
        /// </summary>
        /// <param name="edge">Edge to intersect with polygon</param>
        /// <returns>List of vertices and/or edges, or emtpy if not intersecting.</returns>
        public List<Geometry> Intersection(Edge edge)
        {
            // No fast algorithm to calculate intersection on concave polygons
            if (!this.IsConvex() || this.Vertices.Count < 4)
                return this.IntersectionNaive(edge);

            //https://stackoverflow.com/questions/4497841/asymptotically-optimal-algorithm-to-compute-if-a-line-intersects-a-convex-polygo
            List<Geometry> intersections = new List<Geometry>();

            if (!this.BoundingBox.Intersects(edge.BoundingBox))
                return intersections;

            var vertexCount = this.Vertices.Count;
            int midIndex = (int)(vertexCount / 2);
            bool isFirstVertexInEdge = edge.Contains(this.Vertices[0]);
            Ray diagonal = this.RayByVertexIndex(0, midIndex, isFirstVertexInEdge);

            bool PolygonCW = this.Vertices[1].IsClockwise(diagonal.Origin, diagonal.Direction);

            double offset;
            bool doesIntersect = diagonal.TryIntersectionOffset(edge, out offset) 
                && (Double.IsInfinity(offset) || offset.InRange(0, 1));

            while(!doesIntersect)
            {
                // If midIndex is any neighbour from the start vertex
                // means the whole line is to one side or the other and doesn't intersect.
                if(midIndex == 1 || midIndex == vertexCount - 1)
                    return intersections;

                bool startCW = edge.StartVertex.IsClockwise(diagonal.Origin, diagonal.Direction);
                bool endCW = edge.EndVertex.IsClockwise(diagonal.Origin, diagonal.Direction);

                //TODO: This case requires more thought
                if(startCW != endCW)
                    return this.IntersectionNaive(edge);

                if (startCW == PolygonCW)
                    midIndex = (int)Math.Ceiling(midIndex / 2.0);
                else
                    midIndex += (int)(vertexCount - midIndex) / 2;

                diagonal = this.RayByVertexIndex(0, midIndex, isFirstVertexInEdge);
                doesIntersect = diagonal.TryIntersectionOffset(edge, out offset)
                    && (Double.IsInfinity(offset) || offset.InRange(0, 1));
            }

            // If offset is Infinity, means intersection is parallel
            if(Double.IsInfinity(offset))
            {
                // If diagonal other edge is any neighbour, intersection is on one of the sides
                if(midIndex == 1 || midIndex == vertexCount - 1)
                    intersections.Add(Edge.ByStartVertexEndVertex(this.Vertices.First(), this.Vertices[midIndex]));

                // If not, the intersection can pass through both diagonal extremes, so its external.
                // Through just one, so only one intersection, or non and endge is inside polygon.
                else if(this.Vertices.First().OnEdge(edge))
                    intersections.Add(this.Vertices.First());

                else if(this.Vertices[midIndex].OnEdge(edge))
                    intersections.Add(this.Vertices[midIndex]);
            }

            // If intersection is a Vertex
            else if(!Double.IsNaN(offset))
            { 
                if (offset.AlmostEqualTo(0))
                    intersections.Add(this.Vertices.First());
                else if (offset.AlmostEqualTo(1))
                    intersections.Add(this.Vertices[midIndex]);

                // Else the intersection is between the diagonal's extremes
                // find intersection at each side of the mid vertex

                // Going from midVertex to 0
                for (int i = midIndex; i > 0; i--)
                {
                    var side = RayByVertexIndex(i, i - 1, isFirstVertexInEdge);
                    if (side.TryIntersectionOffset(edge, out double t) && t.InRange(0, 1))
                    {
                        Vertex intersection = side.Origin.Translate(side.Direction.Scale(t));
                        if (!intersections.Contains(intersection) && intersection.OnEdge(edge))
                        {
                            intersections.Add(intersection);
                            break;
                        }
                    }
                }

                for (int j = midIndex; j < vertexCount; j++)
                {
                    int next = (j + 1) % vertexCount;
                    var side = RayByVertexIndex(j, next, isFirstVertexInEdge);
                    if (side.TryIntersectionOffset(edge, out double t) && t.InRange(0, 1))
                    {
                        Vertex intersection = side.Origin.Translate(side.Direction.Scale(t));
                        if (!intersections.Contains(intersection) && intersection.OnEdge(edge))
                        {
                            intersections.Add(intersection);
                            break;
                        }
                    }
                }

            }

            return intersections;
        }

        /// <summary>
        /// Determines if an edge belongs to the polygon.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool Belongs(Edge edge)
        {
            var startIndex = this.Vertices.IndexOf(edge.StartVertex);

            if (startIndex == -1)
                return false;

            var next = (startIndex + 1) % this.Vertices.Count;
            var prev = startIndex == 0 ? this.Vertices.Count - 1 : startIndex - 1;

            if (this.Vertices[next].Equals(edge.EndVertex) || this.Vertices[prev].Equals(edge.EndVertex))
                return true;

            return false;
        }

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
