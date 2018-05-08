using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphical.DataStructures
{
    public enum BinaryHeapType
    {
        MinHeap,
        MaxHeap
    }

    public abstract class BinaryHeap<TObject, TValue> where TValue : IComparable
    {
        #region Private Properties
        private const int DEFAULT_SIZE = 32;
        private const int GROWTH_FACTOR = 2;

        internal HeapItem[] _heapItems = null;
        private int _capacity;
        private int _size;
        private BinaryHeapType _heapType { get; set; }
        #endregion

        #region Public Properties

        /// <summary>
        /// HeapItem array capacity
        /// </summary>
        public int Capacity
        {
            get { return _capacity; }
            set { SetCapacity(value); }
        }

        /// <summary>
        /// HeapItem array size
        /// </summary>
        public int Size { get { return _size; } }

        public BinaryHeapType HeapType { get { return _heapType; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Binary Heap default constructor
        /// </summary>
        public BinaryHeap(BinaryHeapType heapType)
        {
            Capacity = DEFAULT_SIZE;
            _heapType = heapType;
            _heapItems = new HeapItem[DEFAULT_SIZE];
        }

        /// <summary>
        /// Binary heap constructor with initial capacity
        /// </summary>
        /// <param name="capacity">Heap initial capacity</param>
        public BinaryHeap(BinaryHeapType heapType, int capacity)
        {
            if( capacity < 1) { throw new ArgumentException("Capacity must be greater than zero"); }
            Capacity = capacity;
            _heapType = heapType;
            _heapItems = new HeapItem[capacity];
        }

        #endregion

        #region Private Methods

        internal virtual void SetCapacity (int newCapacity)
        {
            newCapacity = Math.Max(newCapacity, _size);
            if(_capacity != newCapacity)
            {
                _capacity = newCapacity;
                Array.Resize(ref _heapItems, _capacity);
            }
        }

        private void EnsureCapacity()
        {
            if(_size == _capacity)
            {
                SetCapacity(_capacity * GROWTH_FACTOR);
            }
        }

        private void CheckHeap()
        {
            if (_size == 0) { throw new InvalidOperationException("Heap is empty."); }
        }

        #endregion

        #region Internal Methods

        internal int ParentIndex (int childIndex) { return (childIndex - 1) / 2; }
        internal int LeftChildIndex (int parentIndex) { return 2 * parentIndex + 1; }
        internal int RightChildIndex (int parentIndex) { return 2 * parentIndex + 2; }

        internal bool HasLeftChild (int parentIndex) { return LeftChildIndex(parentIndex) < _size; }
        internal bool HasRightChild (int parentIndex) { return RightChildIndex(parentIndex) < _size; }
        internal bool HasParent (int childIndex) { return ParentIndex(childIndex) >= 0; }

        internal HeapItem LeftChild (int parentIndex) { return _heapItems[LeftChildIndex(parentIndex)]; }
        internal HeapItem RightChild (int parentIndex) { return _heapItems[RightChildIndex(parentIndex)]; }
        internal HeapItem Parent (int childIndex) { return _heapItems[ParentIndex(childIndex)]; }

        /// <summary>
        /// Swap items
        /// </summary>
        /// <param name="firstIndex">Index of first item</param>
        /// <param name="secondIndex">Index of second item</param>
        internal virtual void Swap(int firstIndex, int secondIndex)
        {
            var tempItem = _heapItems[firstIndex];
            _heapItems[firstIndex] = _heapItems[secondIndex];
            _heapItems[secondIndex] = tempItem;
        }

        internal void HeapifyDown(int index = 0)
        {
            while (HasLeftChild(index))
            {
                int smallestChildIndex = LeftChildIndex(index);
                if (HasRightChild(index))
                {
                    switch (HeapType)
                    {
                        case BinaryHeapType.MinHeap:
                            if(RightChild(index) < LeftChild(index)) { smallestChildIndex = RightChildIndex(index); }
                            break;
                        case BinaryHeapType.MaxHeap:
                            if (RightChild(index) > LeftChild(index)) { smallestChildIndex = RightChildIndex(index); }
                            break;
                        default:
                            break;
                    }
                    
                }

                if (HeapType == BinaryHeapType.MinHeap && _heapItems[index] < _heapItems[smallestChildIndex])
                {
                    break;
                }
                else if(HeapType == BinaryHeapType.MaxHeap && _heapItems[index] > _heapItems[smallestChildIndex])
                {
                    break;
                }
                else
                {
                    Swap(index, smallestChildIndex);
                }
                index = smallestChildIndex;
            }
        }

        internal void HeapifyUp(int index = -1)
        {
            index = (index < 0) ? this.Size - 1 : index;
            if(HeapType == BinaryHeapType.MinHeap)
            {
                while (HasParent(index) && _heapItems[index] < Parent(index))
                {
                    Swap(index, ParentIndex(index));
                    index = ParentIndex(index);
                }
            }else
            {
                while (HasParent(index) && _heapItems[index] > Parent(index))
                {
                    Swap(index, ParentIndex(index));
                    index = ParentIndex(index);
                }
            }
        }

        #endregion
        
        #region Public Methods

        /// <summary>
        /// Returns the object associated with the first item on the Heap
        /// </summary>
        /// <returns>Object of first item.</returns>
        public virtual TObject Peek()
        {
            CheckHeap();
            return (TObject)_heapItems[0].Item;
        }

        /// <summary>
        /// Returns object associated with first item and removes it from the Heap
        /// </summary>
        /// <returns>Object of first item</returns>
        public virtual TObject Take()
        {
            CheckHeap();

            HeapItem first = _heapItems[0];
            _heapItems[0] = _heapItems[_size - 1];
            _size--;
            HeapifyDown();
            return (TObject)first.Item;
        }

        /// <summary>
        /// Removes all items on the Heap
        /// </summary>
        public virtual void Clear()
        {
            _size = 0;
            Array.Clear(_heapItems, 0, _heapItems.Length);
        }

        /// <summary>
        /// Adds a new item on the Heap
        /// </summary>
        /// <param name="item">Object to add</param>
        /// <param name="value">Value associated with the object</param>
        public virtual void Add(TObject item, TValue value)
        {
            Add(new HeapItem(item, value));
        }

        /// <summary>
        /// Adds a new item on the Heap
        /// </summary>
        /// <param name="item">Heap item</param>
        public virtual void Add(HeapItem item)
        {
            EnsureCapacity();
            _heapItems[_size] = item;
            _size++;
            HeapifyUp();
        }

        #endregion

    }

    public class MinBinaryHeap<TObject, TValue> : BinaryHeap<TObject, TValue> where TValue : IComparable
    {
        #region Constructors
        public MinBinaryHeap() : base(BinaryHeapType.MinHeap) { }
        public MinBinaryHeap(int capacity) : base(BinaryHeapType.MinHeap, capacity) { }
        #endregion
    }

    public class MaxBinaryHeap<TObject, TValue> : BinaryHeap<TObject, TValue> where TValue : IComparable
    {
        #region Constructors
        public MaxBinaryHeap() : base(BinaryHeapType.MaxHeap) { }
        public MaxBinaryHeap(int capacity) : base(BinaryHeapType.MaxHeap, capacity) { }
        #endregion
    }

    public class HeapItem : IComparable<HeapItem>
    {
        public object Item { get; private set; }
        public IComparable Value { get; private set; }

        public HeapItem(object data, IComparable value)
        {
            Item = data;
            Value = value;
        }

        public void SetValue(IComparable newValue)
        {
            Value = newValue;
        }

        #region Override IComparable Methods
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(HeapItem)) { return false; }
            HeapItem item = (HeapItem)obj;
            return this.Item.Equals(item.Item) && this.Value.Equals(item.Value);
        }

        public override int GetHashCode()
        {
            return Item.GetHashCode() ^ Value.GetHashCode();
        }

        public int CompareTo(HeapItem obj)
        {
            return this.Value.CompareTo(obj.Value);
        }

        public static bool operator <(HeapItem item1, HeapItem item2)
        {
            return item1.CompareTo(item2) < 0;
        }

        public static bool operator >(HeapItem item1, HeapItem item2)
        {
            return item1.CompareTo(item2) > 0;
        }
        #endregion

        public override string ToString()
        {
            return string.Format("[Item: {0}, Value: {1}", Item.ToString(), Value.ToString());
        }
    }
}
