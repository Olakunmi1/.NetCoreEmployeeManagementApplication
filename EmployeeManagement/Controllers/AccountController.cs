using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeManagement.Controllers
{
    public class AccountController : Controller
    {
        //The UserManager<IdentityUser> helps to create and manage user 
        //while SignInManager helps to sign in with the exisiting user

        private UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<ApplicationUser> logger;

        public AccountController(UserManager<ApplicationUser> userManager, 
                                    SignInManager<ApplicationUser> signInManager, ILogger<ApplicationUser> logger)
        {
            _userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        } 

        //This action serves the Change password view 
        //Serves the password html page
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        //This action responds to the chnagePassword post method
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                // ChangePasswordAsync changes the user password
                var result = await _userManager.ChangePasswordAsync(user,
                    model.CurrentPassword, model.NewPassword);

                // The new password did not meet the complexity rules or
                // the current password is incorrect. Add these errors to
                // the ModelState and rerender ChangePassword view
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                // Upon successfully changing the password refresh sign-in cookie
                await signInManager.RefreshSignInAsync(user);
                return View("ChangePasswordConfirmation");
            }

            return View(model);
        }

        //Handles the ResetPaswword Action, serves the Reset Password page
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            // If password reset token or email is null, most likely the
            // user tried to tamper the password reset link
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }

        //Handles the post request for ResetPassword Action
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    // reset the user password
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        // Upon successful password reset and if the account is lockedout, set
                        // the account lockout end date to current UTC date time, so the user
                        // can login with the new password
                        //esle if not set , the user wont be able to log in until the time out is finished
                        if (await _userManager.IsLockedOutAsync(user))
                        {
                            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                        }
                        return View("ResetPasswordConfirmation");
                    }
                    // Display validation errors. For example, password reset token already
                    // used to change the password or password complexity rules not met
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist
                return View("ResetPasswordConfirmation");
            }
            // Display validation errors if model state is not valid
            return View(model);
        }



        //This action serves the forget password page
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //This Action responds to post request for forget password
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                //getting the user by mail
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {

                    //before proceeding, we generate a password reset token 
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    //then we build a reset link the user will click on

                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                                                        new { email = model.Email, token = token }, Request.Scheme);
                    logger.Log(LogLevel.Warning, passwordResetLink);

                    return View("ForgotPasswordConfirmation");
                }
                //In order to avoid brute attacks we return the same view, so as to 
                //not give clue to the attacker if truly the user is registerd with us or not
                return View("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        //Confirm Email Action
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if(userId == null || token == null)
            {
                return RedirectToAction("index", "home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                ViewBag.ErrorMessage = $"The User ID {userId} is invalid";
                return View("NotFound");
            }

            //if the user is found, then we confirm the email
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return View();
            }

            ViewBag.ErrorTitle = "Email Cannot be confirmed";
            return View("Error");
            

            //return View();
        }

        //Serves the Register Page
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        //Check If Email is In use 
        //This Action method helps us to check from the client side 
        //if the email is In use or not, instead of having a roudntrip to d server
        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use");
            }
        }
        //Register a new user
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City
                };
                var result = await _userManager.CreateAsync(user, model.Password); 
                if (result.Succeeded) 
                {
                    //before proceeding to signIn, we generte an Email confrimation token
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    //then we build a confirmation link the user will click on

                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                                                        new { userId = user.Id, token = token }, Request.Scheme);
                    logger.Log(LogLevel.Warning, confirmationLink);

                    // If the user is signed in and in the Admin role, then it is
                    // the Admin user that is creating a new user. So redirect the
                    // Admin user to ListUsers action

                    if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors) 
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            return View(model);
        }

        // Serves the Log in page
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        //Enable user to log in
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string ReturnUrl)
            {
            
            if (ModelState.IsValid)
            {
                //getting the user by mail
                var user = await _userManager.FindByEmailAsync(model.Email);
                if(user !=null && !user.EmailConfirmed && 
                                        (await _userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(" ", "Email is not Confirmed yet, " +
                        "pls confirm your mail and log in again.");

                    //before proceeding to signIn, we generte an Email confrimation token
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    //then we build a confirmation link the user will click on

                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                                                        new { userId = user.Id, token = token }, Request.Scheme);
                    logger.Log(LogLevel.Warning, confirmationLink);

                    return View(model);
                }

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password,
                                                    model.Rememberme, true);
                if (result.Succeeded) 
                {
                    if (!string.IsNullOrEmpty(ReturnUrl)) 
                    {
                        return LocalRedirect(ReturnUrl);  
                    }
                    return RedirectToAction("Index", "Home"); 
                }
                // If account is lockedout send the use to AccountLocked view
                if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }
                ModelState.AddModelError("", "Invalid Login Attempt");
            }
            return View(model);
        }
        //Log out 
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        //Serves the "Access Denied Page" Error...
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
