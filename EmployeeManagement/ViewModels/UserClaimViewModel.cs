using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class UserClaimViewModel
    {
        public UserClaimViewModel()
        {
            Claims = new List<userClaim>();
           
        }
        public string UserId { get; set; }

        public List<userClaim> Claims { get; set; }

    }
}
