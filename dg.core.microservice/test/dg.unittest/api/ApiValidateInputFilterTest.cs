using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.Results;using Xunit;
using dg.contract;


namespace dg.unittest.api
{
    public class ApiValidateInputFilterTest : IClassFixture<TestServerFixtureforValidateInputFilter>
    {
        private TestServerFixtureforValidateInputFilter _fixture;

        public ApiValidateInputFilterTest(TestServerFixtureforValidateInputFilter fixture)
        {
            _fixture = fixture;
        }

        [Fact(Skip="Need to refactor TestServerFixture to register ValidateInputFilter")]
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
