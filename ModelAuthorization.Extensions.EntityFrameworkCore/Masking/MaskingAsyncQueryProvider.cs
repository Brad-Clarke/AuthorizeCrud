using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using ModelAuthorization.Masking;
using System.Linq.Expressions;

namespace ModelAuthorization.Extensions.EntityFrameworkCore.Masking
{
    internal class MaskingAsyncQueryProvider : IAsyncQueryProvider
    {
        private readonly IAsyncQueryProvider _internalProvider;

        private readonly IMaskingHandler _maskingHandler;

        public MaskingAsyncQueryProvider(IQueryProvider internalProvider, IMaskingHandler maskingHandler)
        {
            if (internalProvider is not IAsyncQueryProvider asyncProvider)
            {
                throw new InvalidOperationException(CoreStrings.IQueryableProviderNotAsync);
            }

            _internalProvider = asyncProvider;
            _maskingHandler = maskingHandler;
        }

        public IQueryable CreateQuery(Expression expression)
            => _internalProvider.CreateQuery(expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => _internalProvider.CreateQuery<TElement>(expression);

        public object? Execute(Expression expression)
            => _internalProvider.Execute(expression);

        public TResult Execute<TResult>(Expression expression)
        {
            TResult result = _internalProvider.Execute<TResult>(expression);

            return _maskingHandler.MaskAsync(result).GetAwaiter().GetResult()!;
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = new CancellationToken())
        {
            Task task = (Task)(object)_internalProvider.ExecuteAsync<TResult>(expression, cancellationToken);

            dynamic result = ((dynamic)task).Result;

            return Task.FromResult(_maskingHandler.MaskAsync(result).GetAwaiter().GetResult());
        }
    }
}
