using J789.Library.Core.UnitTests.Fakes;
using System;
using Xunit;

namespace J789.Library.Core.UnitTests
{
    public class Enumeration_Tests
    {
        [Fact]
        public void Can_Get_All_Of_Enumeration_Type()
        {
            var expectedRoles = new[] { TestRole.User, TestRole.Admin };

            var roles = Enumeration.GetAll<TestRole>();

            Assert.All(roles, role =>
            {
                Assert.Contains(role, expectedRoles);
            });
        }

        [Fact]
        public void ToString_Displays_Enumeration_Name()
        {
            Assert.Equal(TestRole.User.Name, TestRole.User.ToString());
        }

        [Fact]
        public void Can_Compare_Enumeration_Types()
        {
            var ur = TestRole.User;
            var ar = TestRole.Admin;

            var dupeUR = new TestRole(ur.Id, ur.Name, ur.Description);
            var dupeAR = new TestRole(ar.Id, ar.Name, ar.Description);

            var incorrectUserROle = new TestRole(99, ur.Name, ur.Description);

            Assert.Equal(ur, dupeUR);
            Assert.Equal(ar, dupeAR);
            Assert.NotEqual(ur, ar);
            Assert.NotEqual(ur, incorrectUserROle);
            Assert.True(ur == dupeUR);
            Assert.True(ur != ar);
        }

        [Fact]
        public void Can_Get_By_Id()
        {
            Assert.Equal(TestRole.User, Enumeration.FromId<TestRole>(TestRole.User.Id));
        }

        [Fact]
        public void Can_Get_By_Name()
        {
            Assert.Equal(TestRole.Admin, Enumeration.FromName<TestRole>(TestRole.Admin.Name));
        }

        [Fact]
        public void Can_Cast_To_Id()
        {
            int val = TestRole.Admin;
            Assert.Equal(val, (int)TestRole.Admin);
        }

        [Fact]
        public void Throws_Invalid_Operations_Exception_When_Id_Or_Name_Is_Missing()
        {
            Assert.Throws<InvalidOperationException>(() => Enumeration.FromId<TestRole>(99));
            Assert.Throws<InvalidOperationException>(() => Enumeration.FromName<TestRole>(nameof(Throws_Invalid_Operations_Exception_When_Id_Or_Name_Is_Missing)));
        }
    }
}
