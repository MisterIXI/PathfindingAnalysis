using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder
{
    class DuplicateComparer<TKey> : IComparer<TKey> where TKey : IComparable
        //https://stackoverflow.com/questions/5716423/c-sharp-sortable-collection-which-allows-duplicate-keys
    {
        public int Compare(TKey x, TKey y)
        {
            int result = x.CompareTo(y);
            if (result == 0)
                return 1; //handle equal as greater
            else
                return result;


        }
    }
}
