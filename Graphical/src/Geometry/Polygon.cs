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

        internal Edge DiagonalByVertexIndex(int start, int end)
        {
            var vertexCount = this.Vertices.Count;
            if(start < 0 || start > vertexCount - 1)
            {
                throw new ArgumentOutOfRangeException("start", start, "Out of range");
            }
            if (end < 0 || end > vertexCount - 1)
            {
                throw new ArgumentOutOfRangeException("end", end, "Out of range");
            }

            return Edge.ByStartVertexEndVertex(this.Vertices[start], this.Vertices[end]);
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
                        {
                            xFirstSign = 1;
                        }
                        else if(xSign < 0)
                        {
                            xFlips += 1;
                        }

                        xSign = 1;
                    }
                    else // nextX < 0
                    {
                        if(xSign == 0)
                        {
                            xFirstSign = -1;
                        }
                        else if(xSign > 0)
                        {
                            xFlips += 1;
                        }
                        xSign = -1;
                   }
                }

                if(xFlips > 2) { return false; }

                if (!nextY.AlmostEqualTo(0))
                {
                    if(nextY > 0)
                    {
                        if(ySign == 0) { yFirstSign = 1; }
                        else if(ySign < 0) { yFlips += 1; }

                        ySign = 1;
                    }
                    else // nextY < 0
                    {
                        if (ySign == 0) { yFirstSign = -1; }
                        else if (ySign > 0) { yFlips += 1; }

                        ySign = -1;
                    }
                }

                if(yFlips > 2) { return false; }

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
                    {
                        return false;
                    }
                }
            }

            // Final/wraparound sign flips
            if(xSign != 0 && xFirstSign != 0 && xSign != xFirstSign)
            {
                xFlips += 1;
            }

            if(ySign != 0 && yFirstSign != 0 && ySign != yFirstSign)
            {
                yFlips += 1;
            }

            // Concave polygons have two sign flips along each axis
            if( xFlips != 2 || yFlips != 2) { return false; }

            // This is a convex polygon
            return true;
        }

        private List<Geometry> IntersectionNaive(Edge edge)
        {
            List<Geometry> intersections = new List<Geometry>();

            if (this.BoundingBox.Intersects(edge.BoundingBox))
            {
                for (int i = 0; i < this.Edges.Count; i++)
                {
                    var side = this.Edges[i];
                    var intersection = edge.Intersection(side);

                    if(intersection is Edge interEdge)
                    {
                        return new List<Geometry>() { interEdge };
                    }
                    if (intersection is Vertex && !intersections.Contains(intersection))
                    {
                        intersections.Add(intersection);
                    }
                }
            }

            return intersections;
        }

        public List<Geometry> Intersection(Edge edge)
        {
            // No fast algorithm yet to calculate intersection on concave polygons
            if (!this.IsConvex())
            {
                return this.IntersectionNaive(edge);
            }

            //https://stackoverflow.com/questions/4497841/asymptotically-optimal-algorithm-to-compute-if-a-line-intersects-a-convex-polygo

            List<Geometry> intersections = new List<Geometry>();
            if (!this.BoundingBox.Intersects(edge.BoundingBox)) { return intersections; }

            var vertexCount = this.Vertices.Count;
            int midIndex = (int)(vertexCount / 2);
            Edge diagonal = this.DiagonalByVertexIndex(0, midIndex);
            int polygonDirection = this.Vertices[1].IsLeftFrom(diagonal);
            int startFirstSide = edge.StartVertex.IsLeftFrom(diagonal);
            int endFirstSide = edge.EndVertex.IsLeftFrom(diagonal);

            // Depending on which vertex is considered as start, 
            // the method can enter on a infinite loop if the edge
            // does not intersect but start and end point lie on 
            // different sides from the first diagonal.
            Vertex edgeVertex = edge.StartVertex;

            Geometry intersection = diagonal.Intersection(edge);

            while(intersection == null)
            {
                // If midIndex is any neighbour from the start vertex
                // means the whole line is to one side or the other and doesn't intersect.
                if(midIndex == 1 || midIndex == vertexCount - 1) { return intersections; }

                int vertexSide = edgeVertex.IsLeftFrom(diagonal);

                // If same side, don't intersect
                if(vertexSide == polygonDirection && vertexSide == startFirstSide && vertexSide == endFirstSide)
                {
                    return intersections;
                }
                // Is on other side from the polygonDirection (Vertices[1])
                else if(vertexSide != polygonDirection)
                {
                    midIndex += (int)((vertexCount - midIndex) / 2);
                }
                else
                {
                    midIndex = (int)(midIndex / 2);
                }

                // If the vertes is the start and the side has changed from the initial side,
                // swap the edgeVertex to be the end and start from the mid diagonal
                if(edgeVertex.Equals(edge.StartVertex) && startFirstSide != vertexSide)
                {
                    edgeVertex = edge.EndVertex;
                    midIndex = (int)(vertexCount / 2);
                    intersection = null;
                }
                // If same but with end vertex, does not intersect.
                else if(edgeVertex.Equals(edge.EndVertex) && endFirstSide != vertexSide)
                {
                    return intersections;
                }
                else
                {
                    intersection = edge.Intersection(diagonal);
                }

                diagonal = this.DiagonalByVertexIndex(0, midIndex);
            }

            // If intersection is an Edge
            if(intersection is Edge edgeIntersection)
            {
                // If diagonal other edge is any neighbour, intersection is on one of the sides
                if(midIndex == 1 || midIndex == vertexCount - 1)
                {
                    intersections.Add(edgeIntersection);
                    return intersections;
                }

                // If not, the intersection can pass through both diagonal extremes, so its external.
                // Through just one, so only one intersection, or non and endge is inside polygon.

                if (this.Vertices.First().OnEdge(edgeIntersection)) { intersections.Add(this.Vertices.First()); }
                if(this.Vertices[midIndex].OnEdge(edgeIntersection)) { intersections.Add(this.Vertices[midIndex]); }

                return intersections;
            }

            // If intersection is a Vertex
            if(intersection is Vertex vertexIntersection)
            {

                if (vertexIntersection.Equals(this.Vertices.First()))
                {
                    intersections.Add(this.Vertices.First());
                }

                else if (vertexIntersection.Equals(this.Vertices[midIndex]))
                {
                    intersections.Add(this.Vertices[midIndex]);
                }

                else
                {
                    // Else the intersection is between the diagonal's extremes
                    // find intersection at each side of the mid vertex

                    // Going from midVertex to 0
                    for (int i = midIndex; i > 0; i--)
                    {
                        var side = DiagonalByVertexIndex(i, i - 1);
                        Vertex sideIntersection = edge.Intersection(side) as Vertex;
                        if (sideIntersection != null)
                        {
                            intersections.Add(sideIntersection);
                            break;
                        }
                    }

                    for (int j = midIndex; j < vertexCount; j++)
                    {
                        int next = (j + 1) % vertexCount;
                        var side = DiagonalByVertexIndex(j, next);
                        Vertex sideIntersection = edge.Intersection(side) as Vertex;
                        if (sideIntersection != null)
                        {
                            intersections.Add(sideIntersection);
                            break;
                        }
                    }
                }

                return intersections;
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
            var endIndex = this.Vertices.IndexOf(edge.EndVertex);

            if(startIndex == -1 || endIndex == -1)
            {
                return false;
            }

            var startNext = (startIndex + 1) % this.Vertices.Count;
            var startPrev = startIndex == 0 ? this.Vertices.Count - 1 : startIndex - 1;

            if(startNext != endIndex && startPrev != endIndex)
            {
                return false;
            }

            return true;
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
        //        for (var i = count; i < result.Count; i++)
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
