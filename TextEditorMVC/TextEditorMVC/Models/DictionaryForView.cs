using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextEditorMVC.CustomClasses;

namespace TextEditorMVC.Models
{
    public class DictionaryForView
    {
        public List<WordProposition> wordPropositions { get; set; }

        public string SearchWord { get; set; }
    }
}
