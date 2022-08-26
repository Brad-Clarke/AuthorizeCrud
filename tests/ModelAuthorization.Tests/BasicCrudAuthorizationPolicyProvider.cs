using ModelAuthorization.Attributes;
using ModelAuthorization.Enums;
using Shouldly;
using System.Reflection;

namespace ModelAuthorization.Tests
{
    public class BasicCrudAuthorizationPolicyProviderShould
    {
        [AllowCrud("test.policy.all", CrudPermission.Create | CrudPermission.Read | CrudPermission.Update | CrudPermission.Delete)]
        [AllowCrud("test.policy.read_delete", CrudPermission.Read | CrudPermission.Delete)]
        [AllowCrud("test.policy.read", CrudPermission.Read)]
        private class TestClass
        {
            [RestrictCrud(CrudPermission.Update)]
            public Guid Id { get; set; }

            public string Name { get; set; }

            [AllowCrud("test.policy.read", CrudPermission.Update)]
            public string Email { get; set; }

            [RestrictCrud(CrudPermission.Read)]
            [AllowCrud("test.policy.all", CrudPermission.Read)]
            public DateTime BirthDate { get; set; }
        }

        [Theory]
        [InlineData("test.policy.all", CrudPermission.Create, true)]
        [InlineData("test.policy.all", CrudPermission.Read, true)]
        [InlineData("test.policy.all", CrudPermission.Update, true)]
        [InlineData("test.policy.all", CrudPermission.Delete, true)]
        [InlineData("test.policy.read_delete", CrudPermission.Create, false)]
        [InlineData("test.policy.read_delete", CrudPermission.Read, true)]
        [InlineData("test.policy.read_delete", CrudPermission.Update, false)]
        [InlineData("test.policy.read_delete", CrudPermission.Delete, true)]
        [InlineData("test.policy.read", CrudPermission.Create, false)]
        [InlineData("test.policy.read", CrudPermission.Read, true)]
        [InlineData("test.policy.read", CrudPermission.Update, false)]
        [InlineData("test.policy.read", CrudPermission.Delete, false)]
        public async Task AuthorizeForClassAsync(string policy, CrudPermission permissions, bool isAuthorized)
        {
            ICrudAuthorizationPolicyProvider provider = new BasicCrudAuthorizationPolicyProvider(policy);

            bool result = await provider.AuthorizeTypeAsync(typeof(TestClass), permissions);

            result.ShouldBe(isAuthorized);
        }

        [Theory]
        [InlineData("test.policy.all", nameof(TestClass.Id), CrudPermission.Read, true)]
        [InlineData("test.policy.all", nameof(TestClass.Name), CrudPermission.Read, true)]
        [InlineData("test.policy.all", nameof(TestClass.Email), CrudPermission.Read, true)]
        [InlineData("test.policy.all", nameof(TestClass.BirthDate), CrudPermission.Read, true)]
        [InlineData("test.policy.all", nameof(TestClass.Id), CrudPermission.Update, false)]
        [InlineData("test.policy.all", nameof(TestClass.Name), CrudPermission.Update, true)]
        [InlineData("test.policy.all", nameof(TestClass.Email), CrudPermission.Update, true)]
        [InlineData("test.policy.all", nameof(TestClass.BirthDate), CrudPermission.Update, true)]
        [InlineData("test.policy.read_delete", nameof(TestClass.Id), CrudPermission.Read, true)]
        [InlineData("test.policy.read_delete", nameof(TestClass.Name), CrudPermission.Read, true)]
        [InlineData("test.policy.read_delete", nameof(TestClass.Email), CrudPermission.Read, true)]
        [InlineData("test.policy.read_delete", nameof(TestClass.BirthDate), CrudPermission.Read, false)]
        [InlineData("test.policy.read_delete", nameof(TestClass.Id), CrudPermission.Update, false)]
        [InlineData("test.policy.read_delete", nameof(TestClass.Name), CrudPermission.Update, false)]
        [InlineData("test.policy.read_delete", nameof(TestClass.Email), CrudPermission.Update, false)]
        [InlineData("test.policy.read_delete", nameof(TestClass.BirthDate), CrudPermission.Update, false)]
        [InlineData("test.policy.read", nameof(TestClass.Id), CrudPermission.Read, true)]
        [InlineData("test.policy.read", nameof(TestClass.Name), CrudPermission.Read, true)]
        [InlineData("test.policy.read", nameof(TestClass.Email), CrudPermission.Read, true)]
        [InlineData("test.policy.read", nameof(TestClass.BirthDate), CrudPermission.Read, false)]
        [InlineData("test.policy.read", nameof(TestClass.Id), CrudPermission.Update, false)]
        [InlineData("test.policy.read", nameof(TestClass.Name), CrudPermission.Update, false)]
        [InlineData("test.policy.read", nameof(TestClass.Email), CrudPermission.Update, true)]
        [InlineData("test.policy.read", nameof(TestClass.BirthDate), CrudPermission.Update, false)]
        public async Task AuthorizeForPropertyAsync(string policy, string propertyName, CrudPermission permissions, bool isAuthorized)
        {
            ICrudAuthorizationPolicyProvider provider = new BasicCrudAuthorizationPolicyProvider(policy);

            PropertyInfo? property = typeof(TestClass).GetProperty(propertyName);

            property.ShouldNotBeNull();

            bool result = await provider.AuthorizePropertyAsync(typeof(TestClass), property, permissions);

            result.ShouldBe(isAuthorized);
        }
    }
}