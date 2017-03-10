
using Microsoft.AspNetCore.Mvc.Filters;

namespace dg.common.validation
{

    public class ValidateInputAttribute : ActionFilterAttribute 
    {
        /// <summary>
        /// Validate the model before control passes to the controller's action
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Validation.TryValidateModel(context);
        }
    }
}
