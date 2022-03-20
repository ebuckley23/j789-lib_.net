using J789.Library.Core.Abstraction.Security;

namespace J789.Library.Core.Models
{
    public class ContextUserRole : IUserRole
    {
        public ContextUserRole(string name) 
            => Name = name;
        public string Name { get; }
    }
}
