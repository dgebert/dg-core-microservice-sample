using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;

using FluentValidation;
using FluentValidation.Results;

namespace dg.common.validation
{

    public interface IActionContextModelValidator
    {
        ValidationResult TryValidateModel(ActionExecutingContext context);
        ValidationResult TryValidateModel(ActionExecutingContext context, object model);
    }

    // This is used by both IActionFilter and ActionFilterAttribute types to locate
    // validator for model argument, execute validation logic, and set result if validation fails
    public class ActionContextModelValidator : IActionContextModelValidator
    {
        public ValidationResult TryValidateModel(ActionExecutingContext context) 
        {
            var errors = new List<ValidationFailure>();
            var actionArgList = context.ActionArguments;
            if (actionArgList != null)
            {
                foreach (var arg in actionArgList)
                {
                    var validationResult = TryValidateModel(context, arg.Value);
                    if (validationResult != null)
                    {
                        errors.AddRange(validationResult.Errors);
                    }
                }
            }

            var result = new ValidationResult(errors);
             return result;
        }
        
        public ValidationResult TryValidateModel(ActionExecutingContext context, object model)
        {
            if (model == null)
            {
                return null;
            }

            // Lookup validator
            var validator = GetValidatorForModel(context, model);
            if (validator == null)
            {
                return null;
            }

            // validate
            var validationResult = validator.Validate(model);
            return validationResult;
        }


        public IValidator GetValidatorForModel(ActionExecutingContext context, object model)
        {
            var abstractValidatorType = typeof(IValidator<>);
            var validatorForType = abstractValidatorType.MakeGenericType(model.GetType());
            var validator = (IValidator)context.HttpContext.RequestServices.GetService(validatorForType);
            return validator;
        }

    }
}
