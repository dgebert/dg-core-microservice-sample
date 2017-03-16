using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Xunit;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;

using dg.common.validation;
using dg.contract;
using dg.validator;
using dg.test.infrastructure;

namespace dg.unittest.common
{
    public class ValidationActionFiltersTest
    {
        
        // Each test can be applied to both filters
        public static IEnumerable<object> ValidationFilters
        {
            get
            {
                return new object[]
                {
                     new object[] { new ValidateInputAttributeImpl(new ActionContextModelValidator()) },
                     new object[] { new ValidateInputFilterImpl(new ActionContextModelValidator()) }
               };
            }
        }
           
        [Theory, MemberData("ValidationFilters")]
        public void GivenNoActionArgumentsInActionContext_WhenOnActionExecuting_ShouldNotValidate(IValidationResult filter)
        {
            // Mock the HttpContext 
            var mockHttpContext = Substitute.For<HttpContext>();
            var actionArgs = new Dictionary<string, object>();
            var actionExecutingContext = HttpUtils.MockedActionExecutingContext(mockHttpContext, actionArgs);
            var actionContextModelValidator = new ActionContextModelValidator();

            // Act
            filter.OnActionExecuting(actionExecutingContext);

            // Assert
            actionExecutingContext.Result.Should().BeNull();
            filter.Result.IsValid.Should().BeTrue();
        }

         [Theory, MemberData("ValidationFilters")]
        public void GivenNoModelInActionContext_WhenOnActionExecuting_ShouldNotValidate(IValidationResult filter)
        {
            int argValue = 99;

            var mockValidator = new MockPersonValidator(new ValidationResult());
            var mockServiceProvider = Substitute.For<IServiceProvider>();
            mockServiceProvider.GetService(typeof(IValidator<Person>)).Returns(mockValidator);  

            // Mock the HttpContext 
            var mockHttpContext = Substitute.For<HttpContext>();
            mockHttpContext.RequestServices.Returns(mockServiceProvider);


            var actionArgs = new Dictionary<string, object>();
            actionArgs["notPerson"] = argValue;  // Validator should not be resolved

            var mockController = Substitute.For<Controller>();
            var actionExecutingContext = HttpUtils.MockedActionExecutingContext(mockHttpContext, actionArgs);

            // Act
            filter.OnActionExecuting(actionExecutingContext);

            // Assert
            actionExecutingContext.Result.Should().BeNull();
            filter.Result.IsValid.Should().BeTrue();
        }

         [Theory, MemberData("ValidationFilters")]
        public void GivenValidatorNotFound_WhenOnActionExecuting_ShouldNotValidate(IValidationResult filter)
        {
            // Mock the all the pieces for ActionExecutingContext 
             var p = new Person();
            var mockServiceProvider = Substitute.For<IServiceProvider>();
            mockServiceProvider.GetService(typeof(IValidator<Person>)).Returns(null);

            var mockHttpContext = Substitute.For<HttpContext>();
            mockHttpContext.RequestServices.Returns(mockServiceProvider);

            var actionArgs = new Dictionary<string, object>();
            actionArgs["person"] = p;
            var actionExecutingContext = HttpUtils.MockedActionExecutingContext(mockHttpContext, actionArgs);

            // Act
            filter.OnActionExecuting(actionExecutingContext);

            // Assert
            actionExecutingContext.Result.Should().BeNull();
            filter.Result.IsValid.Should().BeTrue();
        }

         [Theory, MemberData("ValidationFilters")]
        public void GivenValidatorFound_AndValidationSucceeds_WhenOnActionExecuting_ShouldReturnValidationResultIsValid(IValidationResult filter)
        {
            var p = new Person();
            // Create the validator mock with success result
            var validationResult = new ValidationResult();
            var mockValidator = new MockPersonValidator(validationResult);

            // If provider.GetService(typeof(IValidator<User>)) gets called, IValidator<Person> mock will be returned
            var mockServiceProvider = Substitute.For<IServiceProvider>();
            mockServiceProvider.GetService(typeof(IValidator<Person>)).Returns(mockValidator);
            mockServiceProvider.GetService(typeof(ActionContextModelValidator)).Returns(new ActionContextModelValidator());

            // Mock the HttpContext 
            var mockHttpContext = Substitute.For<HttpContext>();
            mockHttpContext.RequestServices.Returns(mockServiceProvider);

            var actionArgs = new Dictionary<string, object>();
            actionArgs["person"] = p;
            var actionExecutingContext = HttpUtils.MockedActionExecutingContext(mockHttpContext, actionArgs);
            var actionContextModelValidator = new ActionContextModelValidator();


            // Act
            filter.OnActionExecuting(actionExecutingContext);

            // Assert
            var actionResult = actionExecutingContext.Result;
            actionResult.Should().BeNull();
            filter.Result.IsValid.Should().BeTrue();
        }

         [Theory, MemberData("ValidationFilters")]
        public void GivenValidatorFound_AndValidationFailure_WhenOnActionExecuting_ShouldHaveBadRequestResponse_WithFailure(IValidationResult filter)
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
            mockServiceProvider.GetService(typeof(ActionContextModelValidator)).Returns(new ActionContextModelValidator());

            // Mock the HttpContext 
            var mockHttpContext = Substitute.For<HttpContext>();
            mockHttpContext.RequestServices.Returns(mockServiceProvider);

            var actionArgs = new Dictionary<string, object>();
            actionArgs["person"] = p;
            var actionExecutingContext = HttpUtils.MockedActionExecutingContext(mockHttpContext, actionArgs);
            var actionContextModelValidator = new ActionContextModelValidator();


            // Act
            filter.OnActionExecuting(actionExecutingContext);

            // Assert
            var actionResult = actionExecutingContext.Result;
            actionResult.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = actionExecutingContext.Result as BadRequestObjectResult;
            badRequestResult.StatusCode.Value.Should().Be(StatusCodes.Status400BadRequest);
            var result = badRequestResult.Value as ValidationResult;
            result.ShouldBeEquivalentTo(validationResult);
            filter.Result.ShouldBeEquivalentTo(validationResult);
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
