using System;
using System.Collections.Generic;

namespace Graphical.Core
{
    /// <summary>
    /// Represents a Fibonacci heap data structure capable of storing generic key-value pairs.
    /// </summary>
    public class FibonacciHeap<TKey, TValue> : IPriorityQueue<TKey, TValue>
        where TKey : IComparable 
    {

        private Node _minNode;

        /// <summary>
        /// The size of the heap.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Creates a Fibonacci heap.
        /// </summary>
        public FibonacciHeap() 
        {
            _minNode = null;
            Size = 0;
        }

        /// <summary>
        /// Clears the heap's data, making it an empty heap.
        /// </summary>
        public void Clear() 
        {
            _minNode = null;
            Size = 0;
        }


        /// <summary>
        /// Inserts a new key-value pair into the heap.
        /// </summary>
        /// <param name="key">The key to insert.</param>
        /// <param name="val">The value to insert.</param>
        /// <returns>The inserted node.</returns>
        public INode<TKey, TValue> Insert(TKey key, TValue val) 
        {
            Node node = new Node(key, val);
            _minNode = MergeLists(_minNode, node);
            Size++;
            return node;
        }

        /// <summary>
        /// Returns the minimum node from the heap.
        /// </summary>
        /// <returns>The heap's minimum node or undefined if the heap is empty.</returns>
        public INode<TKey, TValue> FindMinimum() 
        {
            return _minNode;
        }

        /// <summary>
        /// Decreases a key of a node.
        /// </summary>
        /// <param name="node">The node to decrease the key of.</param>
        /// <param name="newKey">The new key to assign to the node.</param>
        public void DecreaseKey(INode<TKey, TValue> node, TKey newKey) 
        {
            var casted = node as Node;
            if (casted == null)
                throw new ArgumentException("node must be a FibonacciHeap.Node");
            DecreaseKey(casted, newKey);
        }

        /// <summary>
        /// Decreases a key of a node.
        /// </summary>
        /// </param name="node">The node to decrease the key of.</param>
        /// </param name="newKey">The new key to assign to the node.</param>
        public void DecreaseKey(Node node, TKey newKey) 
        {
            if (node == null)
                throw new ArgumentException("node must be non-null.");
            if (newKey.CompareTo(node.Key) > 0)
                throw new ArgumentOutOfRangeException("New key is larger than old key.");
            
            node.Key = newKey;
            Node parent = node.Parent;
            if (parent != null && node.CompareTo(parent) < 0) 
            {
                Cut(node, parent);
                CascadingCut(parent);
            }
            if (node.CompareTo(_minNode) < 0) 
            {
                _minNode = node;
            }
        }

        /// <summary>
        /// Cut the link between a node and its parent, moving the node to the root list.
        /// </summary>
        /// <param name="node">The node being cut.</param>
        /// <param name="parent">The parent of the node being cut.</param>
        private void Cut(Node node, Node parent) 
        {
            parent.Degree--;
            parent.Child = (node.Next == node ? null : node.Next);
            RemoveNodeFromList(node);
            MergeLists(_minNode, node);
            node.IsMarked = false;
        }

        /// <summary>
        /// Perform a cascading cut on a node; mark the node if it is not marked, otherwise cut the
        /// node and perform a cascading cut on its parent.
        /// </summary>
        /// <param name="node">The node being considered to be cut.</param>
        private void CascadingCut(Node node) 
        {
            Node parent = node.Parent;
            if (parent != null) 
            {
                if (node.IsMarked) 
                {
                    Cut(node, parent);
                    CascadingCut(parent);
                } 
                else 
                {
                    node.IsMarked = true;
                }
            }
        }

        /// <summary>
        /// Deletes a node.
        /// </summary>
        /// <param name="node">The node to delete.</param>
        public void Delete(INode<TKey, TValue> node) 
        {
            var casted = node as Node;
            if (casted == null)
                throw new ArgumentException("node must be a FibonacciHeap.Node");
            Delete(casted);
        }

        /// <summary>
        /// Deletes a node.
        /// </summary>
        /// <param name="node">The node to delete.</param>
        public void Delete(Node node) 
        {
            // This is a special implementation of decreaseKey that sets the
            // argument to the minimum value. This is necessary to make generic keys
            // work, since there is no MIN_VALUE constant for generic types.
            Node parent = node.Parent;
            if (parent != null) 
            {
                Cut(node, parent);
                CascadingCut(parent);
            }
            _minNode = node;

            ExtractMinimum();
        }

        /// <summary>
        /// Extracts and returns the minimum node from the heap.
        /// </summary>
        /// <returns>The heap's minimum node or undefined if the heap is empty.</returns>
        public INode<TKey, TValue> ExtractMinimum() 
        {
            Node extractedMin = _minNode;
            if (extractedMin != null) 
            {
                // Set parent to null for the minimum's children
                if (extractedMin.Child != null) 
                {
                    Node child = extractedMin.Child;
                    do 
                    {
                        child.Parent = null;
                        child = child.Next;
                    } while (child != extractedMin.Child);
                }

                Node nextInRootList = extractedMin.Next == extractedMin ? null : extractedMin.Next;

                // Remove min from root list
                RemoveNodeFromList(extractedMin);
                Size--;

                // Merge the children of the minimum node with the root list
                _minNode = MergeLists(nextInRootList, extractedMin.Child);

                if (nextInRootList != null) 
                {
                    _minNode = nextInRootList;
                    Consolidate();
                }
            }
            return extractedMin;
        }


        /// <summary>
        /// Merge all trees of the same order together until there are no two trees of the same
        /// order.
        /// </summary>
        private void Consolidate() 
        {
            if (_minNode == null)
                return;
            
            IList<Node> aux = new List<Node>();
            var items = GetRootTrees();

            foreach (var current in items) 
            {
                //Node current = it.next();
                var top = current;

                while (aux.Count <= top.Degree + 1) 
                {
                    aux.Add(null);
                }

                // If there exists another node with the same degree, merge them
                while (aux[top.Degree] != null)
                {
                    if (top.Key.CompareTo(aux[top.Degree].Key) > 0) 
                    {
                        Node temp = top;
                        top = aux[top.Degree];
                        aux[top.Degree] = temp;
                    }
                    LinkHeaps(aux[top.Degree], top);
                    aux[top.Degree] = null;
                    top.Degree++;
                }

                while (aux.Count <= top.Degree + 1) 
                {
                    aux.Add(null);
                }
                aux[top.Degree] = top;
            }

            _minNode = null;
            for (int i = 0; i < aux.Count; i++) 
            {
                if (aux[i] != null) 
                {
                    // Remove siblings before merging
                    aux[i].Next = aux[i];
                    aux[i].Prev = aux[i];
                    _minNode = MergeLists(_minNode, aux[i]);
                }
            }
        }

        /// <summary>
        /// Gets all root-level trees of the heap. 
        /// </summary>
        private IEnumerable<Node> GetRootTrees()
        {
            var items = new Queue<Node>();
            Node current = _minNode;
            do 
            {
                items.Enqueue(current);
                current = current.Next;
            } while (_minNode != current);
            return items;
        }

        /// <summary>
        /// Removes a node from a node list.
        /// </summary>
        /// <param name="node">The node to remove.</param>
        private void RemoveNodeFromList(Node node) 
        {
            Node prev = node.Prev;
            Node next = node.Next;
            prev.Next = next;
            next.Prev = prev;

            node.Next = node;
            node.Prev = node;
        }

        /// <summary>
        /// Links two heaps of the same order together.
        /// </summary>
        /// <param name="max">The heap with the larger root.</param>
        /// <param name="min">The heap with the smaller root.</param>
        private void LinkHeaps(Node max, Node min) 
        {
            RemoveNodeFromList(max);
            min.Child = MergeLists(max, min.Child);
            max.Parent = min;
            max.IsMarked = false;
        }

        /// <summary>
        /// Joins another heap to this heap.
        /// </summary>
        /// <param name="other">The other heap.</param>
        public void Union(IPriorityQueue<TKey, TValue> other) 
        {
            var casted = other as FibonacciHeap<TKey, TValue>;
            if (casted == null)
                throw new ArgumentException("other must be a FibonacciHeap");
            Union(casted);
        }

        /// <summary>
        /// Joins another heap to this heap.
        /// </summary>
        /// <param name="other">The other heap.</param>
        public void Union(FibonacciHeap<TKey, TValue> other) 
        {
            _minNode = MergeLists(_minNode, other._minNode);
            Size += other.Size;
        }


        /// <summary>
        /// Merge two lists of nodes together.
        /// </summary>
        /// <param name="a">The first list to merge.</param>
        /// <param name="b">The second list to merge.</param>
        /// <returns>The new minimum node from the two lists.</returns>
        private Node MergeLists(Node a, Node b)
        {
            if (a == null && b == null)
                return null;
            if (a == null)
                return b;
            if (b == null)
                return a;

            Node temp = a.Next;
            a.Next = b.Next;
            a.Next.Prev = a;
            b.Next = temp;
            b.Next.Prev = b;

            return a.CompareTo(b) < 0 ? a : b;
        }

        /// <summary>
        /// Gets whether the heap is empty.
        /// </summary>
        public bool IsEmpty {
            get { return _minNode == null; }
        }

        /// <summary>
        /// A node object used to store data in the Fibonacci heap.
        /// </summary>
        public class Node : INode<TKey, TValue> 
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }
            public int Degree { get; set; }
            public Node Parent { get; set; }
            public Node Child { get; set; }
            public Node Prev { get; set; }
            public Node Next { get; set; }
            public bool IsMarked { get; set; } 

            /// <summary>
            /// Creates a Fibonacci heap node.
            /// </summary>
            public Node()
            { 
            }

            /// <summary>
            /// Creates a Fibonacci heap node initialised with a key and value.
            /// </summary>
            /// <param name="key">The key to use.</param>
            /// <param name="val">The value to use.</param>
            public Node(TKey key, TValue val) 
            {
                Key = key;
                Value = val;
                Next = this;
                Prev = this;
            }

            public int CompareTo(object other) 
            {
                var casted = other as Node;
                if (casted == null)
                    throw new NotImplementedException("Cannot compare to a non-Node object");
                return this.Key.CompareTo(casted.Key);
            }
        }
    }
}