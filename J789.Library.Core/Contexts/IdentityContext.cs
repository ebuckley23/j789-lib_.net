using J789.Library.Core.Abstraction.Contexts;
using J789.Library.Core.Abstraction.Enums;
using System.Security.Claims;

namespace J789.Library.Core.Contexts
{
    public abstract class IdentityContext : IIdentityContext
    {
        protected readonly ClaimsPrincipal _claimsPrincipal;

        protected IdentityContext(ClaimsPrincipal claimsPrincipal)
            : this()
            => _claimsPrincipal = claimsPrincipal;

        protected IdentityContext() { }

        public IdentityIntegrationType IdentityIntegrationType { get; private set; }
        public ContextType ContextType { get; protected set; }
        protected void SetIdentityIntegrationType(IdentityIntegrationType iiType) 
            => IdentityIntegrationType = iiType;
    }
}
