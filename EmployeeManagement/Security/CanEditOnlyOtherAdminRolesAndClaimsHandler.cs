using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagement.Security
{
    // Below is our custom Handler that inherits from 
    // AuthorizationHandler<which den implements d requirement class>
    //Note: CustomHandlers contains the logic for the custom Authorization 
    public class CanEditOnlyOtherAdminRolesAndClaimsHandler : AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>

    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
            ManageAdminRolesAndClaimsRequirement requirement)
        {

            var authFilterContext = context.Resource as AuthorizationFilterContext;
            if (authFilterContext==null)
            {
                return Task.CompletedTask;
            }
            //getting the ID of the loggedIn USer 
            string loggedInAdminId =
                    context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            //ID of the Admin being edited 
            string adminIdBeingEdited = authFilterContext.HttpContext.Request.Query["userId"];

            //Condition to check if the User is logged in as an Admin, has claim types and value,
            //and the adminIdBeingEdited is not equal to loggedInAdmin
            if (context.User.IsInRole("Admin") &&
             context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") &&
             adminIdBeingEdited.ToLower() != loggedInAdminId.ToLower())
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;

        }
    }
}
