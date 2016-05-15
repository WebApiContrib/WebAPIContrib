using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;

using NUnit.Framework;

using Rhino.Mocks;

using WebApiContrib.Http;

using Microsoft.Owin;

namespace WebApiContribTests.Http
{
    [TestFixture]
    public class HttpRequestMessageExtensionsTests : TestBase
    {
        [Test]
        public void TestGetClientIpAddressFromHttpContext()
        {
            var httpContextBaseStub = MockRepository.GenerateMock<HttpContextBase>();
            var httpRequestBaseStub = MockRepository.GenerateMock<HttpRequestBase>();
            httpRequestBaseStub.Stub(x => x.UserHostAddress).Return("127.0.0.1").Repeat.Once();
            httpContextBaseStub.Stub(x => x.Request).Return(httpRequestBaseStub).Repeat.Once();
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Properties.Add("MS_HttpContext", httpContextBaseStub);

            var result = httpRequestMessage.GetClientIpAddress();

            Assert.That(result, Is.EqualTo("127.0.0.1"));

            httpContextBaseStub.VerifyAllExpectations();
            httpRequestBaseStub.VerifyAllExpectations();
        }

        [Test]
        public void TestGetClientIpAddressFromRemoteEndpointMessage()
        {
            var remoteEndpointMessageProperty = new RemoteEndpointMessageProperty("127.0.0.1", 8050);
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Properties.Add(
                "System.ServiceModel.Channels.RemoteEndpointMessageProperty",
                remoteEndpointMessageProperty);

            var result = httpRequestMessage.GetClientIpAddress();

            Assert.That(result, Is.EqualTo("127.0.0.1"));
        }

        [Test]
        public void TestGetClientIpAddressFromOwinContext()
        {
            var owinContextStub = MockRepository.GenerateMock<OwinContext>();
            var owinRequest = new OwinRequest { RemoteIpAddress = "127.0.0.1" };
            owinContextStub.Stub(x => x.Request).Return(owinRequest).Repeat.Once();
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Properties.Add("MS_OwinContext", owinContextStub);

            var result = httpRequestMessage.GetClientIpAddress();

            Assert.That(result, Is.EqualTo("127.0.0.1"));
        }
    }
}