using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TextEditorMVC.CustomClasses
{
    public class WordWordType
    {
        public Guid Id_Word { get; set; }

        public Guid Id_WordType { get; set; }
    }
}
