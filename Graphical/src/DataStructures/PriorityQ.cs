using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphical.DataStructures
{
    /// <summary>
    /// Binary Priority Queue
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class PriorityQ<TObject, TValue> : BinaryHeap<TObject, TValue> where TObject : IEquatable<TObject> where TValue : IComparable
    {
        #region Private Properties
        internal Dictionary<TObject, int> heapIndices { get; private set; }
        #endregion

        #region Public Properties
        public Dictionary<TObject, int> HeapIndices { get { return heapIndices; } } 
        #endregion

        #region Constructors

        public PriorityQ(BinaryHeapType heapType) : base(heapType)
        {
            heapIndices = new Dictionary<TObject, int>(this.Capacity);
        }
        public PriorityQ(BinaryHeapType heapType, int capacity) : base(heapType, capacity)
        {
            heapIndices = new Dictionary<TObject, int>(this.Capacity);
        }

        #endregion

        #region Private Methods
        internal override void Swap(int firstIndex, int secondIndex)
        {
            var firstItem = _heapItems[firstIndex];
            var secondItem = _heapItems[secondIndex];
            _heapItems[firstIndex] = secondItem;
            _heapItems[secondIndex] = firstItem;
            heapIndices[(TObject)firstItem.Item] = secondIndex;
            heapIndices[(TObject)secondItem.Item] = firstIndex;
        }
        #endregion

        public override void Add(TObject item, TValue value)
        {
            heapIndices[item] = this.Size;
            base.Add(item, value);
        }

        public TValue GetValue(TObject item)
        {
            if (!heapIndices.ContainsKey(item)) { throw new ArgumentException("Element not existing in Priority Queue"); }
            int heapIndex = heapIndices[item];
            return (TValue)_heapItems[heapIndex].Value;
        }

        public void UpdateValue(TObject item, TValue newValue)
        {
            if (!heapIndices.ContainsKey(item)) { throw new ArgumentException("Element not existing in Priority Queue"); }
            int heapIndex = heapIndices[item];
            HeapItem heapItem = _heapItems[heapIndex];
            IComparable currentValue = heapItem.Value;
            heapItem.SetValue(newValue);

            int comparison = newValue.CompareTo(currentValue);

            if ( (HeapType == BinaryHeapType.MinHeap && comparison < 0) ||
                (HeapType == BinaryHeapType.MaxHeap && comparison > 0))
            {
                HeapifyUp(heapIndex);
            }
            else
            {
                HeapifyDown(heapIndex);
            }
        }

        public TValue PeekValue()
        {
            return (TValue)_heapItems[0].Value;
        }
        
        public override TObject Take()
        {
            TObject first = (TObject)_heapItems[0].Item;
            heapIndices.Remove(first);
            return base.Take();
        }

        public override void Clear()
        {
            heapIndices.Clear();
            base.Clear();
        }
    }

    public class MinPriorityQ<TObject, TValue> : PriorityQ<TObject, TValue> where TObject : IEquatable<TObject> where TValue : IComparable
    {
        #region Constructor

        public MinPriorityQ() : base(BinaryHeapType.MinHeap) { }
        public MinPriorityQ(int capacity) : base(BinaryHeapType.MinHeap, capacity) { }

        #endregion
    }

    public class MaxPriorityQ<TObject, TValue> : PriorityQ<TObject, TValue> where TObject : IEquatable<TObject> where TValue : IComparable
    {
        #region Constructor

        public MaxPriorityQ() : base(BinaryHeapType.MaxHeap) { }
        public MaxPriorityQ(int capacity) : base(BinaryHeapType.MaxHeap, capacity) { }

        #endregion
    }
}
