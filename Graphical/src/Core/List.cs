using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphical.Geometry;
using EdgeKey = Graphical.Graphs.EdgeKey;

namespace Graphical.Core
{
    /// <summary>
    /// Static class extending List funtionality
    /// </summary>
    public static class List
    {
        /// <summary>
        /// Given a ascending sorted list, add items mantaining list's order.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        internal static List<dynamic> AddItemSorted(List<dynamic> list, dynamic[] item)
        {
            List<dynamic> clone = list.ToList();
            foreach(var it in item)
            {
                int lo = 0;
                int hi = clone.Count();
                while (lo < hi)
                {
                    int mid = (int)(lo + hi) / 2;
                    if (it < clone[mid])
                    {
                        hi = mid;
                    }
                    else
                    {
                        lo = mid + 1;
                    }
                }
                clone.Insert(lo, it);
            }
            return clone;
        }
        internal static List<EdgeKey> AddItemSorted(List<EdgeKey> list, EdgeKey item)
        {
            
            int lo = 0;
            int hi = list.Count();
            while (lo < hi)
            {
                int mid = (int)(lo + hi) / 2;
                if (item < list[mid])
                {
                    hi = mid;
                }
                else
                {
                    lo = mid + 1;
                }
            }
            list.Insert(lo, item);
            
            return list;
        }

        internal static int Bisect(List<EdgeKey> list, EdgeKey item)
        {
            int lo = 0, hi = list.Count;
            while(lo < hi)
            {
                int mid = (lo + hi) / 2;
                if(item < list[mid])
                {
                    hi = mid;
                }else
                {
                    lo = mid + 1;
                }
            }
            return lo;
        }

        public static List<List<T>> Chop<T>(List<T> list, int length)
        {
            return list
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / length)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}
