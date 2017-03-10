
using Microsoft.AspNetCore.Mvc.Filters;



namespace dg.common.validation
{
    public class ValidateInputFilter : IActionFilter 
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {

            Validation.TryValidateModel(context);
        }


        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }

}
