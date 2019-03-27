using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphical.DataStructures
{   
    /// <summary>
    /// MinBinaryHeap class
    /// </summary>
    /// <typeparam name="TObject">Type of object implementing IComparable<TObject> interface</TObject></typeparam>
    public class MinBinaryHeap<TObject> : BinaryHeap<TObject> where TObject : IComparable<TObject>
    {
        #region Constructors
        /// <summary>
        /// MinBinaryHeap default constructor
        /// </summary>
        public MinBinaryHeap() : base(BinaryHeapType.MinHeap) { }

        /// <summary>
        /// MinBinaryHeap constructor with initial capacity
        /// </summary>
        /// <param name="capacity"></param>
        public MinBinaryHeap(int capacity) : base(BinaryHeapType.MinHeap, capacity) { }
        #endregion
    }

    /// <summary>
    /// MinBinaryHeap class
    /// </summary>
    /// <typeparam name="TObject">Type of object to store</typeparam>
    /// <typeparam name="TValue">Type of value associated to the object, implementing IComparable interface</typeparam>
    public class MinBinaryHeap<TObject, TValue> : BinaryHeap<TObject, TValue> where TValue : IComparable
    {
        #region Constructors
        /// <summary>
        /// MinBinaryHeap default constructor
        /// </summary>
        public MinBinaryHeap() : base(BinaryHeapType.MinHeap) { }

        /// <summary>
        /// MinBinaryHeap constructor with initial capacity
        /// </summary>
        /// <param name="capacity"></param>
        public MinBinaryHeap(int capacity) : base(BinaryHeapType.MinHeap, capacity) { }
        #endregion
    }
}
