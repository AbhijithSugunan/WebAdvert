using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvertV1.Models.Accounts;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAdvertV1.Controllers
{
    public class AccountsController : Controller
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;

        public AccountsController(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, CognitoUserPool pool)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _pool = pool;
        }
        public async Task<IActionResult> SignUp()
        {
            var modal = new SignUpModel();
            return View(modal);
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _pool.GetUser(model.Email);
                    if (user.Status != null)
                    {
                        ModelState.AddModelError("User Error", "Email is already registered");
                        return View(model);
                    }
                    user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Email);
                    var createdUser = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);
                    if (createdUser.Succeeded)
                    {
                       return RedirectToAction(nameof(Confirm));
                    }
                }
            }
            catch (Exception e)
            {
                return View();
            }
           

            return View();
        }

        public async Task<IActionResult> Confirm()
        {
            ConfirmModel model = new ConfirmModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(ConfirmModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
                    if (user == null)
                    {
                        ModelState.AddModelError("User Not Found", "A user with the given email address was not found");
                        return View(model);
                    }
                    var result = await (_userManager as CognitoUserManager<CognitoUser>).ConfirmSignUpAsync(user, model.Code, true).ConfigureAwait(false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError(item.Code, item.Description);
                        }
                        return View(model);
                    }
                }
                catch (Exception e)
                {

                }
                
            }

            return View(model);
        }
        
        [HttpGet]
        public async Task<IActionResult> SignIn(SignInModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("SignIn")]
        public async Task<IActionResult> SignInPost(SignInModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, 
                    model.Password, model.RememberMe, false).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("SignInError", "Username and password doesnot match");
                }
            }
            return View("SignIn", model);
        }
    }
}
