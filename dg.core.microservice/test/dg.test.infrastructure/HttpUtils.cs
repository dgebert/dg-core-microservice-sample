
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using NSubstitute;
using System.Collections.Generic;
using System.Net.Http;

namespace dg.unittest
{
    public static class HttpUtils
    {

        public static ActionExecutingContext MockedActionExecutingContext(HttpContext context, object controller = null)
        {
            return MockedActionExecutingContext(context, new List<IFilterMetadata>(), new Dictionary<string, object>(), controller);
        }

        public static ActionExecutingContext MockedActionExecutingContext(HttpContext context,
                                                                          IDictionary<string, object> actionArguments,
                                                                          object controller = null)
        {
            return MockedActionExecutingContext(context, new List<IFilterMetadata>(), actionArguments, controller);
        }

        public static ActionExecutingContext MockedActionExecutingContext(
                                                                            HttpContext context,
                                                                            IList<IFilterMetadata> filters,
                                                                            IDictionary<string, object> actionArguments,
                                                                            object controller = null)
        {
            var actionContext = new ActionContext()
            {
                HttpContext = context,
                RouteData = new RouteData(),
                ActionDescriptor = new ControllerActionDescriptor()
            };

            //          return Substitute.For<ActionExecutingContext>(actionContext, filters, actionArguments, controller);
            return new ActionExecutingContext(actionContext, filters, actionArguments, controller);
        }

     
        public static ValidationResult GetValidationResult(this HttpResponseMessage response)
        {
            var json = response.Content.ReadAsStringAsync().Result;
            var errorResponse = JsonConvert.DeserializeObject<ValidationResult>(json);
            return errorResponse;
        }
    }
}

