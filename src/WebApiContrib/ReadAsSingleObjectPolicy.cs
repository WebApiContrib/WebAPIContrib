using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace WebApiContrib
{
    public class ReadAsSingleObjectPolicy : IRequestContentReadPolicy
    {
        public RequestContentReadKind GetRequestContentReadKind(HttpActionDescriptor actionDescriptor)
        {
            return RequestContentReadKind.AsSingleObject;
        }
    }
}
