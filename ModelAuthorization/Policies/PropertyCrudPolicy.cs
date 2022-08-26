using ModelAuthorization.Attributes;
using ModelAuthorization.Enums;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ModelAuthorization.Policies
{
    public class PropertyCrudPolicy
    {
        public PropertyInfo Property { get; }

        public CrudPermission Permission { get; }

        public CrudPermission Restrictions { get; }

        public PropertyCrudPolicy(PropertyInfo property, CrudPermission permission, CrudPermission restrictions)
        {
            Property = property;
            Permission = permission;
            Restrictions = restrictions;
        }

        public static Dictionary<string, PropertyCrudPolicy> Build(PropertyInfo property)
        {
            Dictionary<string, PropertyCrudPolicy> propertyPolicyMap = new Dictionary<string, PropertyCrudPolicy>();

            var policies = property.GetCustomAttributes<CrudPolicyAttributeBase>();

            foreach (CrudPolicyAttributeBase policy in policies)
            {
                if (propertyPolicyMap.ContainsKey(policy.Policy ?? string.Empty))
                {
                    throw new Exception("Only one Policy can be applied to a Property.");
                }

                if (policy.Permissions.HasFlag(CrudPermission.Create | CrudPermission.Delete))
                {
                    throw new Exception("A Property Crud Policy cannot be assigned the Update or Delete Permissions");
                }

                PropertyCrudPolicy crudPolicy = policy switch
                {
                    AllowCrudAttribute allow => new PropertyCrudPolicy(property, allow.Permissions, CrudPermission.None),
                    RestrictCrudAttribute restrict => new PropertyCrudPolicy(property, CrudPermission.None, restrict.Permissions),
                    _ => throw new Exception("Unknown Policy.")
                };

                propertyPolicyMap.Add(policy.Policy ?? string.Empty, crudPolicy);
            }

            return propertyPolicyMap;
        }

        public bool IsAuthorized(CrudPermission permissions)
        {
            if (Permission.HasFlag(permissions))
            {
                return true;
            }

            if (Restrictions.HasFlag(permissions))
            {
                return false;
            }

            return true;
        }
    }
}
