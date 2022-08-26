using ModelAuthorization.Enums;
using ModelAuthorization.Masking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ModelAuthorization
{
    public class BasicCrudAuthorizationPolicyProvider : CrudAuthorizationPolicyProviderBase, ICrudAuthorizationPolicyProvider
    {
        public IReadOnlyList<string> Policies { get; }

        public IMaskingHandler MaskingHandler => new DefaultMaskingHandler(this);

        public BasicCrudAuthorizationPolicyProvider(params string[] policies)
        {
            Policies = new List<string>(policies);
        }

        public ValueTask<bool> AuthorizeTypeAsync(Type type, CrudPermission permissions)
        {
            var policies = GetCrudPolicies(type).ToArray();

            if (policies.Length == 0)
            {
                return new ValueTask<bool>(true);
            }

            policies = policies.Where(p => Policies.Contains(p.Policy)).ToArray();

            return new ValueTask<bool>(policies.Length != 0 && policies.All(p => p.IsAuthorized(permissions)));
        }

        public ValueTask<bool> AuthorizePropertyAsync(Type type, PropertyInfo property, CrudPermission permissions)
        {
            var policies = GetCrudPolicies(type).ToArray();

            if (policies.Length == 0)
            {
                return new ValueTask<bool>(true);
            }

            policies = policies.Where(p => Policies.Contains(p.Policy)).ToArray();

            return new ValueTask<bool>(policies.Length != 0 && policies.All(p => p.IsAuthorized(permissions)));
        }
    }
}
