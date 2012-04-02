using System.Collections.Generic;

namespace WebApiContrib.MessageHandlers
{
    public class UriExtensionMappings : List<UriExtensionMapping>
    {
        public UriExtensionMappings()
        {
            this.AddMapping("xml", "application/xml");
            this.AddMapping("json", "application/json");
            this.AddMapping("proto", "application/x-protobuf");
            this.AddMapping("png", "image/png");
            this.AddMapping("jpg", "image/jpg");            
        }
    }
}