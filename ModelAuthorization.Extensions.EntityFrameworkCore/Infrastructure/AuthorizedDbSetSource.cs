using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ModelAuthorization.Extensions.EntityFrameworkCore.Options;
using System.Collections.Concurrent;
using System.Reflection;

namespace ModelAuthorization.Extensions.EntityFrameworkCore.Infrastructure
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Required for AuthorizedDbSet to be used for DbSet Initialization")]
    internal class AuthorizedDbSetSource : IDbSetSource
    {
        private static readonly MethodInfo GenericCreateSet
            = typeof(AuthorizedDbSetSource).GetTypeInfo().GetDeclaredMethod(nameof(CreateSetFactory))!;

        private readonly ConcurrentDictionary<(Type Type, string? Name), Func<DbContext, string?, object>> _cache = new();

        private readonly IAuthorizedDbSetOptions _options;

        private readonly ICrudAuthorizationPolicyProvider _policyProvider;

        public AuthorizedDbSetSource(IAuthorizedDbSetOptions options, ICrudAuthorizationPolicyProvider policyProvider)
        {
            _options = options;
            _policyProvider = policyProvider;
        }

        public virtual object Create(DbContext context, Type type)
            => CreateCore(context, type, null, GenericCreateSet);

        public virtual object Create(DbContext context, string name, Type type)
            => CreateCore(context, type, name, GenericCreateSet);

        private object CreateCore(DbContext context, Type type, string? name, MethodInfo createMethod)
            => _cache.GetOrAdd(
                (type, name),
                (t, createMethod) => (Func<DbContext, string?, object>)createMethod
                    .MakeGenericMethod(t.Type)
                    .Invoke(null, new object[] { _options, _policyProvider })!,
                createMethod)(context, name);

        private static Func<DbContext, string?, object> CreateSetFactory<TEntity>(IAuthorizedDbSetOptions options, ICrudAuthorizationPolicyProvider policyProvider)
            where TEntity : class
            => (c, name) => new InternalDbSet<TEntity>(c, name).ToAuthorizedDbSet(options, policyProvider);
    }
}
