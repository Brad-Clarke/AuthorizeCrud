using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using ModelAuthorization.Enums;
using ModelAuthorization.Extensions.EntityFrameworkCore.Masking;
using ModelAuthorization.Extensions.EntityFrameworkCore.Options;
using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ModelAuthorization.Extensions.EntityFrameworkCore
{
    internal class AuthorizedDbSet<TEntity> : DbSet<TEntity>, IAsyncEnumerable<TEntity>, IQueryable<TEntity>, IInfrastructure<IServiceProvider>, IListSource where TEntity : class
    {
        private readonly ICrudAuthorizationPolicyProvider _authorizationProvider;

        private readonly IAuthorizedDbSetOptions _options;

        private readonly DbSet<TEntity> _internalDbSet;

        public override IEntityType EntityType => _internalDbSet.EntityType;

        public AuthorizedDbSet(DbSet<TEntity> internalDbSet, IAuthorizedDbSetOptions options, ICrudAuthorizationPolicyProvider authorizationProvider)
        {
            _internalDbSet = internalDbSet;
            _authorizationProvider = authorizationProvider;
            _options = options;
        }

        public override IAsyncEnumerable<TEntity> AsAsyncEnumerable()
            => _internalDbSet.AsAsyncEnumerable();

        public override IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default)
                => new MaskingAsyncEnumerator<TEntity>(_internalDbSet.GetAsyncEnumerator(cancellationToken), _authorizationProvider.MaskingHandler);

        public override IQueryable<TEntity> AsQueryable()
            => _internalDbSet.AsQueryable();

        public override LocalView<TEntity> Local
            => _internalDbSet.Local;

        public override TEntity? Find(params object?[]? keyValues)
            => _internalDbSet.Find(keyValues);

        public override ValueTask<TEntity?> FindAsync(params object?[]? keyValues)
            => _internalDbSet.FindAsync(keyValues);

        public override ValueTask<TEntity?> FindAsync(object?[]? keyValues, CancellationToken cancellationToken)
            => _internalDbSet.FindAsync(keyValues, cancellationToken);

        public override EntityEntry<TEntity> Add(TEntity entity)
        {
            _authorizationProvider.AuthorizeTypeAsync(typeof(TEntity), CrudPermission.Create).GetAwaiter().GetResult();

            return _internalDbSet.Add(entity);
        }

        public override async ValueTask<EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _authorizationProvider.AuthorizeTypeAsync(typeof(TEntity), CrudPermission.Create);

            return await _internalDbSet.AddAsync(entity, cancellationToken);
        }

        public override EntityEntry<TEntity> Attach(TEntity entity)
            => _internalDbSet.Attach(entity);

        public override EntityEntry<TEntity> Remove(TEntity entity)
        {
            _authorizationProvider.AuthorizeTypeAsync(typeof(TEntity), CrudPermission.Delete).GetAwaiter().GetResult();

            return _internalDbSet.Remove(entity);
        }

        public override EntityEntry<TEntity> Update(TEntity entity)
        {
            EntityEntry<TEntity> entry = _internalDbSet.Update(entity);

            foreach (var propertyEntry in entry.Properties.Where(p => p.IsModified))
            {
                if (_authorizationProvider.AuthorizePropertyAsync(typeof(TEntity), propertyEntry.Metadata.PropertyInfo!, CrudPermission.Update).GetAwaiter().GetResult())
                {
                    continue;
                }

                if (_options.ThrowOnRestrictedPropertyUpdated)
                {
                    throw new UnauthorizedAccessException($"The Current User is not Authorized to perform an Update operation on {typeof(TEntity).Name}.");
                }

                propertyEntry.IsModified = false;
            }

            return entry;
        }

        public override void AddRange(params TEntity[] entities)
            => _internalDbSet.AddRange(entities);

        public override Task AddRangeAsync(params TEntity[] entities)
            => _internalDbSet.AddRangeAsync(entities);

        public override void AttachRange(params TEntity[] entities)
            => _internalDbSet.AttachRange(entities);

        public override void RemoveRange(params TEntity[] entities)
            => _internalDbSet.RemoveRange(entities);

        public override void UpdateRange(params TEntity[] entities)
        {
            foreach (TEntity entity in entities)
            {
                Update(entity);
            }
        }

        public override void AddRange(IEnumerable<TEntity> entities)
        {
            _authorizationProvider.AuthorizeTypeAsync(typeof(TEntity), CrudPermission.Create).GetAwaiter().GetResult();

            _internalDbSet.AddRange(entities);
        }

        public override async Task AddRangeAsync(
            IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
        {
            await _authorizationProvider.AuthorizeTypeAsync(typeof(TEntity), CrudPermission.Create);

            await _internalDbSet.AddRangeAsync(entities, cancellationToken);
        }

        public override void AttachRange(IEnumerable<TEntity> entities)
            => _internalDbSet.AttachRange(entities);


        public override void RemoveRange(IEnumerable<TEntity> entities)
        {
            _authorizationProvider.AuthorizeTypeAsync(typeof(TEntity), CrudPermission.Delete).GetAwaiter().GetResult();

            _internalDbSet.RemoveRange(entities);
        }


        public override void UpdateRange(IEnumerable<TEntity> entities)
        {
            foreach (TEntity entity in entities)
            {
                Update(entity);
            }
        }

        Type IQueryable.ElementType
            => ((IQueryable<TEntity>)_internalDbSet).ElementType;

        Expression IQueryable.Expression
            => ((IQueryable<TEntity>)_internalDbSet).Expression;

        IQueryProvider IQueryable.Provider
            => new MaskingAsyncQueryProvider(((IQueryable<TEntity>)_internalDbSet).Provider, _authorizationProvider.MaskingHandler);

        IServiceProvider IInfrastructure<IServiceProvider>.Instance
            => ((IInfrastructure<IServiceProvider>)_internalDbSet).Instance;

        IList IListSource.GetList()
            => ((IListSource)_internalDbSet).GetList();

    }
}
