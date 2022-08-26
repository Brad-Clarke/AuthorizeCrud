using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using ModelAuthorization.Extensions.EntityFrameworkCore.Options;

namespace ModelAuthorization.Extensions.EntityFrameworkCore.Infrastructure
{
    internal class ModelAuthorizationOptionsExtensions : IDbContextOptionsExtension
    {
        private DbContextOptionsExtensionInfo? _info;

        private readonly IAuthorizedDbSetOptions _options;

        private readonly IServiceProvider _services;

        public ModelAuthorizationOptionsExtensions(IAuthorizedDbSetOptions options, IServiceProvider services)
        {
            _options = options;
            _services = services;
        }

        public void ApplyServices(IServiceCollection services)
        {
            services.AddScoped(_ => _services.GetRequiredService<ICrudAuthorizationPolicyProvider>());
            services.AddScoped(_ => _options);
        }

        public void Validate(IDbContextOptions options)
        {
        }

        public DbContextOptionsExtensionInfo Info => _info ??= new ModelAuthorizationExtensionInfo(this);

        private sealed class ModelAuthorizationExtensionInfo : DbContextOptionsExtensionInfo
        {
            public ModelAuthorizationExtensionInfo(IDbContextOptionsExtension extension) : base(extension)
            {
            }

            public override int GetServiceProviderHashCode() => 0;

            public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => true;

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
            }

            public override bool IsDatabaseProvider => false;

            public override string LogFragment => string.Empty;
        }
    }
}
