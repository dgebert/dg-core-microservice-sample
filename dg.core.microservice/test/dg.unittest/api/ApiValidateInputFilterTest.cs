using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.Results;
using Xunit;
using dg.contract;
using dg.validator;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using dg.common.validation;

namespace dg.unittest.api
{
    public class ApiValidateInputFilterTest : IClassFixture<TestServerfixtureForValdateInputFilter>
    {
        private TestServerfixtureForValdateInputFilter _fixture;

        public ApiValidateInputFilterTest(TestServerfixtureForValdateInputFilter fixture)
        {
            _fixture = fixture;
            _fixture.ConfigureMvcForValidateInputFilter<PersonValidator>();
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

                var response = await _fixture.Client.PostAsync("people2", _fixture.BuildRequestContent(p));

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
            services.AddValidateInputFilter<PersonValidator>();
        }
    }


}
