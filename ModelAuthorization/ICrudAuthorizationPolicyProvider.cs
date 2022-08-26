using ModelAuthorization.Enums;
using ModelAuthorization.Masking;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace ModelAuthorization
{
    public interface ICrudAuthorizationPolicyProvider
    {
        IMaskingHandler MaskingHandler { get; }

        ValueTask<bool> AuthorizeTypeAsync(Type type, CrudPermission permissions);
        ValueTask<bool> AuthorizePropertyAsync(Type type, PropertyInfo property, CrudPermission permissions);
    }
}
