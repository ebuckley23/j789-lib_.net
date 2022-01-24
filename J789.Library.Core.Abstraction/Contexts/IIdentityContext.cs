using J789.Library.Core.Abstraction.Enums;

namespace J789.Library.Core.Abstraction.Contexts
{
    public interface IIdentityContext
    {
        ContextType ContextType { get; }
        IdentityIntegrationType IdentityIntegrationType { get; }
    }
}
