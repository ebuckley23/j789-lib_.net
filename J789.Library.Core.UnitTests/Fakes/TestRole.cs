namespace J789.Library.Core.UnitTests.Fakes
{
    public class TestRole : Enumeration
    {
        public TestRole(int id, string name, string desc)
            : base(id, name, desc)
        {

        }

        public static TestRole User = new TestRole(1, "UserRole", "This is a user role.");
        public static TestRole Admin = new TestRole(2, "AdminRole", "This is an admin role.");
    }
}
