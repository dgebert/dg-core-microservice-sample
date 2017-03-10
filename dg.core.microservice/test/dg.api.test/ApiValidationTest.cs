using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using Xunit;

using dg.contract;
using dg.common.validation;

namespace dg.api.test
{
    public class ApiValidationTest : IClassFixture<TestFixture>
    {
        private HttpClient _client;
        private TestFixture _fixture;

        public ApiValidationTest(TestFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
        }


        [Fact]
        public async Task Explicit_Validation_Test()
        {
            try
            {
                var p = new Person() { FirstName = "Name is too long" };

                var response = await _client.PostAsync("people1", _fixture.BuildRequestContent(p));

                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

                List<ValidationError>  errors = _fixture.GetValidationErrors(response);
                errors.Should().NotBeEmpty();
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }
        [Fact(Skip = "Web API Method contract decoration not working")]
        public async Task ValidationAttribute_On_Method_Contract_Test()
        {
            try
            {
                var p = new Person() { FirstName = "Name is too long" };

                var response = await _client.PostAsync("people2", _fixture.BuildRequestContent(p));

                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

                List<ValidationError> errors = _fixture.GetValidationErrors(response);
                errors.Should().NotBeEmpty();
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        [Fact]
        public async Task ValidationAttribute_On_Method_Test()
        {
            try
            {
                var p = new Person() { FirstName = "Name is too long" };

                var response = await _client.PostAsync("people3", _fixture.BuildRequestContent(p));

                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

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
