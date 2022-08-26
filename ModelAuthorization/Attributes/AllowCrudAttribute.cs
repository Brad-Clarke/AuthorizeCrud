using ModelAuthorization.Enums;
using System;

namespace ModelAuthorization.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public class AllowCrudAttribute : CrudPolicyAttributeBase
    {
        public AllowCrudAttribute(CrudPermission permissions) : base(permissions)
        {
        }

        public AllowCrudAttribute(string policy, CrudPermission permissions) : base(policy, permissions)
        {
        }
    }
}