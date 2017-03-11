﻿using System;
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
    public class ApiValidationActionAttributeTest : IClassFixture<TestFixtureWithValidationActionAttribute>
    {
        private HttpClient _client;
        private TestFixtureWithValidationActionAttribute _fixture;

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
