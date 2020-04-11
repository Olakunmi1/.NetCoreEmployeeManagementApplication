using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }
        public String Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string City { get; set; }

        //In order not to have a null reference exception at runtime,
        // we have to "Initialise Claims and Roles" with a new list in the constructor above.
        public List<string> Claims { get; set; }

        public IList<string> Roles { get; set; }


    }
}
