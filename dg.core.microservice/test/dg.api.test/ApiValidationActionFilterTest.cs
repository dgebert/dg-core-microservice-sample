using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.Results;using Xunit;
using dg.contract;


namespace dg.api.test
{
    public class ApiValidationActionFilterTest : IClassFixture<TestFixtureWithValidationActionFilter>
    {
        private TestFixtureWithValidationActionFilter _fixture;

        public ApiValidationActionFilterTest(TestFixtureWithValidationActionFilter fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ValidationFilter_Test()
        {
            try
            {
                var p = new Person()
                {
                    FirstName = "supercalifragilisticexpialidocious",
                    LastName = "Willis"
                };

                var response = await _fixture.Client.PostAsync("people3", _fixture.BuildRequestContent(p));

                response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
                ValidationResult result = _fixture.GetValidationResult(response);
                result.IsValid.Should().BeFalse();
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

    }

}
