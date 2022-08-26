using ModelAuthorization.Policies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelAuthorization
{
    public abstract class CrudAuthorizationPolicyProviderBase
    {
        private readonly Dictionary<Type, List<TypeCrudPolicy>> _cachePolicies = new();

        protected IEnumerable<TypeCrudPolicy> GetCrudPolicies<T>()
        {
            return GetCrudPolicies(typeof(T));
        }

        protected IEnumerable<TypeCrudPolicy> GetCrudPolicies(Type type)
        {
            if (!_cachePolicies.TryGetValue(type, out List<TypeCrudPolicy> cachedPolicies))
            {
                cachedPolicies = TypeCrudPolicy.Build(type).ToList();

                if (cachedPolicies.Count == 0)
                {
                    return new List<TypeCrudPolicy>();
                }

                _cachePolicies.Add(type, cachedPolicies);
            }

            return cachedPolicies;
        }
    }
}
