using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Newtonsoft.Json.Linq;

namespace WebApiContrib.Filters
{
    public class ValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext context)
        {
            var modelState = context.ModelState;

            if (!modelState.IsValid)
            {
                dynamic errors = new JObject();

                foreach (var key in modelState.Keys)
                {
                    var state = modelState[key];

                    if (state.Errors.Any())
                    {
                    	errors[key] = state.Errors.First().ErrorMessage;
                    }
                }

            	var contentNegotiator = (IContentNegotiator) context.ControllerContext.Configuration.Services.GetService(typeof (IContentNegotiator));
            	var result = contentNegotiator.Negotiate(typeof (object), context.Request, context.ControllerContext.Configuration.Formatters);
            	context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            	{
            		Content = new ObjectContent(typeof (JValue), errors, result.Formatter, result.MediaType.MediaType)
            	};
            }
        }
    }
}
