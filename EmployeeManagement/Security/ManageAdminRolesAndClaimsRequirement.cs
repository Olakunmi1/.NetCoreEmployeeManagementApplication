using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Security
{
    //Our custom Requirement which must inherit from IAuthorizationRequirement
    //and 1 or more handlers will also inherit from it
    public class ManageAdminRolesAndClaimsRequirement : IAuthorizationRequirement
    {
         
    }
}
