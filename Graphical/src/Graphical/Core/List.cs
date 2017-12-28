using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphical.Base;
using EdgeKey = Graphical.Graphs.EdgeKey;

namespace Graphical.Core
{
    public static class List
    {
        public static List<dynamic> AddItemSorted(List<dynamic> list, dynamic[] item)
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

        public static int Bisect(List<EdgeKey> list, EdgeKey item)
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
    }
}
