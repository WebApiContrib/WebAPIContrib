using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApiContrib.Filters;

namespace WebApiContrib.Selectors
{
    // Code based on: http://code.msdn.microsoft.com/Implementing-CORS-support-418970ee
    // and updated by: http://pastebin.com/tNdS5P5A
    // For a more complete solution, see http://nuget.org/Packages/Thinktecture.IdentityModel.Http
    public class CorsActionSelector : ApiControllerActionSelector
    {
        private const string origin = "Origin";
        private const string accessControlRequestMethod = "Access-Control-Request-Method";
        private const string accessControlRequestHeaders = "Access-Control-Request-Headers";
        private const string accessControlAllowMethods = "Access-Control-Allow-Methods";
        private const string accessControlAllowHeaders = "Access-Control-Allow-Headers";

        public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            var originalRequest = controllerContext.Request;
            var isCorsRequest = originalRequest.Headers.Contains(origin);

            if (originalRequest.Method == HttpMethod.Options && isCorsRequest)
            {
                var currentAccessControlRequestMethod = originalRequest.Headers.GetValues(accessControlRequestMethod).FirstOrDefault();

                if (!string.IsNullOrEmpty(currentAccessControlRequestMethod))
                {
                    var modifiedRequest = new HttpRequestMessage(
                        new HttpMethod(currentAccessControlRequestMethod),
                        originalRequest.RequestUri);
                    controllerContext.Request = modifiedRequest;
                    var actualDescriptor = base.SelectAction(controllerContext);
                    controllerContext.Request = originalRequest;

                    if (actualDescriptor != null && actualDescriptor.GetFilters().OfType<EnableCorsAttribute>().Any())
                        return new PreflightActionDescriptor(actualDescriptor, accessControlRequestMethod);
                }
            }

            return base.SelectAction(controllerContext);
        }

        class PreflightActionDescriptor : HttpActionDescriptor
        {
            private readonly HttpActionDescriptor originalAction;
            private readonly string prefilghtAccessControlRequestMethod;

            public PreflightActionDescriptor(HttpActionDescriptor originalAction, string accessControlRequestMethod)
            {
                this.originalAction = originalAction;
                this.prefilghtAccessControlRequestMethod = accessControlRequestMethod;
            }

            public override string ActionName
            {
                get { return originalAction.ActionName; }
            }

            public override Task<object> ExecuteAsync(HttpControllerContext controllerContext, IDictionary<string, object> arguments)
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Headers.Add(accessControlAllowMethods, prefilghtAccessControlRequestMethod);

                var requestedHeaders = string.Join(", ", controllerContext.Request.Headers.GetValues(accessControlRequestHeaders));

                if (!string.IsNullOrEmpty(requestedHeaders))
                    response.Headers.Add(accessControlAllowHeaders, requestedHeaders);

                var tcs = new TaskCompletionSource<object>();
                tcs.SetResult(response);
                return tcs.Task;
            }

            public override Collection<HttpParameterDescriptor> GetParameters()
            {
                return originalAction.GetParameters();
            }

            public override Type ReturnType
            {
                get { return typeof(HttpResponseMessage); }
            }

            public override Collection<FilterInfo> GetFilterPipeline()
            {
                return originalAction.GetFilterPipeline();
            }

            public override Collection<IFilter> GetFilters()
            {
                return originalAction.GetFilters();
            }

            public override Collection<T> GetCustomAttributes<T>()
            {
                if (typeof(T).IsAssignableFrom(typeof(AllowAnonymousAttribute)))
                    return new Collection<T> { new AllowAnonymousAttribute() as T };

                return originalAction.GetCustomAttributes<T>();
            }

            public override HttpActionBinding ActionBinding
            {
                get { return originalAction.ActionBinding; }
                set { originalAction.ActionBinding = value; }
            }
        }
    }
}
