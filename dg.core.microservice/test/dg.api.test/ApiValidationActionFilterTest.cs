using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using dg.contract;
using dg.common.validation;

namespace dg.api.test
{
    public class ApiValidationActionFilterTest : IClassFixture<TextFixtureWithValidationActionFilter>
    {
        private TextFixtureWithValidationActionFilter _fixture;

        public ApiValidationActionFilterTest(TextFixtureWithValidationActionFilter fixture)
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

                List<ValidationError> errors = _fixture.GetValidationErrors(response);
                errors.Should().NotBeEmpty();
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

    }

}
