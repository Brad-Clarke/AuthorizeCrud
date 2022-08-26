using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using ModelAuthorization.Extensions.EntityFrameworkCore.Infrastructure;
using ModelAuthorization.Extensions.EntityFrameworkCore.Options;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextOptionsBuilderExtensions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Required for AuthorizedDbSet to be used for DbSet Initialization")]
        public static void UseModelAuthorization(this DbContextOptionsBuilder optionsBuilder, bool throwOnRestrictedPropertyUpdated = false)
        {
            optionsBuilder.ReplaceService<IDbSetSource, AuthorizedDbSetSource>();

            CoreOptionsExtension coreOptions = (CoreOptionsExtension)optionsBuilder.Options.Extensions.First();

            if (coreOptions.ApplicationServiceProvider is null)
            {
                throw new ArgumentNullException(nameof(CoreOptionsExtension.ApplicationServiceProvider), "The UseApplicationServiceProvider method must be called before this method is called.");
            }

            IAuthorizedDbSetOptions options = new AuthorizedDbSetOptions
            {
                ThrowOnRestrictedPropertyUpdated = throwOnRestrictedPropertyUpdated
            };
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(new ModelAuthorizationOptionsExtensions(options, coreOptions.ApplicationServiceProvider));
        }
    }
}