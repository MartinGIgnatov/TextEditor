using System;
using System.Collections.Generic;
using System.Text;

namespace ReadingFilesAndCreatingWordObjects
{
    public struct BoudryIndexes
    {
        public int Start { get; }
        public int End { get; }

        public BoudryIndexes(int start, int end)
        {
            Start = start;
            End = end;
        }
    }
}
