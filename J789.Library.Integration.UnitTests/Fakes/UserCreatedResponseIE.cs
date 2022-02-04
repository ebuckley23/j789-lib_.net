using J789.Library.Integration.Abstraction;

namespace J789.Library.Integration.UnitTests.Fakes
{
    public class UserCreatedResponseIE : IntegrationEvent, IUserCreatedResponseIE
    {
        public string Message { get; set; }
    }

    public interface IUserCreatedResponseIE : IIntegrationEvent
    {
        string Message { get; set; }
    }
}
