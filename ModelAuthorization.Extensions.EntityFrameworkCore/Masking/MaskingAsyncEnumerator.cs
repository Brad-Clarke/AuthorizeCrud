using ModelAuthorization.Masking;

namespace ModelAuthorization.Extensions.EntityFrameworkCore.Masking
{
    internal class MaskingAsyncEnumerator<TEntity> : IAsyncEnumerator<TEntity> where TEntity : class
    {
        private readonly IAsyncEnumerator<TEntity> _internalEnumerator;

        private readonly IMaskingHandler _maskingHandler;

        public TEntity Current { get; private set; } = null!;

        public MaskingAsyncEnumerator(IAsyncEnumerator<TEntity> internalEnumerator, IMaskingHandler maskingHandler)
        {
            _internalEnumerator = internalEnumerator;
            _maskingHandler = maskingHandler;
        }

        public ValueTask DisposeAsync() => _internalEnumerator.DisposeAsync();

        public async ValueTask<bool> MoveNextAsync()
        {
            bool hasNext = await _internalEnumerator.MoveNextAsync();

            Current = _internalEnumerator.Current;

            await _maskingHandler.MaskAsync(Current);

            return hasNext;
        }
    }
}
