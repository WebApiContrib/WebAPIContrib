using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApiContrib.Messages;

namespace WebApiContrib.Filters
{
    public class ValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                var errors = actionContext.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .Select(e => new Error 
                    {
                        Name = e.Key,
                        Message = e.Value.Errors.First().ErrorMessage
                    }).ToArray();

            	actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, errors);
            } 
        }
    }
}
