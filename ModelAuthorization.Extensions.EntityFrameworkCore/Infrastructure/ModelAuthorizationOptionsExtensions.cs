using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ModelAuthorization.Extensions.EntityFrameworkCore.Infrastructure
{
    internal class ModelAuthorizationOptionsExtensions : IDbContextOptionsExtension
    {
        private DbContextOptionsExtensionInfo? _info;

        private readonly IServiceProvider _services;

        public ModelAuthorizationOptionsExtensions(IServiceProvider services)
        {
            _services = services;
        }

        public void ApplyServices(IServiceCollection services)
        {
            services.AddScoped(_ => _services.GetRequiredService<ICrudAuthorizationPolicyProvider>());
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
