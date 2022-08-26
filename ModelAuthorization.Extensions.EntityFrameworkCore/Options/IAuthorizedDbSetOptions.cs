namespace ModelAuthorization.Extensions.EntityFrameworkCore.Options
{
    public interface IAuthorizedDbSetOptions
    {
        bool ThrowOnRestrictedPropertyUpdated { get; }
    }
}
