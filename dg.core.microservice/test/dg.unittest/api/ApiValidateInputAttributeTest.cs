using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using Xunit;

using dg.contract;
using FluentValidation.Results;

namespace dg.unittest.api
{
    public class ApiValidateInputAttributeTest : IClassFixture<TestServerFixtureVIA>
    {
        private readonly HttpClient _client;
        private readonly TestServerFixtureVIA _fixture;


        // TODO:  these tests are for validation filters.  Should configure DB in Test fixture to use in-memory db
        public ApiValidateInputAttributeTest(TestServerFixtureVIA fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
        }


        [Fact]
        public async Task GivenApiWithExplicitValidator_WhenInvokedWithInvalidContract_ShouldReturnBadRequest_WithErrors()
        {
            try
            {
                var p = new Person()
                {
                    FirstName = "supercalifragilisticexpialidocious",
                    LastName = "Willis"
                };

                var response = await _client.PostAsync("people2", _fixture.BuildRequestContent(p));

                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

                ValidationResult result = _fixture.GetValidationResult(response);
                result.IsValid.Should().BeFalse();
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }
       

        [Fact]
        public async Task GivenApiWithValidationActionAttribute_WhenInvokedWithInvalidContract_ShouldReturnBadRequest_WithErrors()
        {
            try
            {
                var p = new Person()
                {
                    FirstName = "supercalifragilisticexpialidocious",
                    LastName = "Willis"
                };

                var response = await _client.PostAsync("people", _fixture.BuildRequestContent(p));

                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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
