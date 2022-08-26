using Microsoft.AspNetCore.Authorization;

namespace ModelAuthorization.Extensions.AspNetCore.Authorization.Infrastructure
{
    internal class CrudAuthorizeData : IAuthorizeData
    {
        public string? Policy { get; set; }
        public string? Roles { get; set; }
        public string? AuthenticationSchemes { get; set; }
    }
}
