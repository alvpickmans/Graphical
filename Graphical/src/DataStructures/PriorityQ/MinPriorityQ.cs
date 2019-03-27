using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphical.DataStructures
{
    /// <summary>
    /// MinBinary Priority Queue
    /// </summary>
    /// <typeparam name="TObject">Type of object to store implementing IEquatable and IComparable interfaces</typeparam>
    public class MinPriorityQ<TObject> : PriorityQ<TObject> where TObject : IEquatable<TObject>, IComparable<TObject>
    {
        #region Constructor
        /// <summary>
        /// MinPriorityQ default constructor
        /// </summary>
        public MinPriorityQ() : base(BinaryHeapType.MinHeap) { }

        /// <summary>
        /// MinPriorityQ constructor with initial capacity.
        /// </summary>
        /// <param name="capacity">Initial capacity</param>
        public MinPriorityQ(int capacity) : base(BinaryHeapType.MinHeap, capacity) { }

        #endregion
    }

    /// <summary>
    /// MinBinary Priority Queue
    /// </summary>
    /// <typeparam name="TObject">Type of object to store implementing IEquatable interface</typeparam>
    /// <typeparam name="TValue">Type of value associated with TObject implementing IComparable</typeparam>
    public class MinPriorityQ<TObject, TValue> : PriorityQ<TObject, TValue> where TObject : IEquatable<TObject> where TValue : IComparable
    {
        #region Constructor
        /// <summary>
        /// MinPriorityQ default constructor
        /// </summary>
        public MinPriorityQ() : base(BinaryHeapType.MinHeap) { }

        /// <summary>
        /// MinPriorityQ constructor with initial capacity.
        /// </summary>
        /// <param name="capacity">Initial capacity</param>
        public MinPriorityQ(int capacity) : base(BinaryHeapType.MinHeap, capacity) { }

        #endregion
    }
}
