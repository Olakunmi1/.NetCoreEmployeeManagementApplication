using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    //this static class helps to store claims...
    //Claim type and Claim Vlaues
    public  static class ClaimsStore
    {
        public static List<Claim> AllClaims = new List<Claim>()
        {
            new Claim("Create Role", "Create Role"),
            new Claim("Edit Role", "Create Role"),
            new Claim("Delete Role", "Create Role") 
        };
    }
}
