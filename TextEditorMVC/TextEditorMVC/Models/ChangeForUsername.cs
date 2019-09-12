using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TextEditorMVC.Models
{
    public class ChangeForUsername
    {
        [Required]
        [MinLength(6, ErrorMessage = "Username should be at least 6 characters long.")]
        [MaxLength(25, ErrorMessage = "Username should be shorter than 25 characters.")]
        public string NewUsername { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
