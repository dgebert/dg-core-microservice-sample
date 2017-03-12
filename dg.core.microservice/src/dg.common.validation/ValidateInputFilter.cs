using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace dg.common.validation
{
    public class ValidateInputFilter : TypeFilterAttribute
    {
        public ValidateInputFilter() : base(typeof(ValidateInputFilterImpl))
        {
        }

        public class ValidateInputFilterImpl : IActionFilter
        {
            private IActionContextModelValidator _actionContextModelValidator;
            public ValidationResult Result { get; set; } = null;

            public ValidateInputFilterImpl(IActionContextModelValidator actionContextModelValidator)
            {
                _actionContextModelValidator = actionContextModelValidator;
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                Result = _actionContextModelValidator.TryValidateModel(context);
                if (!Result.IsValid)
                {
                    context.Result = new BadRequestObjectResult(Result);
                }
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
            }
        }

    }
}
