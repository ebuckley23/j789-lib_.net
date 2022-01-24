using J789.Library.Core.Abstraction.Enums;
using J789.Library.Core.Contexts;
using J789.Library.Core.Models;
using J789.Library.Core.UnitTests.Fakes;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Linq;
using Xunit;

namespace J789.Library.Core.UnitTests
{
    public class Contexts_Tests
    {
        [InlineData(ContextType.Integration, IdentityIntegrationType.AmazonCognito)]
        [Theory]
        public void Can_Set_Context_And_Integration_Type(ContextType cType, IdentityIntegrationType idType)
        {
            var context = new MyCustomIdentityContext(cType, idType);

            Assert.Equal(cType, context.ContextType);
            Assert.Equal(idType, context.IdentityIntegrationType);
        }

        [InlineData("subjectId", "firstName", "lastName", "email", "a5dae037-1ddb-4723-b3e9-c8882298336c", "Manny's Business", "a5dae0371ddb4723b3e9")]
        [Theory]
        public void Can_Create_Configuration_Based_UserContext(
            string subjectId, 
            string firstName, 
            string lastName, 
            string email, 
            string tenantId, 
            string tenantName, 
            string tenantOwnerId)
        {
            var tenantIdPropName = "MyTenantId";
            var tenantNamePropName = "MyTenantName";
            var tenantOwnerIdPropName = "MyOwnerId";
            var permissionPropName = "Permission";
            var expectedPerms = new[] { "permission1", "permission2", "permission3" };

            var cp = new FakeClaimsPrincipal()
                .AsJwtClaim(subjectId, firstName, lastName, email, idProvider: "Google");

            cp.AddCustomClaim(tenantIdPropName, tenantId);
            cp.AddCustomClaim(tenantNamePropName, tenantName);
            cp.AddCustomClaim(tenantOwnerIdPropName, tenantOwnerId);

            foreach (var p in expectedPerms) cp.AddCustomClaim(permissionPropName, p);

            var userContext = new UserContext(GetUserContextConfiguration(), cp, ContextType.User);

            Assert.Equal(subjectId, userContext.SubjectId);
            Assert.Equal(firstName, userContext.FirstName);
            Assert.Equal(lastName, userContext.LastName);
            Assert.Equal(email, userContext.Email);
            Assert.Equal(email, userContext.UserName);
            Assert.Equal($"{firstName} {lastName}", userContext.Name);
            Assert.Equal("Google", userContext.IDProvider);
            Assert.False(userContext.IsTenantOwner);
            Assert.Equal(tenantId, userContext.TenantId);
            Assert.Equal(tenantName, userContext.TenantName);
            Assert.Equal(tenantOwnerId, userContext.TenantOwnerId);
            Assert.True(userContext.HasPermissions(expectedPerms.Select(ep => new ContextPermission(ep)).ToArray()));
        }

        [InlineData("subjectId", "firstName", "lastName", "email", "a5dae037-1ddb-4723-b3e9-c8882298336c", "Manny's Business", "a5dae0371ddb4723b3e9")]
        [Theory]
        public void Can_Create_Empty_Configuration_Based_UserContext(
            string subjectId,
            string firstName,
            string lastName,
            string email,
            string tenantId,
            string tenantName,
            string tenantOwnerId)
        {
            var tenantIdPropName = "MyCustomIdProp";
            var tenantNamePropName = "MyCustomTenantNameProp";
            var tenantOwnerIdPropName = "MyCustomOwnerIdProp";
            var permissionPropName = "PermissionProp";
            var expectedPerms = new[] { "permission1", "permission2", "permission3" };

            var cp = new FakeClaimsPrincipal()
                .AsJwtClaim(subjectId, firstName, lastName, email, idProvider: "Facebook");

            cp.AddCustomClaim(tenantIdPropName, tenantId);
            cp.AddCustomClaim(tenantNamePropName, tenantName);
            cp.AddCustomClaim(tenantOwnerIdPropName, tenantOwnerId);

            foreach (var p in expectedPerms) cp.AddCustomClaim(permissionPropName, p);

            var userContext = new UserContext(cp, ContextType.User);

            Assert.Equal(subjectId, userContext.SubjectId);
            Assert.Equal(firstName, userContext.FirstName);
            Assert.Equal(lastName, userContext.LastName);
            Assert.Equal(email, userContext.Email);
            Assert.Equal(email, userContext.UserName);
            Assert.Equal($"{firstName} {lastName}", userContext.Name);
            Assert.Equal("Facebook", userContext.IDProvider);
            Assert.False(userContext.IsTenantOwner);
            Assert.Equal(userContext.TenantId, string.Empty);
            Assert.Equal(userContext.TenantName, string.Empty);
            Assert.Equal(userContext.TenantOwnerId, string.Empty);
            Assert.False(userContext.HasPermissions(expectedPerms.Select(ep => new ContextPermission(ep)).ToArray()));
        }

        [Fact]
        public void Can_Set_Additional_Properties_On_Context()
        {
            var cp = new FakeClaimsPrincipal()
                .AsJwtClaim("SubjectId", "fName", "lname", "email", idProvider: "Facebook");

            var additionalProp1Name = "AdditionalProp1";
            var additionalProp1Value = "Value1";
            var additionalProp2Name = "AdditionalProp2";
            var additionalProp2Value = "Value2";
            var additionalProp3Name = "AdditionalProp3";
            var prop3Value = "value";

            cp.AddCustomClaim(additionalProp1Name, additionalProp1Value);
            cp.AddCustomClaim(additionalProp2Name, additionalProp2Value);
            cp.AddCustomClaim(additionalProp3Name, prop3Value);
            cp.AddCustomClaim(additionalProp3Name, prop3Value);
            cp.AddCustomClaim(additionalProp3Name, prop3Value);

            var userContext = new UserContext(GetUserContextConfiguration(), cp, ContextType.User);

            Assert.Equal(additionalProp1Value, userContext.Get(additionalProp1Name));
            Assert.Equal(additionalProp2Value, userContext.Get(additionalProp2Name));
            Assert.Equal(3, userContext.GetAll(additionalProp3Name).Count());
        }

        [Fact]
        public void Can_Create_Anonymous_UserContext()
        {
            var userContext = UserContext.AsAnonymous() as UserContext;

            Assert.Equal(ContextType.Anonymous, userContext.ContextType);
        }

        private IConfiguration GetUserContextConfiguration()
        {
            var mockConfigSection = new Mock<IConfigurationSection>();

            var baseContextConfigPath = "Core:UserContext";
            var config = new Mock<IConfiguration>();
            config.SetupGet(x => x[It.Is<string>(s => s == $"{baseContextConfigPath}:TenantIdConfig")])
                .Returns("MyTenantId");
            config.SetupGet(x => x[It.Is<string>(s => s == $"{baseContextConfigPath}:TenantNameConfig")])
                .Returns("MyTenantName");
            config.SetupGet(x => x[It.Is<string>(s => s == $"{baseContextConfigPath}:TenantOwnerIdConfig")])
                .Returns("MyOwnerId");
            config.SetupGet(x => x[It.Is<string>(s => s == $"{baseContextConfigPath}:TenantPermissionConfig")])
                .Returns("Permission");
            config.Setup(x => x.GetSection(It.Is<string>(s => s == $"{baseContextConfigPath}:AdditionalProperties")))
                .Returns(mockConfigSection.Object);

            return config.Object;
        }
    }
}
