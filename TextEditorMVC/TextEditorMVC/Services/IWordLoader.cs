using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextEditorMVC.CustomClasses;

namespace TextEditorMVC.Services
{
    public interface IWordLoader
    { 
        Task LoadingTask { get; }

        int LongestWordLength { get; }

        SymbolBranch WordTree { get; }

        Dictionary<string, SymbolBranch> WordTypeTrees { get; }
    }
}
