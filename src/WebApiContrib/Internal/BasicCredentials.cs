using System;

namespace WebApiContrib.Internal
{
    internal struct BasicCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public override string ToString()
        {
            return String.Format("{0}:{1}", Username, Password);
        }
    }
}
