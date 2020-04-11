using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Controllers
{
    [Authorize(Roles ="Admin,SuperAdmin")]
    //Admisnitrator controller helps to create user, manage Roles etc..
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<AdministrationController> logger;

        // A constructor that injects the RoleManager Service
        // A constructor that injects the UserManger Service and Ilogger Service..
        public AdministrationController(RoleManager<IdentityRole> roleManager, 
                                        UserManager<ApplicationUser> userManager, ILogger<AdministrationController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
        }

        //This helps to ManageUserClaims, serves the UsersClaims page 
        [HttpGet]
        //Appylying EditRolePolicy to this action
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            // UserManager service, GetClaimsAsync method gets all the current claims of the user
            var claims = await userManager.GetClaimsAsync(user);
            var model = new UserClaimViewModel
            {
                UserId = userId
            };

            // Loop through each claim we have in our application
            foreach (var claim in ClaimsStore.AllClaims) 
            {
                userClaim userClaim = new userClaim
                {
                    ClaimType = claim.Type
                };

                // If the user has the claim, set IsSelected property to true, so the checkbox
                // next to the claim is checked on the UI
                if (claims.Any(c => c.Type == claim.Type && c.Value == "true"))
                {
                    userClaim.IsSelected = true;
                }

                model.Claims.Add(userClaim);
            }

            return View(model);


        }

        // Handles the post request for ManagingClaims for a User
        [HttpPost]
        //Appylying EditRolePolicy to this action
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserClaims(UserClaimViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found";
                return View("NotFound");
            }

            // Get all the user existing claims and delete them
            var claims = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            // Add all the claims that are selected on the UI
            result = await userManager.AddClaimsAsync(user,
                model.Claims.Select(c => new Claim(c.ClaimType, c.IsSelected ? "true": "false")));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = model.UserId });

        }


        //This helps to ManageRoles, serves the roles page
        [HttpGet]
        //Appylying EditRole Policy to this action
        [Authorize(Policy = "EditRolePolicy")]
        public async Task <IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;
           var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRolesViewModel>();
            foreach(var role in roleManager.Roles)
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
               if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }
                model.Add(userRolesViewModel);
            }

            return View(model);
        }

        //responds to post method for the roles action 
        [HttpPost]
        //Appylying EditRole Policy to this action
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult>ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var roles = await userManager.GetRolesAsync(user);
            //removes all roles regardlesss so it can respond incase if all d roles are unchecked by d user
            var result = await userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }

            result = await userManager.AddToRolesAsync(user,
                model.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = userId });
        

        }


        //List the users registered with the website
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }

        //this serves the Edit User Page 
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found"; 
                return View("NotFound");
            }

            // GetClaimsAsync retunrs the list of user Claims
            var userClaims = await userManager.GetClaimsAsync(user);
            // GetRolesAsync returns the list of user Roles
            var userRoles = await userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                City = user.City,
                Claims = userClaims.Select(c => c.Type + " : " + c.Value).ToList(),
                Roles = userRoles
            };

            return View(model);
        }

        //this responds to a post method that makes changes to Users 
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.City = model.City;
                

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        //This deletes Users from the DB using the DeleteAsync from UserManager 
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id {Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("ListUsers");
            }
           
        }

        //this action method serves the CreateRole View
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        //this action method creates the Role
        [HttpPost] 
        public async Task<IActionResult> CreateRole(CreateRoleViewModel createRole) 
        {
            if (ModelState.IsValid)
            {
                var identityRole = new IdentityRole
                {
                    Name = createRole.Rolename
                };
               var roleResult =  await roleManager.CreateAsync(identityRole);
                if (roleResult.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(createRole);
        } 

        //This returns the list of Roles present
        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles); 
        }

        //This deletes Role present
        [HttpPost]
        //Appylying DeleteRole Policy to EditRole action
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteRole(string Id)
        {

            var role = await roleManager.FindByIdAsync(Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"User with id {Id} cannot be found";
                return View("NotFound");
            }
            else
            {
               // Wrap the code in a try/catch block
               //wrapping the code below in a try, catch error
               //block helps to catch d possible exception that might likely occur
               // whcih is "DbUpdateException"
                try
                {
                    var result = await roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("ListRoles");
                }
                catch (DbUpdateException ex)
                {
                    //Log the Exception to a file 
                    logger.LogError($"Exception Occurred{ex}");

                    //Pass the ErrorTitle and ErrorMessage you want 
                    //to show to the user using d ViewBag..The ErrorView retreives 
                    //this data from the ViewBag and displays to d User

                    ViewBag.ErrorTitle = $"{role.Name} Role is in use";
                    ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as" +
                        $" there are users in this role. If you want to" +
                        $" delete this role, please remove the users from the role and then try to delete";
                    return View("Error");


                }
            }
           
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
           
           var role = await roleManager.FindByIdAsync(id);
            if(role == null)
            {
                ViewBag.ErrorMesage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            //checking if Users is in Role...
           foreach (var user in userManager.Users) 
           {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {

                    model.Users.Add(user.UserName);
                } 

            }

            return View(model);
           
        }


        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {

            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMesage = $"Role with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
               var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles","Administration");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);

        }
        [HttpGet]
        //this helps to assign roles to users 
        public async Task<IActionResult> EditUserInRole(string roleId)
        {
            ViewBag.roleId = roleId; 
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMesage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }
            var model = new List<UserRoleViewModel>();

            //getting the list of all registered users 

            foreach(var user in userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    Username = user.UserName
                };

                if( await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;

                }
                else 
                {
                    userRoleViewModel.IsSelected = false;
                }
                model.Add(userRoleViewModel);
            };
            return View(model);
        }

        //Responds to post request which adds a user to a Role
        //Adding a User to a role ...
        [HttpPost]
        public async Task<IActionResult> EditUserInRole(List <UserRoleViewModel> model, string roleId)
         {
            //getting the ROles from the Db by Id
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMesage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }
            for(int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if(!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });


        }
    }
}