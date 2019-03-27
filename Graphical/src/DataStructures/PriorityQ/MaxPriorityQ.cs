using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphical.DataStructures
{
    /// <summary>
    /// MaxBinary Priority Queue
    /// </summary>
    /// <typeparam name="TObject">Type of object to store implementing IEquatable interface</typeparam>
    /// <typeparam name="TValue">Type of value associated with TObject implementing IComparable</typeparam>
    public class MaxPriorityQ<TObject> : PriorityQ<TObject> where TObject : IEquatable<TObject>, IComparable<TObject>
    {
        #region Constructor
        /// <summary>
        /// MaxPriorityQ default constructor
        /// </summary>
        public MaxPriorityQ() : base(BinaryHeapType.MaxHeap) { }

        /// <summary>
        /// MaxPriorityQ constructor with initial capacity.
        /// </summary>
        /// <param name="capacity">Initial capacity</param>
        public MaxPriorityQ(int capacity) : base(BinaryHeapType.MaxHeap, capacity) { }

        #endregion
    }

    /// <summary>
    /// MaxBinary Priority Queue
    /// </summary>
    /// <typeparam name="TObject">Type of object to store implementing IEquatable interface</typeparam>
    /// <typeparam name="TValue">Type of value associated with TObject implementing IComparable</typeparam>
    public class MaxPriorityQ<TObject, TValue> : PriorityQ<TObject, TValue> where TObject : IEquatable<TObject> where TValue : IComparable
    {
        #region Constructor
        /// <summary>
        /// MaxPriorityQ default constructor
        /// </summary>
        public MaxPriorityQ() : base(BinaryHeapType.MaxHeap) { }

        /// <summary>
        /// MaxPriorityQ constructor with initial capacity.
        /// </summary>
        /// <param name="capacity">Initial capacity</param>
        public MaxPriorityQ(int capacity) : base(BinaryHeapType.MaxHeap, capacity) { }

        #endregion
    }
}
