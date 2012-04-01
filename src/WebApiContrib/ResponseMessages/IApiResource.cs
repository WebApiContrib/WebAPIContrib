using System;

namespace WebApiContrib.ResponseMessages
{
    public interface IApiResource
    {
        void SetLocation(ResourceLocation location);
    }

    public class ResourceLocation
    {
        public Uri Location { get; private set; }

        public void Set(Uri location)
        {
            Location = location;
        }
        
    }
}

