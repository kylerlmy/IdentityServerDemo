using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using mvcCookieAuthSample.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcCookieAuthSample.Services
{
    public class ConsentService
    {
        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;
        private readonly IIdentityServerInteractionService _identityServerInteractionService;
        public ConsentService(IClientStore clientStore, IResourceStore resourceStore, IIdentityServerInteractionService identityServerInteractionService)
        {
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _identityServerInteractionService = identityServerInteractionService;
        }


        #region private method
        private ConsentViewModel CreateConsentViewModel(AuthorizationRequest request, Client client, Resources resources, InputConsentViewModel model)
        {
            var rememberConsent = model?.RememberConsent ?? true;
            var selectedScopes = model?.ScopesConsented ?? Enumerable.Empty<string>();

            var consentViewModel = new ConsentViewModel();

            consentViewModel.ClientName = client.ClientName;
            consentViewModel.ClientLogoUrl = client.LogoUri;
            consentViewModel.ClientUrl = client.ClientUri;
            //consentViewModel.RememberConsent = client.AllowRememberConsent;
            consentViewModel.RememberConsent = rememberConsent;

            consentViewModel.IdentityScopes = resources.IdentityResources.Select(i => CreateScopeViewModel(i, model == null || selectedScopes.Contains(i.Name)));
            consentViewModel.ResourceScopes = resources.ApiResources.SelectMany(i => i.Scopes).Select(x => CreateScopeViewModel(x, model == null || selectedScopes.Contains(x.Name)));

            return consentViewModel;
        }

        private ScopeViewModel CreateScopeViewModel(Scope scope, bool check)
        {
            return new ScopeViewModel
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Description = scope.Description,
                Checked = check || scope.Required,
                Required = scope.Required,
                Emphasize = scope.Emphasize
            };
        }

        private ScopeViewModel CreateScopeViewModel(IdentityResource identityResource, bool check)
        {
            return new ScopeViewModel
            {
                Name = identityResource.Name,
                DisplayName = identityResource.DisplayName,
                Description = identityResource.Description,
                Checked = check || identityResource.Required,
                Required = identityResource.Required,
                Emphasize = identityResource.Emphasize
            };
        }

        #endregion

        public async Task<ConsentViewModel> BuildConsentViewModel(string returnUrl, InputConsentViewModel model = null)
        {
            var request = await _identityServerInteractionService.GetAuthorizationContextAsync(returnUrl);
            if (returnUrl == null) return null;

            var client = await _clientStore.FindEnabledClientByIdAsync(request.ClientId);
            var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);

            var viewModel = CreateConsentViewModel(request, client, resources, model);
            viewModel.ReturnUrl = returnUrl;
            return viewModel;
        }


        public async Task<ProcessConsentResult> ProcessConsent(InputConsentViewModel model)
        {


            ConsentResponse consentResponse = null;
            var result = new ProcessConsentResult();

            if (model.Button == "no")
            {
                consentResponse = ConsentResponse.Denied;

            }
            else if (model.Button == "yes")
            {
                if (model.ScopesConsented != null && model.ScopesConsented.Any())
                {
                    consentResponse = new ConsentResponse
                    {
                        RememberConsent = model.RememberConsent,
                        ScopesConsented = model.ScopesConsented
                    };
                }

                result.ValidationError = "请至少选中一个权限";
            }

            if (consentResponse != null)
            {
                var request = await _identityServerInteractionService.GetAuthorizationContextAsync(model.ReturnUrl);
                await _identityServerInteractionService.GrantConsentAsync(request, consentResponse);


                result.RedirectUrl = model.ReturnUrl;
            }
            else
            {
                var consentViewModel = await BuildConsentViewModel(model.ReturnUrl);
                result.ViewModel = consentViewModel;
            }


            return result;
        }


    }
}
