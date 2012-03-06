using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Thinktecture.Web.Http.Filters;

namespace Thinktecture.Web.Http.Selectors
{
    // Code based on: http://code.msdn.microsoft.com/Implementing-CORS-support-418970ee
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

                    if (actualDescriptor != null)
                    {
                        if (actualDescriptor.GetFilters().OfType<EnableCorsAttribute>().Any())
                        {
                            return new PreflightActionDescriptor(actualDescriptor, accessControlRequestMethod);
                        }
                    }
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

            public override object Execute(HttpControllerContext controllerContext, IDictionary<string, object> arguments)
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Headers.Add(accessControlAllowMethods, prefilghtAccessControlRequestMethod);

                var requestedHeaders = string.Join(
                    ", ",
                    controllerContext.Request.Headers.GetValues(accessControlRequestHeaders));

                if (!string.IsNullOrEmpty(requestedHeaders))
                {
                    response.Headers.Add(accessControlAllowHeaders, requestedHeaders);
                }

                return response;
            }

            public override ReadOnlyCollection<HttpParameterDescriptor> GetParameters()
            {
                return originalAction.GetParameters();
            }

            public override Type ReturnType
            {
                get { return typeof(HttpResponseMessage); }
            }

            public override ReadOnlyCollection<Filter> GetFilterPipeline()
            {
                return originalAction.GetFilterPipeline();
            }

            public override IEnumerable<IFilter> GetFilters()
            {
                return originalAction.GetFilters();
            }

            public override ReadOnlyCollection<T> GetCustomAttributes<T>()
            {
                return originalAction.GetCustomAttributes<T>();
            }
        }
    }
}