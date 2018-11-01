using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;     //授权
using Microsoft.AspNetCore.Authentication;    //证明
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using mvcCookieAuthSample.Models;
using mvcCookieAuthSample.ViewModels;
using IdentityServer4.Test;

namespace mvcCookieAuthSample.Controllers
{

    public class AccountController : Controller
    {

        private readonly TestUserStore _users;

        //private UserManager<ApplicationUser> _userManager;
        //private SignInManager<ApplicationUser> _signInManager;
        //public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManage)
        //{
            //_userManager = userManager;
            //_signInManager = signInManage;
        //}
            
        public AccountController(TestUserStore users)
        {
            _users = users;
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        /// <summary>
        /// 添加注册功能时（添加包好ViewModel的方法）时，也要保留这个方法，否则，无法请求注册页面（返回状态码404）
        /// </summary>
        /// <returns></returns>
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel, string returnUrl = null)
        {

            //if (ModelState.IsValid)
            //{
            //    ViewData["ReturnUrl"] = returnUrl;
            //    var identityUser = new ApplicationUser
            //    {
            //        Email = viewModel.Email,
            //        UserName = viewModel.Email,
            //        NormalizedUserName = viewModel.Email
            //    };
            //    var identityResult = await _userManager.CreateAsync(identityUser, viewModel.Password);

            //    if (identityResult.Succeeded)
            //    {
            //        //HttpContext.SignInAsync
            //        await _signInManager.SignInAsync(identityUser, new AuthenticationProperties { IsPersistent = true });
            //        return RedirectToLocal(returnUrl);
            //        //return RedirectToAction("Index", "Home");
            //    }
            //    else
            //    {
            //        AddErrors(identityResult);
            //    }
            //}


            return View();
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }


        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                var user = _users.FindByUsername(viewModel.UserName);

                if (user == null)
                {
                    ModelState.AddModelError(nameof(viewModel.UserName), "Email not Exists");//必须在View中使用 asp-validation-summary提示信息才能在界面输出

                }
                else
                {
                    if (_users.ValidateCredentials(viewModel.UserName, viewModel.Password))
                    {

                        var props = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(30))
                        };
                        //HttpContext.SignInAsync(user.SubjectId, user.Username,props);

                        await Microsoft.AspNetCore.Http.AuthenticationManagerExtensions.SignInAsync(HttpContext, user.SubjectId, user.Username, props);

                        return RedirectToLocal(returnUrl);
                    }
                    ModelState.AddModelError(nameof(viewModel.UserName), "wrong password");//必须在View中使用 asp-validation-summary提示信息才能在界面输出

                }

                return View();
            }




            //if (ModelState.IsValid)
            //{
            //    ViewData["ReturnUrl"] = returnUrl;
            //    var user = await _userManager.FindByEmailAsync(viewModel.Email);

            //    if (user == null)
            //    {
            //        ModelState.AddModelError(string.Empty, "Register please");//必须在View中使用 asp-validation-summary提示信息才能在界面输出
            //        return View();
            //    }

            //    //await _signInManager.SignInAsync(user, new AuthenticationProperties { IsPersistent = true });
            //    //return RedirectToAction("Index", "Home");

            //    var result = await _signInManager.PasswordSignInAsync(user, viewModel.Password, true, false);

            //    if (!result.Succeeded)
            //    {
            //        ModelState.AddModelError(string.Empty, "password error");
            //        return View();
            //    }

            //    return RedirectToAction("Index", "Home");
            //}

            return View();
        }




        //[Authorize]  如果启用，将会一直循环跳转
        public IActionResult MakeLogin()
        {

            //模拟用户认证登录
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,"Kyle"),
                new Claim(ClaimTypes.Role,"Admin"),
            };
            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);


            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimIdentity));
            return Ok();       //返回一个OK(),它会变成一个Api
        }


        //public IActionResult Logout()
        //{
        //    HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    return RedirectToAction("Index", "Home");
        //}


        /// <summary>
        ///这里使用的时Post提交方式（再前端的Form表单中指定），因为，退出登录时，需要拿着相应的Cookie来请求退出 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Logout()
        {
            //await _signInManager.SignOutAsync();
            //return RedirectToAction("Index", "Home");

            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }
    }



}