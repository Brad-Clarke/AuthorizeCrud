using ModelAuthorization;
using ModelAuthorization.Extensions.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    public static class DbSetExtensions
    {
        public static DbSet<TEntity> ToAuthorizedDbSet<TEntity>(this DbSet<TEntity> dbSet, ICrudAuthorizationPolicyProvider authorizationProvider) where TEntity : class
            => new AuthorizedDbSet<TEntity>(dbSet, authorizationProvider);

    }
}