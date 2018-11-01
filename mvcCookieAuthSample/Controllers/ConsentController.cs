using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mvcCookieAuthSample.ViewModels;

namespace mvcCookieAuthSample.Controllers
{
    //[ApiController]
    public class ConsentController : Controller
    {

        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;
        private readonly IIdentityServerInteractionService _identityServerInteractionService;
        public ConsentController(IClientStore clientStore, IResourceStore resourceStore, IIdentityServerInteractionService identityServerInteractionService)
        {
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _identityServerInteractionService = identityServerInteractionService;
        }

        private async Task<ConsentViewModel> BuildConsentViewModel(string returnUrl)
        {
            var request = await _identityServerInteractionService.GetAuthorizationContextAsync(returnUrl);
            if (returnUrl == null) return null;

            var client = await _clientStore.FindEnabledClientByIdAsync(request.ClientId);
            var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);

            var viewModel = CreateConsentViewModel(request, client, resources);
            viewModel.ReturnUrl = returnUrl;
            return viewModel;
        }

        private ConsentViewModel CreateConsentViewModel(AuthorizationRequest request, Client client, Resources resources)
        {
            var consentViewModel = new ConsentViewModel();
            consentViewModel.ClientName = client.ClientName;
            consentViewModel.ClientLogoUrl = client.LogoUri;
            consentViewModel.RememberConsent = client.AllowRememberConsent;

            consentViewModel.IdentityScopes = resources.IdentityResources.Select(i => CreateScopeViewModel(i));
            consentViewModel.ResourceScopes = resources.ApiResources.SelectMany(i => i.Scopes).Select(x => CreateScopeViewModel(x));

            return consentViewModel;
        }

        private ScopeViewModel CreateScopeViewModel(Scope scope)
        {
            return new ScopeViewModel
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Description = scope.Description,
                Checked = scope.Required,
                Required = scope.Required,
                Emphasize = scope.Emphasize
            };
        }

        private ScopeViewModel CreateScopeViewModel(IdentityResource identityResource)
        {
            return new ScopeViewModel
            {
                Name = identityResource.Name,
                DisplayName = identityResource.DisplayName,
                Description = identityResource.Description,
                Checked = identityResource.Required,
                Required = identityResource.Required,
                Emphasize = identityResource.Emphasize
            };
        }

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var model = await BuildConsentViewModel(returnUrl);

            if (model == null)
            {

            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Index(InputConsentViewModel viewModel)
        {
            ConsentResponse consentResponse = null;

            if (viewModel.Button == "no")
            {
                consentResponse = ConsentResponse.Denied;

            }
            else if (viewModel.Button == "yes")
            {
                if (viewModel.ScopesConsented != null && viewModel.ScopesConsented.Any())
                {
                    consentResponse = new ConsentResponse
                    {
                        RememberConsent = viewModel.RememberConsent,
                        ScopesConsented = viewModel.ScopesConsented
                    };
                }

            }

            if (consentResponse != null)
            {
                var request = await _identityServerInteractionService.GetAuthorizationContextAsync(viewModel.ReturnUrl);
                await _identityServerInteractionService.GrantConsentAsync(request, consentResponse);

               return Redirect(viewModel.ReturnUrl);
            }

            return View();
        }
    }
}