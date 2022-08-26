using ModelAuthorization.Enums;
using System;

namespace ModelAuthorization.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RestrictCrudAttribute : CrudPolicyAttributeBase
    {
        public RestrictCrudAttribute(CrudPermission permissions) : base(permissions)
        {
        }

        public RestrictCrudAttribute(string policy, CrudPermission permissions) : base(policy, permissions)
        {
        }
    }
}
