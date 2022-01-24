using J789.Library.Core.Abstraction.Enums;
using J789.Library.Core.Abstraction.Security;
using System.Collections.Generic;

namespace J789.Library.Core.Abstraction.Contexts
{
    public interface IUserContext
    {
        string TenantId { get; }
        string TenantName { get; }
        string TenantOwnerId { get; }
        string Email { get; }
        string FirstName { get; }
        string IDProvider { get; }
        string LastName { get; }
        string Name { get; }
        string UserName { get; }
        IEnumerable<IPermission> Permissions { get; }
        string SubjectId { get; }
        bool IsTenantOwner { get; }
        bool HasPermission(IPermission permission);
        bool HasPermissions(params IPermission[] permissions);
        SocialProviderType SocialProvider { get; }
    }
}
