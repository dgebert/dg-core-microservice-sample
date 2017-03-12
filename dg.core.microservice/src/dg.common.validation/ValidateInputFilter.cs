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

            public ValidateInputFilterImpl(IActionContextModelValidator actionContextModelValidator)
            {
                _actionContextModelValidator = actionContextModelValidator ?? new ActionContextModelValidator(); // TODO: new is GLUE :(
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                var validationResult = _actionContextModelValidator.TryValidateModel(context);
                if (!validationResult.IsValid)
                {
                    context.Result = new BadRequestObjectResult(validationResult);
                }
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
            }
        }

    }
}
