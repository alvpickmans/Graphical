using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphical.DataStructures
{
    /// <summary>
    /// Custom Enum to determine behaviour of BinaryHeap
    /// </summary>
    public enum BinaryHeapType
    {
        /// <summary>
        /// BinaryHeap's items sorted in increasing order
        /// </summary>
        MinHeap,
        /// <summary>
        /// BinaryHeap's items sorted in decreasing order
        /// </summary>
        MaxHeap
    }
}
