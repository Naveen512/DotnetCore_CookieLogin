using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CookieAuth.Web.DAL;
using CookieAuth.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace CookieAuth.Web.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly UserContext _userContext;
        public AccountController(UserContext userContext)
        {
            _userContext = userContext;
        }


        [Route("sign-up")]
        [HttpGet]
        public IActionResult SignUp()
        {
            return View(new UserViewModel());
        }

        [Route("sign-up")]
        [HttpPost]
        public IActionResult SignUp(UserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (!_userContext.User.Any(_ => _.Email.ToLower() == viewModel.Email.ToLower()))
                {
                    User newUser = new User
                    {
                        Email = viewModel.Email,
                        FirstName = viewModel.FirstName,
                        LastName = viewModel.LastName,
                        Password = viewModel.Password
                    };
                    _userContext.User.Add(newUser);
                    _userContext.SaveChanges();

                    return Redirect("account/login");
                }
            }
            return View(viewModel);
        }

        [Route("login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // note : real time we save password with encryption into the database
                // so to check that viewModel.Password also need to encrypt with same algorithm 
                // and then that encrypted password value need compare with database password value
                Models.User user = _userContext.User.Where(_ => _.Email.ToLower() == viewModel.Email.ToLower() && _.Password == viewModel.Password).FirstOrDefault();
                if (user != null)
                {
                    user.LastLoginTime = DateTime.Now;
                    _userContext.SaveChanges();



                    var claims = new List<Claim>
                     {
                         new Claim(ClaimTypes.Name, user.Email),
                         new Claim("FirstName",user.FirstName),

                     };
                    var userRoles = _userContext.UserRole.Join(
                                  _userContext.Roles,
                                  ur => ur.RoleId,
                                  r => r.Id,
                                  (ur, r) => new
                                  {
                                      ur.RoleId,
                                      r.RoleName,
                                      ur.UserId
                                  }).Where(_ => _.UserId == user.Id).ToList();
                    foreach (var ur in userRoles)
                    {
                        var roleClaim = new Claim(ClaimTypes.Role, ur.RoleName);
                        claims.Add(roleClaim);
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties() { IsPersistent = viewModel.IsPersistant };
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    return Redirect("/");
                }
                else
                {
                    ModelState.AddModelError("InvalidCredentials", "Either username or password is not correct");
                }
            }
            return View(viewModel);
        }

        [HttpGet]
        [Route("sign-out")]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }
    }
}