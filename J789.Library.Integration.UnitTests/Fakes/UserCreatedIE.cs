using J789.Library.Integration.Abstraction;

namespace J789.Library.Integration.UnitTests
{
    public class UserCreatedIE : IntegrationEvent, IUserCreatedIE
    {
        public string Message { get; set; }
    }

    public interface IUserCreatedIE : IIntegrationEvent
    {
        string Message { get; set; }
    }
}
