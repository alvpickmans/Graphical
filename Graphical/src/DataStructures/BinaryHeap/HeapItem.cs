using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphical.DataStructures
{   
    /// <summary>
    /// Class to use in BinaryHeaps when the object is not of type IComparable
    /// </summary>
    public class HeapItem : IEquatable<HeapItem>, IComparable<HeapItem>
    {
        #region Properties
        /// <summary>
        /// Object to store
        /// </summary>
        public object Item { get; private set; }

        /// <summary>
        /// Item's associated value
        /// </summary>
        public IComparable Value { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// HeapItem default constructor
        /// </summary>
        /// <param name="data">Object to store</param>
        /// <param name="value">Associated value</param>
        public HeapItem(object data, IComparable value)
        {
            Item = data;
            Value = value;
        } 
        #endregion

        /// <summary>
        /// Method to set/update HeapItem's value
        /// </summary>
        /// <param name="newValue"></param>
        public void SetValue(IComparable newValue)
        {
            Value = newValue;
        }

        #region Override IComparable Methods
        /// <summary>
        /// HeapItem's equality comparer
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(HeapItem obj)
        {
            return this.Item.Equals(obj.Item);
        }

        /// <summary>
        /// HeapItem's HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Item.GetHashCode();
        }

        /// <summary>
        /// Implementation of IComparable interface
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(HeapItem obj)
        {
            return this.Value.CompareTo(obj.Value);
        }

        /// <summary>
        /// IComparable less than operator
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <returns></returns>
        public static bool operator <(HeapItem item1, HeapItem item2)
        {
            return item1.CompareTo(item2) < 0;
        }

        /// <summary>
        /// IComparable greater than operator
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <returns></returns>
        public static bool operator >(HeapItem item1, HeapItem item2)
        {
            return item1.CompareTo(item2) > 0;
        }
        #endregion

        /// <summary>
        /// HeapItem's string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[Item: {0}, Value: {1}", Item.ToString(), Value.ToString());
        }
    }
}
