using System;
using System.ServiceModel.Channels;
using System.Web.Http.SelfHost;
using System.Web.Http.SelfHost.Channels;

namespace Thinktecture.Web.Http.SelfHost
{
    public class SslHttpSelfHostConfiguration : HttpSelfHostConfiguration
    {
        public SslHttpSelfHostConfiguration(string baseAddress) : base(baseAddress) { }

        public SslHttpSelfHostConfiguration(Uri baseAddress) : base(baseAddress) { }

        protected override BindingParameterCollection OnConfigureBinding(HttpBinding httpBinding)
        {
            httpBinding.Security.Mode = HttpBindingSecurityMode.Transport;

            return base.OnConfigureBinding(httpBinding);
        }        
    }
}
