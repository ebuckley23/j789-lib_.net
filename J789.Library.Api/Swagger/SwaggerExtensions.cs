using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace J789.Library.Api.Swagger
{
    public static class SwaggerExtensions
    {
        /// <summary>
        /// This will enable the SwaggerUI to be treated as a UI application able to retrieve an authorization token via the implicit flow.
        /// When attempting to authorize, swagger will be redirected to the configured login page from the authentication authority being used, then
        /// rerouted back to the swagger UI.
        /// Important: SwaggerUI url should be configured as a valid callback url with the authentication authority
        /// </summary>
        /// <param name="options"></param>
        /// <param name="authority"></param>
        /// <param name="audience"></param>
        public static void AddImplicitFlowAuthorization(this SwaggerGenOptions options, string authority, string audience)
        {
            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.OAuth2,
                Flows = new Microsoft.OpenApi.Models.OpenApiOAuthFlows
                {
                    Implicit = new Microsoft.OpenApi.Models.OpenApiOAuthFlow
                    {
                        Scopes = new Dictionary<string, string> { { "openid", "Open Id" } },
                        AuthorizationUrl = new Uri(BuildAuthorizationUrl(authority, audience))
                    }
                }
            });
        }

        private static string BuildAuthorizationUrl(string authority, string audience)
        {
            var auth = authority;
            if (authority.Length - 1 == authority.LastIndexOf("/"))
            {
                auth = authority.Substring(0, authority.LastIndexOf("/"));
            }
            return $"{auth}/authorize?audience={audience}";
        }
    }
}
