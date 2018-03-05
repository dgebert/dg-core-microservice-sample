using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using FluentValidation.Results;
using FluentAssertions;
using Xunit;

using gwn.contract;
using gwn.validation;
using gwn.common.test;


namespace gwn.unittest.api
{
    public class ApiValidateInputFilterTest : IClassFixture<TestServerfixtureForValdateInputFilter>
    {
        private TestServerfixtureForValdateInputFilter _fixture;

        public ApiValidateInputFilterTest(TestServerfixtureForValdateInputFilter fixture)
        {
            _fixture = fixture;
            //     _fixture.ConfigureMvcForValidateInputFilter<PersonValidator>();
        }
        public static IEnumerable<object[]> HttpActions =>  new List<object[]>
        {
            new object[] { "POST" } ,
            new object[] { "PUT"  }
        };

        [Theory]
        [MemberData(nameof(HttpActions))]
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

                var response = await _fixture.Client.PostOrPutAsync("people2", p, httpAction);

                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var x = ex.ToString();
                throw;
            }
        }

        [Theory]
        [MemberData(nameof(HttpActions))]
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

                var response = await _fixture.Client.PostOrPutAsync("people2", p, httpAction);

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

        [Theory]
        [MemberData(nameof(HttpActions))]
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

                var response = await _fixture.Client.PostOrPutAsync("people2", p, httpAction);

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

    public class TestServerfixtureForValdateInputFilter : TestServerFixture
    {
        public override void ConfigureValidation(IServiceCollection services)
        {
            ConfigureMvcForValidateInputFilter<PersonValidator>();
        }
    }
}
