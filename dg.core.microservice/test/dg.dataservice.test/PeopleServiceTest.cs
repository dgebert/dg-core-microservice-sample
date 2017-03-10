
using Microsoft.EntityFrameworkCore;
using dg.repository.Models;
using Xunit;
using FluentAssertions;

namespace dg.dataservice.test
{
    public class PeopleServiceTest
    {
        DbContextOptions<PeopleContext> _options;

        public PeopleServiceTest()
        {
            _options = new DbContextOptionsBuilder<PeopleContext>()
                             .UseInMemoryDatabase(databaseName: "People")
                             .Options;
        }

        [Fact]
        public void GivenNoPeopleExist_WhenGetAll_ShouldReturnEmpty()
        {
            using (var db = new PeopleContext(_options))
            {
                var service = new PeopleSqlService(db);

                var allPeople = service.GetAll();
                allPeople.Should().BeEmpty();

            }
        }
    }
}
