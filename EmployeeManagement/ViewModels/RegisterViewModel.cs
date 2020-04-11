using EmployeeManagement.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        //Remote Attribute below helps to send an ajax request to d 
        //client machine to validate d email if its InUse, which also reduces a round trip to d server 
        //thereby improving performance...
        [Remote(action:"IsEmailInUse", controller:"Account")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name ="Confirm Password")]
        [Compare("Password", ErrorMessage ="Password and confirmation password does not match.")]
        public string ConfirmPassword { get; set; }
        public string City { get; set; }
    }
}
