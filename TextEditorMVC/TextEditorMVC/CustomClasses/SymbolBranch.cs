using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextEditorMVC.CustomClasses
{
    public class SymbolBranch
    {
        public Dictionary<char,SymbolBranch> SymbolDictionery { get; set; }

        public bool IsWord { get; set; }

        public List<string> Descriptions { get; set; }

        public SymbolBranch(bool isWord)
        {
            IsWord = isWord;
            Descriptions = new List<string>();
            SymbolDictionery = new Dictionary<char, SymbolBranch>();
        }

    }
}
