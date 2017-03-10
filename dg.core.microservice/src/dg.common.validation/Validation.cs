using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using FluentValidation;
using FluentValidation.Results;

namespace dg.common.validation
{
    public static class Validation
    {

        public static void TryValidateModel(ActionExecutingContext context) 
        {
            var actionArgList = context.ActionArguments;
            if (actionArgList == null)
            {
                return;
            }

            var errors = new List<ValidationFailure>();
            foreach (var arg in actionArgList)
            {
                TryValidateModel(context, arg.Value, errors);
            }

            if (!errors.Any())
            {
                return;
            }


            context.Result = new BadRequestObjectResult(errors);
        }
        
        public static void TryValidateModel(ActionExecutingContext context, object model, List<ValidationFailure> errors)
        {
            if (model == null)
            {
                return;
            }

            // Lookup validator
            var validator = GetValidatorForModel(context, model);
           if (validator == null)
            {
                return;
            }

            // validate
            var validationResult = validator.Validate(model);
            if (validationResult.IsValid)
            {
                return;
            }

            var modelErrors = validationResult.Errors;
            if (modelErrors.Any())
            {
                errors.AddRange(modelErrors);
            }
        }


        public static IValidator GetValidatorForModel(ActionExecutingContext context, object model)
        {
            var abstractValidatorType = typeof(IValidator<>);
            var validatorForType = abstractValidatorType.MakeGenericType(model.GetType());
            var validator = (IValidator)context.HttpContext.RequestServices.GetService(validatorForType);

            return validator;
        }

    }
}
