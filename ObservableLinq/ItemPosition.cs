using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public struct ItemPosition
    {
        public readonly int Index;
        public readonly int Offset;

        public ItemPosition(int index, int offset)
        {
            Index = index;
            Offset = offset;
        }

        public int GetNewIndex()
        {
            return Index + Math.Min(1, Offset);
        }
    }
}
