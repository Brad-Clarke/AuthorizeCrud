namespace ModelAuthorization.Extensions.EntityFrameworkCore.Options
{
    public class AuthorizedDbSetOptions : IAuthorizedDbSetOptions
    {
        public bool ThrowOnRestrictedPropertyUpdated { get; set; }
    }
}
