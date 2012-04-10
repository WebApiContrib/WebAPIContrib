using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http.Formatting;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace WebApiContrib.Formatting {

    public class RouteDataMapping : MediaTypeMapping
    {
        private readonly string _routeDataValueName;
        private readonly string _routeDataValueValue;

        public RouteDataMapping(string routeDataValueName, string routeDataValueValue, MediaTypeHeaderValue mediaType) 
            : base(mediaType) 
        {
            _routeDataValueName = routeDataValueName;
            _routeDataValueValue = routeDataValueValue;
        }

        public RouteDataMapping(string routeDataValueName, string routeDataValueValue, string mediaType) 
            : base(mediaType) 
        {
            _routeDataValueName = routeDataValueName;
            _routeDataValueValue = routeDataValueValue;
        }

        protected override double OnTryMatchMediaType(HttpResponseMessage response) 
        {
            return (
                response.RequestMessage.GetRouteData().Values[_routeDataValueName].ToString() == _routeDataValueValue
            ) ? 1.0 : 0.0;
        }

        //Don't use this
        //This will be removed on the first drop
        protected override double OnTryMatchMediaType(HttpRequestMessage request) 
        {
            throw new NotImplementedException();
        }
    }
}