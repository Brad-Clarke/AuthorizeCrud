using ModelAuthorization.Enums;
using ModelAuthorization.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ModelAuthorization.Masking
{
    public class DefaultMaskingHandler : IMaskingHandler
    {
        private readonly ICrudAuthorizationPolicyProvider _policyProvider;

        public DefaultMaskingHandler(ICrudAuthorizationPolicyProvider policyProvider)
        {
            _policyProvider = policyProvider;
        }

        public async ValueTask<T?> MaskAsync<T>(T? value)
        {
            if (value is null)
            {
                return value;
            }

            Type type = typeof(T);

            foreach (PropertyInfo property in typeof(T).GetPublicProperties())
            {
                if (await _policyProvider.AuthorizePropertyAsync(type, property, CrudPermission.Read))
                {
                    continue;
                }

                MaskProperty(property, value);
            }

            return value;
        }

        public async ValueTask<IEnumerable<T?>?> MaskManyAsync<T>(IEnumerable<T?>? values)
        {
            if (values is null)
            {
                return values;
            }

            Type type = typeof(T);

            T?[] valueArray = values.ToArray();

            foreach (PropertyInfo property in typeof(T).GetPublicProperties())
            {
                if (await _policyProvider.AuthorizePropertyAsync(type, property, CrudPermission.Read))
                {
                    continue;
                }

                foreach (T? value in valueArray)
                {
                    if (value is null)
                    {
                        continue;
                    }

                    MaskProperty(property, value);
                }
            }

            return valueArray;
        }

        public ValueTask<object?> MaskAsync(object? value)
        {
            if (value is null)
            {
                return new ValueTask<object?>(value);
            }

            return MaskAsync(value, value.GetType());
        }

        public ValueTask<IEnumerable<object?>?> MaskManyAsync(IEnumerable<object?>? values)
        {
            if (values is null)
            {
                return new ValueTask<IEnumerable<object?>?>(values);
            }

            return MaskManyAsync(values, values.GetGenericType());
        }

        public async ValueTask<object?> MaskAsync(object? value, Type type)
        {
            if (value is null)
            {
                return value;
            }

            foreach (PropertyInfo property in value.GetType().GetPublicProperties())
            {
                if (await _policyProvider.AuthorizePropertyAsync(type, property, CrudPermission.Read))
                {
                    continue;
                }

                MaskProperty(property, value);
            }

            return value;
        }

        public ValueTask<IEnumerable<object?>?> MaskManyAsync(IEnumerable<object?>? values, Type type)
        {
            throw new NotImplementedException();
        }

        protected virtual void MaskProperty(PropertyInfo property, object source)
        {
            property.SetValue(source, default);
        }
    }
}
