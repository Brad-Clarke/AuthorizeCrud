using ModelAuthorization.Enums;
using System;

namespace ModelAuthorization.Attributes
{
    public abstract class CrudPolicyAttributeBase : Attribute
    {
        public string? Policy { get; }

        public CrudPermission Permissions { get; }

        protected CrudPolicyAttributeBase(CrudPermission permissions)
        {
            Permissions = permissions;
        }

        protected CrudPolicyAttributeBase(string policy, CrudPermission permissions) : this(permissions)
        {
            if (string.IsNullOrWhiteSpace(policy))
            {
                throw new ArgumentException(nameof(policy));
            }

            Policy = policy;
        }
    }
}
