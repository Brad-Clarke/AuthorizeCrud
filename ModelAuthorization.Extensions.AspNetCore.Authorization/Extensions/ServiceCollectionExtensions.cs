using ModelAuthorization;
using ModelAuthorization.Extensions.AspNetCore.Authorization;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthorizedModels(this IServiceCollection services)
        {
            services.AddScoped<ICrudAuthorizationPolicyProvider, AspNetCrudAuthorizationPolicyProvider>();

            return services;
        }
    }
}
