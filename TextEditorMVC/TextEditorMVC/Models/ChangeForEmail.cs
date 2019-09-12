using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TextEditorMVC.Models
{
    public class ChangeForEmail
    {
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password should not be shorter than 8 characters.")]
        [MaxLength(25, ErrorMessage = "Password should not be longer than 25 characters.")]
        public string Password { get; set; }
    }
}
