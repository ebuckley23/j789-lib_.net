using J789.Library.Core.Abstraction.Security;

namespace J789.Library.Core.Models
{
    public class ContextPermission : IPermission
    {
        public ContextPermission(string name) 
            => Name = name;

        public string Name { get; set; }
    }
}
