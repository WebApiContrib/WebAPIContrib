using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace Thinktecture.Web.Http
{
    public class ReadAsSingleObjectPolicy : IRequestContentReadPolicy
    {
        public RequestContentReadKind GetRequestContentReadKind(HttpActionDescriptor actionDescriptor)
        {
            return RequestContentReadKind.AsSingleObject;
        }
    }
}
