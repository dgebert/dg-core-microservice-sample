using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using Xunit;

using dg.contract;
using dg.common.validation;
using FluentValidation.Results;

namespace dg.api.test
{
    public class ApiValidationActionAttributeTest : IClassFixture<TestFixtureWithValidationActionAttribute>
    {
        private HttpClient _client;
        private TestFixtureWithValidationActionAttribute _fixture;


        // TODO:  these tests are for validation filters.  Should configure DB in Test fixture to use in-memory db
        public ApiValidationActionAttributeTest(TestFixtureWithValidationActionAttribute fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
        }


        [Fact]
        public async Task GivenApiWithExplicitValidation_WhenInvokedWithInvalidContract_ShouldReturnBadRequest_WithErrors()
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
    }

  


}
