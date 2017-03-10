
using Microsoft.EntityFrameworkCore;
using dg.repository.Models;
using Xunit;

namespace dg.dataservice.test
{
    public class PeopleServiceTest
    {
        public PeopleServiceTest()
        {
            var options = new DbContextOptionsBuilder<PeopleContext>()
                     .UseInMemoryDatabase(databaseName: "People")
                     .Options;
        }

        [Fact]
        public void GivenNoPeopleExist_WhenGetAll_ShouldReturnEmpty()
        {

        }
    }
}
