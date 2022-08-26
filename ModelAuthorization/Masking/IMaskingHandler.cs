using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModelAuthorization.Masking
{
    public interface IMaskingHandler
    {
        ValueTask<T?> MaskAsync<T>(T? value);

        ValueTask<IEnumerable<T?>?> MaskManyAsync<T>(IEnumerable<T?>? values);

        ValueTask<object?> MaskAsync(object? value);

        ValueTask<IEnumerable<object?>?> MaskManyAsync(IEnumerable<object?>? values);

        ValueTask<object?> MaskAsync(object? value, Type type);

        ValueTask<IEnumerable<object?>?> MaskManyAsync(IEnumerable<object?>? values, Type type);

    }
}
