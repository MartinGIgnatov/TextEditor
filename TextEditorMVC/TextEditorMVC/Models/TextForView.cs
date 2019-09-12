using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TextEditorMVC.CustomClasses;

namespace TextEditorMVC.Models
{
    public class TextForView
    {
        public Guid Id { get; set; }

        public List<int> ErrorPositions { get; set; }

        public List<WordBoundry> WordPositions { get; set; }

        public string Text { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Title should be at least 1 character long.")]
        [MaxLength(50, ErrorMessage = "Title should be shorter than 50 characters.")]
        public string Title { get; set; }

        public List<List<string>> FixPropositions { get; set; }

        
    }
}
