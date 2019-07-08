using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAdvertV1.Models.Accounts
{
    public class ConfirmModel
    {
        [Required(ErrorMessage = "Email address required")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter the code")]
        public string Code { get; set; }
    }
}
