using ModelAuthorization;
using ModelAuthorization.Extensions.EntityFrameworkCore;
using ModelAuthorization.Extensions.EntityFrameworkCore.Options;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    public static class DbSetExtensions
    {
        public static DbSet<TEntity> ToAuthorizedDbSet<TEntity>(this DbSet<TEntity> dbSet, ICrudAuthorizationPolicyProvider authorizationProvider) where TEntity : class
            => ToAuthorizedDbSet(dbSet, new AuthorizedDbSetOptions(), authorizationProvider);

        public static DbSet<TEntity> ToAuthorizedDbSet<TEntity>(this DbSet<TEntity> dbSet, IAuthorizedDbSetOptions options, ICrudAuthorizationPolicyProvider authorizationProvider) where TEntity : class
            => new AuthorizedDbSet<TEntity>(dbSet, options, authorizationProvider);

    }
}