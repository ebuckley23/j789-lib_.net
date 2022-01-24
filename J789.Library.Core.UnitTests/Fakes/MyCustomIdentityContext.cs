using J789.Library.Core.Abstraction.Enums;
using J789.Library.Core.Contexts;

namespace J789.Library.Core.UnitTests.Fakes
{
    public class MyCustomIdentityContext : IdentityContext
    {
        public MyCustomIdentityContext(ContextType cType, IdentityIntegrationType idType)
        {
            ContextType = cType;
            SetIdentityIntegrationType(idType);
        }
    }
}
