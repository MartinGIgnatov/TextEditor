using System;
using System.Collections.Generic;
using System.Text;

namespace ReadingFilesAndCreatingWordObjects
{
    public abstract class Reader
    {
        public string Path { get; }

        public Reader(string path)
        {
            Path = path;
        }

        public abstract List<WordStruct> ReadAll(); 
    }
}
