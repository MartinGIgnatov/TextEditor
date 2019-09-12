using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextEditorMVC.CustomClasses;

namespace TextEditorMVC.Services
{
    public interface IWordRecognition
    {
        bool IsWord(string word);

        List<WordProposition> FindWordPropositions(string searchWord, int propositionsMaxCount = 1, bool hasEndingSymbol = false);

        SymbolBranch WordTree { get; }

        Dictionary<string, SymbolBranch> WordTypeTrees { get; }

    }
}
