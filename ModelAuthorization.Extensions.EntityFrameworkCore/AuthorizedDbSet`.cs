using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using ModelAuthorization.Enums;
using ModelAuthorization.Extensions.EntityFrameworkCore.Masking;
using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ModelAuthorization.Extensions.EntityFrameworkCore
{
    internal class AuthorizedDbSet<TEntity> : DbSet<TEntity>, IAsyncEnumerable<TEntity>, IQueryable<TEntity>, IInfrastructure<IServiceProvider>, IListSource where TEntity : class
    {
        private readonly ICrudAuthorizationPolicyProvider _authorizationProvider;

        private readonly DbSet<TEntity> _innerDbSet;

        public override IEntityType EntityType => _innerDbSet.EntityType;

        public AuthorizedDbSet(DbSet<TEntity> innerDbSet, ICrudAuthorizationPolicyProvider authorizationProvider)
        {
            _innerDbSet = innerDbSet;
            _authorizationProvider = authorizationProvider;
        }

        public override IAsyncEnumerable<TEntity> AsAsyncEnumerable()
            => _innerDbSet.AsAsyncEnumerable();

        public override IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default)
                => new MaskingAsyncEnumerator<TEntity>(_innerDbSet.GetAsyncEnumerator(cancellationToken), _authorizationProvider.MaskingHandler);

        public override IQueryable<TEntity> AsQueryable()
            => _innerDbSet.AsQueryable();

        public override LocalView<TEntity> Local
            => _innerDbSet.Local;

        public override TEntity? Find(params object?[]? keyValues)
            => _innerDbSet.Find(keyValues);

        public override ValueTask<TEntity?> FindAsync(params object?[]? keyValues)
            => _innerDbSet.FindAsync(keyValues);

        public override ValueTask<TEntity?> FindAsync(object?[]? keyValues, CancellationToken cancellationToken)
            => _innerDbSet.FindAsync(keyValues, cancellationToken);

        public override EntityEntry<TEntity> Add(TEntity entity)
        {
            _authorizationProvider.AuthorizeAsync<TEntity>(CrudPermission.Create).GetAwaiter().GetResult();

            return _innerDbSet.Add(entity);
        }

        public override async ValueTask<EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _authorizationProvider.AuthorizeAsync<TEntity>(CrudPermission.Create);

            return await _innerDbSet.AddAsync(entity, cancellationToken);
        }

        public override EntityEntry<TEntity> Attach(TEntity entity)
            => _innerDbSet.Attach(entity);

        public override EntityEntry<TEntity> Remove(TEntity entity)
        {
            _authorizationProvider.AuthorizeAsync<TEntity>(CrudPermission.Delete).GetAwaiter().GetResult();

            return _innerDbSet.Remove(entity);
        }

        public override EntityEntry<TEntity> Update(TEntity entity)
            => _innerDbSet.Update(entity);

        public override void AddRange(params TEntity[] entities)
            => _innerDbSet.AddRange(entities);

        public override Task AddRangeAsync(params TEntity[] entities)
            => _innerDbSet.AddRangeAsync(entities);

        public override void AttachRange(params TEntity[] entities)
            => _innerDbSet.AttachRange(entities);

        public override void RemoveRange(params TEntity[] entities)
            => _innerDbSet.RemoveRange(entities);

        public override void UpdateRange(params TEntity[] entities)
            => _innerDbSet.UpdateRange();

        public override void AddRange(IEnumerable<TEntity> entities)
        {
            _authorizationProvider.AuthorizeAsync<TEntity>(CrudPermission.Create).GetAwaiter().GetResult();

            _innerDbSet.AddRange(entities);
        }

        public override async Task AddRangeAsync(
            IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
        {
            await _authorizationProvider.AuthorizeAsync<TEntity>(CrudPermission.Create);

            await _innerDbSet.AddRangeAsync(entities, cancellationToken);
        }

        public override void AttachRange(IEnumerable<TEntity> entities)
            => _innerDbSet.AttachRange(entities);


        public override void RemoveRange(IEnumerable<TEntity> entities)
        {
            _authorizationProvider.AuthorizeAsync<TEntity>(CrudPermission.Delete).GetAwaiter().GetResult();

            _innerDbSet.RemoveRange(entities);
        }


        public override void UpdateRange(IEnumerable<TEntity> entities)
            => _innerDbSet.UpdateRange(entities);



        Type IQueryable.ElementType
            => ((IQueryable<TEntity>)_innerDbSet).ElementType;

        Expression IQueryable.Expression
            => ((IQueryable<TEntity>)_innerDbSet).Expression;

        IQueryProvider IQueryable.Provider
            => new MaskingAsyncQueryProvider(((IQueryable<TEntity>)_innerDbSet).Provider, _authorizationProvider.MaskingHandler);

        IServiceProvider IInfrastructure<IServiceProvider>.Instance
            => ((IInfrastructure<IServiceProvider>)_innerDbSet).Instance;

        IList IListSource.GetList()
            => ((IListSource)_innerDbSet).GetList();

    }
}
