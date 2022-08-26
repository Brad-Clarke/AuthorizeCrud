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

        public BasicCrudAuthorizationPolicyProvider()
        {
            Policies = new List<string>();
        }

        public BasicCrudAuthorizationPolicyProvider(IEnumerable<string> policies)
        {
            Policies = new List<string>(policies);
        }

        public BasicCrudAuthorizationPolicyProvider(params string[] policies)
        {
            Policies = new List<string>(policies);
        }

        public ValueTask AuthorizeAsync<T>(CrudPermission permissions)
        {
            var policies = GetCrudPolicies<T>().ToArray();

            if (policies.Length == 0)
            {
                return ValueTask.CompletedTask;
            }

            policies = policies.Where(p => Policies.Contains(p.Policy)).ToArray();

            if (policies.Length == 0)
            {
                throw new UnauthorizedAccessException();
            }

            if (policies.All(p => p.IsAuthorized(permissions)))
            {
                return ValueTask.CompletedTask;
            }

            throw new UnauthorizedAccessException($"Policy: {string.Concat(Policies)} is not Authorized to perform {permissions.ToString()} operations on {typeof(T).Name}.");
        }

        public ValueTask<bool> IsAuthorizedForClassAsync<T>(CrudPermission permissions)
        {
            return IsAuthorizedForClassAsync(typeof(T), permissions);
        }

        public ValueTask<bool> IsAuthorizedForClassAsync(Type type, CrudPermission permissions)
        {
            var policies = GetCrudPolicies(type).ToArray();

            if (policies.Length == 0)
            {
                return new ValueTask<bool>(true);
            }

            policies = policies.Where(p => Policies.Contains(p.Policy)).ToArray();

            if (policies.Length == 0)
            {
                return new ValueTask<bool>(false);
            }

            if (policies.All(p => p.IsAuthorized(permissions)))
            {
                return new ValueTask<bool>(true);
            }

            return new ValueTask<bool>(false);
        }

        public ValueTask<bool> IsAuthorizedForPropertyAsync<T>(PropertyInfo property, CrudPermission permissions)
        {
            return IsAuthorizedForPropertyAsync(typeof(T), property, permissions);
        }

        public ValueTask<bool> IsAuthorizedForPropertyAsync(Type type, PropertyInfo property, CrudPermission permissions)
        {
            var policies = GetCrudPolicies(type).ToArray();

            if (policies.Length == 0)
            {
                return new ValueTask<bool>(true);
            }

            policies = policies.Where(p => Policies.Contains(p.Policy)).ToArray();

            if (policies.Length == 0)
            {
                return new ValueTask<bool>(false);
            }

            if (policies.All(p => p.IsAuthorized(property, permissions)))
            {
                return new ValueTask<bool>(true);
            }

            return new ValueTask<bool>(false);
        }
    }
}
