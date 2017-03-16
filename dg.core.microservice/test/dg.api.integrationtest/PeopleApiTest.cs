using dg.contract;
using dg.dataservice;
using dg.unittest.api;
using FluentAssertions;
using FluentValidation.Results;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace dg.api.integrationtest
{
    public class PeopleApiTest : IClassFixture<TestServerFixture>
    {
        private TestServerFixture _fixture;

        public PeopleApiTest(TestServerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GivenInvalidPerson_WhenCreate_ShouldReturnBadRequest()
        {

            var p = new Person()
            {
                FirstName = "supercalifragilisticexpialidocious",
                LastName = "Willis"
            };

            var stringContent = _fixture.BuildRequestContent(p);
            var response = await _fixture.Client.PostAsync("people", stringContent);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            ValidationResult result = _fixture.GetValidationResult(response);
            result.IsValid.Should().BeFalse();
        }
    }
}
