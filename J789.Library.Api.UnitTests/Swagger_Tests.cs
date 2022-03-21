using J789.Library.Api.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Moq;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace J789.Library.Api.UnitTests
{
    public class Swagger_Tests
    {
        [Fact]
        public void Can_Configure_Authorized_Security_Requirements_Operation_Filter_Correctly()
        {
            var filter = new SecurityRequirementsOperationFilter();

            var openApiOp = new OpenApiOperation();
            var apidesc = new ApiDescription();
            var schemaRepo = new SchemaRepository();
            var mockSchemeGen = new Mock<ISchemaGenerator>();
            MethodInfo mi = typeof(TestApiController).GetMethod(nameof(TestApiController.GetAsync));

            var opFilterCtx = new OperationFilterContext(apidesc, mockSchemeGen.Object, schemaRepo, mi);

            filter.Apply(openApiOp, opFilterCtx);

            Assert.True(openApiOp.Responses.ContainsKey("401"));
            Assert.True(openApiOp.Responses.ContainsKey("403"));
            Assert.NotEmpty(openApiOp.Security);
        }

        [Fact]
        public void Can_Configure_AllowAnonymous_Security_Requirements_Operation_Filter_Correctly()
        {
            var filter = new SecurityRequirementsOperationFilter();

            var openApiOp = new OpenApiOperation();
            var apidesc = new ApiDescription();
            var schemaRepo = new SchemaRepository();
            var mockSchemeGen = new Mock<ISchemaGenerator>();
            MethodInfo mi = typeof(TestApiController).GetMethod(nameof(TestApiController.GetAnonymousAsync));

            var opFilterCtx = new OperationFilterContext(apidesc, mockSchemeGen.Object, schemaRepo, mi);

            filter.Apply(openApiOp, opFilterCtx);

            Assert.False(openApiOp.Responses.ContainsKey("401"));
            Assert.False(openApiOp.Responses.ContainsKey("403"));
            Assert.Empty(openApiOp.Security);
        }

        [InlineData("https://example.auth0.com", "https://unitests", "")]
        [InlineData("https://example.auth0.com/", "https://unitests", "")]
        [InlineData("https://example.auth0.com", "http://unittests", "read:email,update:user,create:role")]
        [Theory]
        public void Can_Configure_Implicit_Authorization_Flow_Correctly(string authority, string audience, string additionalScopes)
        {
            var swaggerGenOpts = new SwaggerGenOptions();

            Assert.Empty(swaggerGenOpts.SwaggerGeneratorOptions.SecuritySchemes);

            swaggerGenOpts.AddImplicitFlowAuthorization(authority, audience, additionalScopes.Split(","));

            Assert.NotEmpty(swaggerGenOpts.SwaggerGeneratorOptions.SecuritySchemes);
            Assert.True(swaggerGenOpts.SwaggerGeneratorOptions.SecuritySchemes.ContainsKey("Bearer"));

            var scheme = swaggerGenOpts.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];

            Assert.Equal("Authorization", scheme.Name);
            Assert.Equal(Microsoft.OpenApi.Models.ParameterLocation.Header, scheme.In);
            Assert.Equal(Microsoft.OpenApi.Models.SecuritySchemeType.OAuth2, scheme.Type);
            Assert.NotNull(scheme.Flows);
            Assert.NotNull(scheme.Flows.Implicit);
            Assert.NotEmpty(scheme.Flows.Implicit.Scopes);
            Assert.NotNull(scheme.Flows.Implicit.AuthorizationUrl);
            Assert.Contains(authority, scheme.Flows.Implicit.AuthorizationUrl.AbsoluteUri);
            Assert.Contains(audience, scheme.Flows.Implicit.AuthorizationUrl.AbsoluteUri);
            if (!string.IsNullOrEmpty(additionalScopes))
            {
                foreach(var s in additionalScopes.Split(","))
                {
                    Assert.True(scheme.Flows.Implicit.Scopes.ContainsKey(s));
                }
            }
        }
    }

    internal class TestApiController : ControllerBase
    {
        public Task GetAsync()
        {
            return Task.CompletedTask;
        }

        [AllowAnonymous]
        public Task GetAnonymousAsync()
        {
            return Task.CompletedTask;
        }
    }
}