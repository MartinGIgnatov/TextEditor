using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextEditorMVC.CustomClasses
{
    public class WordProposition
    {
        public string ProposedWord { get; set; }

        public int Length { get; set; }

        public List<string> Descriptions { get; set; }

    }
}
