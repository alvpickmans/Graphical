using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphical.DataStructures
{   
    /// <summary>
    /// MaxBinaryHeap class
    /// </summary>
    /// <typeparam name="TObject">Type of object implementing IComparable<TObject> interface</TObject></typeparam>
    public class MaxBinaryHeap<TObject> : BinaryHeap<TObject> where TObject : IComparable<TObject>
    {
        #region Constructors
        /// <summary>
        /// MaxBinaryHeap default constructor
        /// </summary>
        public MaxBinaryHeap() : base(BinaryHeapType.MaxHeap) { }

        /// <summary>
        /// MaxBinaryHeap constructor with initial capacity
        /// </summary>
        /// <param name="capacity"></param>
        public MaxBinaryHeap(int capacity) : base(BinaryHeapType.MaxHeap, capacity) { }
        #endregion
    }

    /// <summary>
    /// MaxBinaryHeap class
    /// </summary>
    /// <typeparam name="TObject">Type of object to store</typeparam>
    /// <typeparam name="TValue">Type of value associated to the object, implementing IComparable interface</typeparam>
    public class MaxBinaryHeap<TObject, TValue> : BinaryHeap<TObject, TValue> where TValue : IComparable
    {
        #region Constructors
        /// <summary>
        /// MaxBinaryHeap default constructor
        /// </summary>
        public MaxBinaryHeap() : base(BinaryHeapType.MaxHeap) { }

        /// <summary>
        /// MaxBinaryHeap constructor with initial capacity
        /// </summary>
        /// <param name="capacity"></param>
        public MaxBinaryHeap(int capacity) : base(BinaryHeapType.MaxHeap, capacity) { }
        #endregion
    }
}
