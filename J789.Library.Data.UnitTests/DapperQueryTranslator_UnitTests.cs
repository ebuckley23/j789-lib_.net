using J789.Library.Data.Dapper;
using J789.Library.Data.Query;
using J789.Library.Data.UnitTests.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace J789.Library.Data.UnitTests
{
    public class DapperQueryTranslator_UnitTests
    {
        [Fact]
        public void Test_DQT()
        {
            var dqt = new DapperQueryTranslator();

            List<int> data = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var query = data.Where(x => x > 3);

            Expression<Func<Ticket, int, int, bool>> test 
                = (Ticket t, int from, int to) => t.TicketNumber >= from && t.TicketNumber <= to;

            var str = dqt.Translate(
                new IdIsEqualTo(5)
                    .Criteria);
            //var str = dqt.Translate(test);
        }
    }

    class IdIsEqualTo : Specification<Ticket>
    {
        public IdIsEqualTo(int id)
            : base(x => x.Id == id)
        {

        }
    }
}
