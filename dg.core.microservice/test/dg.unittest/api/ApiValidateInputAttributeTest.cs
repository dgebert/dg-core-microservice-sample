using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using FluentValidation.Results;
using Xunit;

using dg.contract;
using dg.validator;

namespace dg.unittest.api
{
    public class ApiValidateInputAttributeTest : IClassFixture<TestServerFixture>
    {
        private readonly HttpClient _client;
        private readonly TestServerFixture _fixture;


        // TODO:  these tests are for validation filters.  Should configure DB in Test fixture to use in-memory db
        public ApiValidateInputAttributeTest(TestServerFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
        }
   

        [Fact]
        public async Task WhenInvokedWithInvalidContract_ShouldReturnBadRequest_WithErrors()
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
