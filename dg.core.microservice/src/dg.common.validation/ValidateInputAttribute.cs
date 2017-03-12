
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace dg.common.validation
{
    public class ValidateInputAttribute : TypeFilterAttribute
    {
        public ValidateInputAttribute() : base(typeof(ValidateInputAttributeImpl))
        {
        }
    }

    public class ValidateInputAttributeImpl : ActionFilterAttribute
    {
        private IActionContextModelValidator _actionContextModelValidator;
        public ValidationResult Result { get; set; } = null;

        public ValidateInputAttributeImpl(IActionContextModelValidator actionContextModelValidator = null)
        {
            _actionContextModelValidator = actionContextModelValidator;
        }
        
        /// <summary>
        /// Validate the model before control passes to the controller's action
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Result = _actionContextModelValidator.TryValidateModel(context);
            if (!Result.IsValid)
            {
                context.Result = new BadRequestObjectResult(Result);
            }
        }
    }
}
