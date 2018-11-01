using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mvcCookieAuthSample.Services;
using mvcCookieAuthSample.ViewModels;

namespace mvcCookieAuthSample.Controllers
{
    //[ApiController]
    public class ConsentController : Controller
    {

        private readonly ConsentService _consentService;
        public ConsentController(ConsentService consentService)
        {
            _consentService = consentService;
        }

       
        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var model = await _consentService.BuildConsentViewModel(returnUrl);

            if (model == null)
            {

            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Index(InputConsentViewModel viewModel)
        {

            var result =await _consentService.ProcessConsent(viewModel);

            if (result.IsRedirect)
            {
                return Redirect(result.RedirectUrl);
            }


            if (string.IsNullOrEmpty(result.ValidationError))
            {
                ModelState.AddModelError("", result.ValidationError);
            }

            return View(result.ViewModel);
        }
    }
}