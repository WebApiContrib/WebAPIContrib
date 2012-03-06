using System.Linq;
using System.Web.Http.Filters;

namespace Thinktecture.Web.Http.Filters
{
    // Code based on: http://code.msdn.microsoft.com/Implementing-CORS-support-418970ee
    public class EnableCorsAttribute : ActionFilterAttribute
    {
        private const string origin = "Origin";
        private const string accessControlAllowOrigin = "Access-Control-Allow-Origin";

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Request.Headers.Contains(origin))
            {
                var originHeader = actionExecutedContext.Request.Headers.GetValues(origin).FirstOrDefault();
                
                if (!string.IsNullOrEmpty(originHeader))
                {
                    actionExecutedContext.Result.Headers.Add(accessControlAllowOrigin, originHeader);
                }
            }
        }
    }
}