
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

        public ValidateInputAttributeImpl(IActionContextModelValidator actionContextModelValidator = null)
        {
            _actionContextModelValidator = actionContextModelValidator ?? new ActionContextModelValidator();  // TODO: new is GLUE :(
        }
        
        /// <summary>
        /// Validate the model before control passes to the controller's action
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            var validationResult = _actionContextModelValidator.TryValidateModel(context);
            if (!validationResult.IsValid)
            {
                context.Result = new BadRequestObjectResult(validationResult);
            }
        }
    }
}
