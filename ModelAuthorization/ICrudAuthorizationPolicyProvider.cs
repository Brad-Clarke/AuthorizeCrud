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

        ValueTask AuthorizeAsync<T>(CrudPermission permissions);

        ValueTask<bool> IsAuthorizedForClassAsync<T>(CrudPermission permissions);
        ValueTask<bool> IsAuthorizedForClassAsync(Type type, CrudPermission permissions);
        ValueTask<bool> IsAuthorizedForPropertyAsync<T>(PropertyInfo property, CrudPermission permissions);
        ValueTask<bool> IsAuthorizedForPropertyAsync(Type type, PropertyInfo property, CrudPermission permissions);
    }
}
