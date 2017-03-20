using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using FluentAssertions;
using FluentValidation.Results;
using Xunit;

using dg.contract;
using dg.validator;
using dg.test.infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace dg.unittest.api
{
    public class ApiValidateInputAttributeTest : IClassFixture<TestServerFixtureForValdateAttributeFilter>
    {
        private readonly HttpClient _client;
        private readonly TestServerFixtureForValdateAttributeFilter _fixture;


        // TODO:  these tests are for validation filters.  Should configure DB in Test fixture to use in-memory db
        public ApiValidateInputAttributeTest(TestServerFixtureForValdateAttributeFilter fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
        }

        public static IEnumerable<object> HttpActions
        {
            get
            {
                return new object[]
                {
                     new object[] { "POST" } ,
                     new object[] { "PUT"  }
               };
            }
        }

        [Theory, MemberData("HttpActions")]
        public async Task WhenPostOrPut_WithValidPerson_ShouldReturnOk(string httpAction)
        {
            try
            {
                var p = new Person()
                {
                    Id = 999,
                    FirstName = "Robert",
                    LastName = "Willis",
                    Email = "bw@aol.com",
                    PhoneNumber = "444-222-3434",
                    BirthDate = new DateTime(1965, 10, 10),
                };

                var response = await _client.PostOrPutAsync("people", p, httpAction);
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }


        [Theory, MemberData("HttpActions")]
        public async Task WhenPostOrPut_WithInvalidFirstName_ShouldReturnBadRequest_WithErrors(string httpAction)
        {
            try
            {
                var p = new Person()
                {
                    FirstName = "supercalifragilisticexpialidocious",
                    LastName = "Willis",
                    Email = "bw@aol.com",
                    PhoneNumber = "444-222-3434",
                    BirthDate = new DateTime(1965, 10, 10),
                };

                var response = await _client.PostOrPutAsync("people", p, httpAction);

                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
                ValidationResult result = _fixture.GetValidationResult(response);
                result.IsValid.Should().BeFalse();
                result.Errors.First().ErrorCode.Should().Be(PersonValidator.ErrorCode.FirstNameInvalidLength.ToString());
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        [Theory, MemberData("HttpActions")]
        public async Task WhenPostOrPut_WithNonUnique_ShouldReturnBadRequest_WithErrors(string httpAction)
        {
            try
            {
                var p = new Person()
                {
                    Id = 999,
                    FirstName = "supercalifragilisticexpialidocious",
                    LastName = "Willis",
                    Email = "somebody1@aol.com",   // this is a dup email, see ConfigurePeopleService()
                    PhoneNumber = "444-222-3434",
                    BirthDate = new DateTime(1965, 10, 10),
                };

                var response = await _client.PostOrPutAsync("people", p, httpAction);

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


    public class TestServerFixtureForValdateAttributeFilter : TestServerFixture
    {
        public override void ConfigureValidation(IServiceCollection services)
        {
            ConfigureMvcForValidateInputAttribute<PersonValidator>();
        }

       
    }
}
