using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TextEditorApi.Models
{
    public class UserForRegistration
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password should not be shorter than 8 characters.")]
        [MaxLength(25, ErrorMessage = "Password should not be longer than 25 characters.")]
        public string Password { get; set; }
        
        [Required]
        [MinLength(8, ErrorMessage = "Password should not be shorter than 8 characters.")]
        [MaxLength(25, ErrorMessage = "Password should not be longer than 25 characters.")]
        public string RepeatedPassword { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
