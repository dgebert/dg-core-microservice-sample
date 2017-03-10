using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using dg.contract;
using dg.common.validation;

namespace dg.api.test
{
    public class ApiValidationActionFilterTest : IClassFixture<TestFixture2>
    {
        private TestFixture2 _fixture;

        public ApiValidationActionFilterTest(TestFixture2 fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ValidationFilter_Test()
        {
            try
            {
                var p = new Person() { FirstName = "Name is too long" };

                var response = await _fixture.Client.PostAsync("people4", _fixture.BuildRequestContent(p));

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
