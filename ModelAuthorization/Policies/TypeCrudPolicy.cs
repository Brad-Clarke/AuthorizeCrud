using ModelAuthorization.Attributes;
using ModelAuthorization.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ModelAuthorization.Policies
{
    [DebuggerDisplay("Policy: {Policy}")]
    public class TypeCrudPolicy
    {
        public string? Policy { get; }

        public Type Type { get; }

        public CrudPermission Permissions { get; }

        public IReadOnlyList<PropertyCrudPolicy> Properties { get; }

        public TypeCrudPolicy(string? policy, Type type, CrudPermission permissions, IEnumerable<PropertyCrudPolicy> properties)
        {
            Policy = policy;
            Type = type;
            Permissions = permissions;
            Properties = new List<PropertyCrudPolicy>(properties);
        }

        public static IEnumerable<TypeCrudPolicy> Build<T>()
        {
            Type type = typeof(T);

            return Build(type);
        }

        public static IEnumerable<TypeCrudPolicy> Build(Type type)
        {
            Dictionary<string, List<PropertyCrudPolicy>> propertyPolicies = new Dictionary<string, List<PropertyCrudPolicy>>();

            foreach (PropertyInfo property in type.GetPublicProperties())
            {
                foreach (KeyValuePair<string, PropertyCrudPolicy> propertyCrudPolicy in PropertyCrudPolicy.Build(property))
                {
                    if (propertyPolicies.TryGetValue(propertyCrudPolicy.Key, out List<PropertyCrudPolicy>? crudPolicies))
                    {
                        crudPolicies.Add(propertyCrudPolicy.Value);
                    }
                    else
                    {
                        propertyPolicies.Add(propertyCrudPolicy.Key, new List<PropertyCrudPolicy> { propertyCrudPolicy.Value });
                    }
                }
            }

            foreach (var typePolicy in type.GetCustomAttributes<AllowCrudAttribute>())
            {
                List<PropertyCrudPolicy> policies = new List<PropertyCrudPolicy>();

                if (propertyPolicies.TryGetValue(string.Empty, out List<PropertyCrudPolicy>? globalPolicies))
                {
                    policies.AddRange(globalPolicies);
                }

                if (!string.IsNullOrWhiteSpace(typePolicy.Policy) && propertyPolicies.TryGetValue(typePolicy.Policy, out List<PropertyCrudPolicy>? crudPolicies))
                {
                    policies.AddRange(crudPolicies);
                }

                yield return new TypeCrudPolicy(typePolicy.Policy, type, typePolicy.Permissions, policies);
            }
        }

        public bool IsAuthorized(CrudPermission permissions)
        {
            return Permissions.HasFlag(permissions);
        }

        public bool IsAuthorized(PropertyInfo property, CrudPermission permissions)
        {
            PropertyCrudPolicy[] propertyPolicy = Properties.Where(p => p.Property == property).ToArray();

            if (propertyPolicy.Length == 0)
            {
                return Permissions.HasFlag(permissions);
            }

            if (propertyPolicy.Any(p => p.Permission.HasFlag(permissions)))
            {
                return true;
            }

            if (propertyPolicy.Any(p => p.Restrictions.HasFlag(permissions)))
            {
                return false;
            }

            return Permissions.HasFlag(permissions);
        }
    }
}
