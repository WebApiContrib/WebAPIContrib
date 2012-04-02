using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Thinktecture.Web.Http.Filters
{
    public class ValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext context)
        {
            var modelState = context.ModelState;

            if (!modelState.IsValid)
            {
                dynamic errors = new JsonObject();

                foreach (var key in modelState.Keys)
                {
                    var state = modelState[key];

                    if (state.Errors.Any())
                    {
                        errors[key] = state.Errors.First().ErrorMessage;
                    }
                }

                context.Response = new HttpResponseMessage<JsonValue>(errors, HttpStatusCode.BadRequest);
            }
        }
    }
}