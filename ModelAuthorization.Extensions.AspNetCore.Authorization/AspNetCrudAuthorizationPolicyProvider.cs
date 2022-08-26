using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using ModelAuthorization.Enums;
using ModelAuthorization.Extensions.AspNetCore.Authorization.Infrastructure;
using ModelAuthorization.Masking;
using ModelAuthorization.Policies;

namespace ModelAuthorization.Extensions.AspNetCore.Authorization
{
    internal class AspNetCrudAuthorizationPolicyProvider : CrudAuthorizationPolicyProviderBase, ICrudAuthorizationPolicyProvider
    {
        private readonly Dictionary<Type, TypeCrudPolicy[]?> _cachedPolicies = new Dictionary<Type, TypeCrudPolicy[]?>();
        
        private readonly IAuthorizationPolicyProvider _policyProvider;
        private readonly IPolicyEvaluator _policyEvaluator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public AspNetCrudAuthorizationPolicyProvider(IHttpContextAccessor httpContextAccessor, IAuthorizationPolicyProvider policyProvider, IPolicyEvaluator policyEvaluator)
        {
            _httpContextAccessor = httpContextAccessor;
            _policyProvider = policyProvider;
            _policyEvaluator = policyEvaluator;
        }

        public IMaskingHandler MaskingHandler => new DefaultMaskingHandler(this);

        public async ValueTask<bool> AuthorizeTypeAsync(Type type, CrudPermission permissions)
        {
            TypeCrudPolicy[]? policies = await GetOrAddAsync(type);

            return policies == null ||
                   (policies.Length != 0 &&
                    policies.All(p => p.IsAuthorized(permissions)));
        }

        public async ValueTask<bool> AuthorizePropertyAsync(Type type, PropertyInfo property, CrudPermission permissions)
        {
            TypeCrudPolicy[]? policies = await GetOrAddAsync(type);

            return policies == null ||
                   (policies.Length != 0 &&
                    policies.All(p => p.IsAuthorized(property, permissions)));
        }

        private async Task<TypeCrudPolicy[]?> GetOrAddAsync(Type type)
        {
            if (_cachedPolicies.TryGetValue(type, out TypeCrudPolicy[]? cachedPolicies))
            {
                return cachedPolicies;
            }

            TypeCrudPolicy[]? policies = await GetAuthorizedPoliciesAsync(type);

            _cachedPolicies.Add(type, policies);

            return policies;
        }

        private async Task<TypeCrudPolicy[]?> GetAuthorizedPoliciesAsync(Type type)
        {
            List<TypeCrudPolicy> authorizedPolicies = new List<TypeCrudPolicy>();

            var policies = GetCrudPolicies(type).ToArray();

            if (policies.Length == 0)
            {
                return null;
            }

            foreach (var policy in policies)
            {
                if (string.IsNullOrWhiteSpace(policy.Policy) || await TryAuthorizeAsync(policy.Policy))
                {
                    authorizedPolicies.Add(policy);
                }
            }

            return authorizedPolicies.ToArray();
        }

        private async Task<bool> TryAuthorizeAsync(string policy)
        {
            AuthorizationPolicy? authorizationPolicy = await AuthorizationPolicy.CombineAsync(_policyProvider, new[] { new CrudAuthorizeData { Policy = policy } });

            AuthenticateResult authenticateResult = await _policyEvaluator.AuthenticateAsync(authorizationPolicy, _httpContextAccessor.HttpContext);

            PolicyAuthorizationResult authorizeResult = await _policyEvaluator.AuthorizeAsync(authorizationPolicy, authenticateResult, _httpContextAccessor.HttpContext, null);

            return authorizeResult.Succeeded;
        }
    }
}