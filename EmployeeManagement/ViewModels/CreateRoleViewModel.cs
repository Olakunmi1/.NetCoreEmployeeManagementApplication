using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class CreateRoleViewModel
    {
        //The Regex expression below validates the string content... 
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage ="RoleName must contain letters only")]
        public string Rolename { get; set; }
    }
}
