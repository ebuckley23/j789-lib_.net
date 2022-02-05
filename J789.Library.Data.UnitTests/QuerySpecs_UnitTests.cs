using J789.Library.Data.Query;
using J789.Library.Data.Query.CommonSpecs;
using J789.Library.Data.UnitTests.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace J789.Library.Data.UnitTests
{
    public class QuerySpecs_UnitTests
    {
        [Fact]
        public void Can_Combine_Or_Specification()
        {
            var ticket1 = new Ticket { Id = 1 };
            var ticket2 = new Ticket { Id = 2 };
            var ticket3 = new Ticket { Id = 3 };

            var list = new List<Ticket> { ticket1, ticket2, ticket3 };

            var result = list.Where(new IdIsEqualTo(1).Or(new IdIsEqualTo(3)).Criteria.Compile()).ToList();

            Assert.DoesNotContain(result, x => x.Id == 2);
            Assert.Contains(result, x => x.Id == 1);
            Assert.Contains(result, x => x.Id == 3);
        }

        [Fact]
        public void Can_Combine_And_Specification()
        {
            var ticket1 = new Ticket { Id = 1 };
            var ticket2 = new Ticket { Id = 2 };
            var ticket3 = new Ticket { Id = 3 };
            ticket1.SetIsActive(true);
            ticket3.SetIsActive(true);

            var list = new List<Ticket> { ticket1, ticket2, ticket3 };

            var result = list.Where(new IsActiveSpec<Ticket>().And(new IdIsGreaterThan(0)).Criteria.Compile()).ToList();

            Assert.DoesNotContain(result, x => x.Id == 2);
            Assert.Contains(result, x => x.Id == 1);
            Assert.Contains(result, x => x.Id == 3);
        }

        [Fact]
        public void Can_Get_The_Inverse_Of_The_Specification()
        {
            var ticket1 = new Ticket { Id = 1 };
            var ticket2 = new Ticket { Id = 2 };
            var ticket3 = new Ticket { Id = 3 };
            ticket1.SetIsActive(true);
            ticket3.SetIsActive(true);

            var list = new List<Ticket> { ticket1, ticket2, ticket3 };

            var result = list.Where(new IsActiveSpec<Ticket>().Not().Criteria.Compile()).ToList();

            Assert.DoesNotContain(result, x => x.Id == 1);
            Assert.DoesNotContain(result, x => x.Id == 3);
            Assert.Contains(result, x => x.Id == 2);
        }

        [Fact]
        public void Can_Get_The_Inverse_And_Preserve_The_OrderBy()
        {
            var ticket1 = new Ticket { Id = 1 };
            var ticket2 = new Ticket { Id = 2 };
            var ticket3 = new Ticket { Id = 3 };
            ticket1.SetIsActive(true);
            ticket3.SetIsActive(true);

            var list = new List<Ticket> { ticket1, ticket2, ticket3 };
            var spec = new IdIsEqualTo(2);
            var notSpec = new IdIsEqualTo(2).Not();
            var result = list.Where(notSpec.Criteria.Compile()).ToList();

            Assert.Contains(result, x => x.Id == 1);
            Assert.Contains(result, x => x.Id == 3);
            Assert.DoesNotContain(result, x => x.Id == 2);
            Assert.NotNull(notSpec.OrderByDescending);
            Assert.NotNull(notSpec.OrderBy);
            Assert.Equal(spec.Take, notSpec.Take);
            Assert.Equal(spec.Skip, notSpec.Skip);
        }
        class IdIsEqualTo : Specification<Ticket>
        {
            public IdIsEqualTo(int id)
                : base(x => x.Id == id)
            {
                // strictly for testing purposes. Don't do this.
                ApplyOrderByDescending(x => x.Id);
                ApplyOrderBy(x => x.Id);
                ApplyPaging(1, 5);
            }
        }

        class IdIsGreaterThan : Specification<Ticket>
        {
            public IdIsGreaterThan(int id)
                : base(x => x.Id > id)
            {

            }
        }
    }
}
