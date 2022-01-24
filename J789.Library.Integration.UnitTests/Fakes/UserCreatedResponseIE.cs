using J789.Library.Integration.Abstraction;

namespace J789.Library.Integration.UnitTests.Fakes
{
    public class UserCreatedResponseIE : IntegrationEvent, IUserCreatedIE
    {
        public string Message { get; set; }
    }

    public interface IUserCreatedResponseIE : IIntegrationEvent
    {
        string Message { get; set; }
    }
}
