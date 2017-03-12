using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Xunit;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;

using dg.contract;
using dg.validator;
using Microsoft.AspNetCore.Mvc.Filters;

namespace dg.common.validation.unittest
{
    public class ValidationInputAttributeTest
    {
        public ValidationInputAttributeTest()
        {
        }


        [Fact]
        public void ActionFilterTest()
        {
            var p = new Person();
            // Create the validator mock with error results
            var validationFailure = new ValidationFailure("FirstName", "Required", "not a good first name");
            validationFailure.ErrorCode = PersonValidator.ErrorCode.FirstNameRequired.ToString();
            var validationFailureList = new List<ValidationFailure> { validationFailure };
            var validationResult = new ValidationResult(validationFailureList);
            var mockValidator = new MockPersonValidator(validationResult);

            // If provider.GetService(typeof(IValidator<User>)) gets called, IValidator<Person> mock will be returned
            var mockServiceProvider = Substitute.For<IServiceProvider>();
            mockServiceProvider.GetService(typeof(IValidator<Person>)).Returns(mockValidator);

            // Mock the HttpContext 
            var mockHttpContext = Substitute.For<HttpContext>();
            mockHttpContext.RequestServices.Returns(mockServiceProvider);


            var actionArgs = new Dictionary<string, object>();
            actionArgs["person"] = p;

            var mockController = Substitute.For<Controller>();
            var actionExecutingContext = HttpContextUtils.MockedActionExecutingContext(mockHttpContext, actionArgs);

            // Act
            var filter = new ValidateInputAttribute();
            filter.OnActionExecuting(actionExecutingContext);

            // Assert

            // Make sure that IsValid is being called at least once, otherwise this throws an exception. This is a behavior test
            //     mockValidator.Received().Validate<Person>(Arg.Any<Person>());
            var badRequestResult = actionExecutingContext.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Value.Should().Be(StatusCodes.Status400BadRequest);
            var failures = badRequestResult.Value as List<ValidationFailure>;
            failures.ShouldBeEquivalentTo(validationFailureList);
        }



        public class MockPersonValidator : AbstractValidator<Person>
        {
            public ValidationResult Result { get; }
            public MockPersonValidator(ValidationResult result)
            {
                Result = result;
            }

            public override ValidationResult Validate(Person p)
            {
                return Result;
            }
        }

    }
}
