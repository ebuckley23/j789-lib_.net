using IdentityModel;
using J789.Library.Core.Abstraction.Contexts;
using J789.Library.Core.Abstraction.Enums;
using J789.Library.Core.Abstraction.Security;
using J789.Library.Core.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace J789.Library.Core.Contexts
{
    public class UserContext : IdentityContext, IUserContext
    {
        protected readonly IEnumerable<Claim> _claims;
        private readonly IConfiguration _configuration;

        public const string USER_CONTEXT_CONFIG_PATH = "Core:UserContext";
        public const string TENANT_CONFIG_ID_PROP = USER_CONTEXT_CONFIG_PATH + ":TenantIdConfig";
        public const string TENANT_CONFIG_NAME_PROP = USER_CONTEXT_CONFIG_PATH + ":TenantNameConfig";
        public const string TENANT_CONFIG_OWNERID_PROP = USER_CONTEXT_CONFIG_PATH + ":TenantOwnerIdConfig";
        public const string TENANT_CONFIG_PERMISSION_PROP =  USER_CONTEXT_CONFIG_PATH + ":TenantPermissionConfig";
        public const string ADDITIONAL_PROPERTIES_CONFIG_PROP = USER_CONTEXT_CONFIG_PATH + ":AdditionalProperties";
        private const string TENANT_ID = "tenant_id";
        private const string TENANT_NAME = "tenant_name";
        private const string TENANT_OWNER_ID = "tenant_owner_id";
        private const string TENANT_PERMISSION = "tenant_permission";

        public UserContext(ClaimsPrincipal claimsPrincipal)
            : base(claimsPrincipal)
        {
            _claims = _claimsPrincipal.Claims;
            SetIdentityIntegrationType(IdentityIntegrationType.Local);
        }

        public UserContext(IConfiguration configuration, ClaimsPrincipal claimsPrincipal)
            : this(claimsPrincipal)
        {
            _configuration = configuration;
        }

        public UserContext(IConfiguration configuration, ClaimsPrincipal claimsPrincipal, ContextType contextType)
            : this(configuration, claimsPrincipal)
        {
            ContextType = contextType;
        }

        public UserContext(ClaimsPrincipal claimsPrincipal, ContextType contextType)
            : this(claimsPrincipal)
        {
            ContextType = contextType;
        }
        /// <summary>
        /// User's Identity Provider (Ex. Google, Facebook)
        /// </summary>
        public string IDProvider
        {
            get
            {
                var claims = _claimsPrincipal.Claims;
                return claims.LastOrDefault(c => c.Type == JwtClaimTypes.IdentityProvider)?.Value;
            }
        }

        /// <summary>
        /// User's Unique Identifier
        /// </summary>
        public string SubjectId
        {
            get
            {
                var claims = _claimsPrincipal.Claims;
                return claims.LastOrDefault(c => c.Type == JwtClaimTypes.Subject)?.Value
                    ?? claims.LastOrDefault(c => c.Type == "sub")?.Value;
            }
        }

        /// <summary>
        /// User's full name
        /// </summary>
        public string Name
        {
            get
            {
                var claims = _claimsPrincipal.Claims;
                return claims.LastOrDefault(c => c.Type == JwtClaimTypes.Name)?.Value
                    ?? claims.LastOrDefault(c => c.Type == ClaimTypes.Name)?.Value
                    ?? $"{FirstName} {LastName}";
            }
        }

        /// <summary>
        /// User's email address
        /// </summary>
        public string Email
        {
            get
            {
                var claims = _claimsPrincipal.Claims;
                return claims.LastOrDefault(c => c.Type == JwtClaimTypes.Email)?.Value
                    ?? claims.LastOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            }
        }

        /// <summary>
        /// User's Firstname
        /// </summary>
        public string FirstName
        {
            get
            {
                var claims = _claimsPrincipal.Claims;
                //var (firstname, lastname) = GetNameParts();
                return claims.LastOrDefault(c => c.Type == JwtClaimTypes.GivenName)?.Value
                    ?? claims.LastOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value
                    ?? string.Empty;
            }
        }

        /// <summary>
        /// User's Lastname
        /// </summary>
        public string LastName
        {
            get
            {
                var claims = _claimsPrincipal.Claims;
                return claims.LastOrDefault(c => c.Type == JwtClaimTypes.FamilyName)?.Value
                    ?? claims.LastOrDefault(c => c.Type == ClaimTypes.Surname)?.Value
                    ?? string.Empty;
            }
        }

        public virtual string TenantId
        {
            get
            {
                var tenantIdConfig = _configuration?[TENANT_CONFIG_ID_PROP] ?? TENANT_ID;
                var claims = _claimsPrincipal.Claims;
                return claims.LastOrDefault(c => c.Type == tenantIdConfig)?.Value ?? string.Empty;
            }
        }

        public virtual string TenantName
        {
            get
            {
                var tenantNameConfig = _configuration?[TENANT_CONFIG_NAME_PROP] ?? TENANT_NAME;
                var claims = _claimsPrincipal.Claims;
                return claims.LastOrDefault(c => c.Type == tenantNameConfig)?.Value ?? string.Empty;
            }
        }
        public virtual string TenantOwnerId
        {
            get
            {
                var tenantOwnerIdConfig = _configuration?[TENANT_CONFIG_OWNERID_PROP] ?? TENANT_OWNER_ID;
                var claims = _claimsPrincipal.Claims;
                return claims.LastOrDefault(c => c.Type == tenantOwnerIdConfig)?.Value ?? string.Empty;
            }
        }

        public virtual bool IsTenantOwner
            => TenantOwnerId == SubjectId;

        public virtual IEnumerable<IPermission> Permissions
        {
            get
            {
                var tenantPermissionConfig = _configuration?[TENANT_CONFIG_PERMISSION_PROP] ?? TENANT_PERMISSION;
                var claims = _claimsPrincipal.Claims;
                var tenant_perms = claims.Where(c => c.Type == tenantPermissionConfig);

                if (tenant_perms.Count() == 0) return Enumerable.Empty<IPermission>();

                return tenant_perms.Select(c => new ContextPermission(c.Value));
            }
        }

        public virtual string UserName => Email;

        public virtual SocialProviderType SocialProvider
            => SocialProviderType.None;

        public bool HasPermission(IPermission permission)
        {
            return Permissions.Any(p => p.Name == permission.Name);
        }

        public static IUserContext AsAnonymous()
            => new UserContext(new ClaimsPrincipal(), ContextType.Anonymous);

        public bool HasPermissions(params IPermission[] permissions)
        {
            var ret = true;

            foreach (var p in permissions)
            {
                if (!HasPermission(p))
                {
                    ret = false;
                }
            }

            return ret;
        }

        public string Get(string property)
        {
            var claims = _claimsPrincipal.Claims;
            return claims.LastOrDefault(c => c.Type.ToLower() == property?.ToLower())?.Value;
        }

        public IEnumerable<string> GetAll(string property)
        {
            var claims = _claimsPrincipal.Claims;
            return claims.Where(c => c.Type.ToLower() == property?.ToLower())
                .Select(c => c.Value);
        }
    }
}
