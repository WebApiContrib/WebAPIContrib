using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Http;

namespace WebApiContrib.ResponseMessages
{
    public interface IApiResource
    {
        void SetLocation(ResourceLocation location);
    }

    public class ResourceLocation
    {
        public Uri Location { get; private set; }

        public void SetInController<T>(Expression<Func<T,object>> location) where T : ApiController
        {
            var context = new HttpContextWrapper(HttpContext.Current);
        }

        public void SetInController<T>(Expression<Func<T,object>> controllerMethod, object arg) where T : ApiController
        {
            
        }

        public void Set(Uri location)
        {
            Location = location;
        }
    }
}

