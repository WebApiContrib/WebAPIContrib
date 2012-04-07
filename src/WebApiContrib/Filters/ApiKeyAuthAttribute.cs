﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Common;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebApiContrib.Filters {

    /// <summary>
    /// QueryString Api Key Authorization filter for ASP.NET Web API.
    /// </summary>
    public class ApiKeyAuthAttribute : AuthorizationFilterAttribute 
    {
        private static readonly string[] _emptyArray = new string[0];
        private const string _apiKeyAuthorizerMethodName = "IsAuthorized";

        private readonly string _apiKeyQueryParameter;
        private string _roles;
        private readonly Type _apiKeyAuthorizerType;
        private string[] _rolesSplit = _emptyArray;

        /// <summary>
        /// The comma seperated list of roles which user needs to be in.
        /// </summary>
        public string Roles 
        {
            get {

                return this._roles ?? string.Empty;
            }
            set {

                this._roles = value;
                this._rolesSplit = splitString(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKeyQueryParameter">The name of the query string parameter whose value needs to be compared against.</param>
        /// <param name="apiKeyAuthorizerType">Type of Api Key Authorizer which implements TugberkUg.Web.Http.IApiKeyAuthorizer</param>
        public ApiKeyAuthAttribute(string apiKeyQueryParameter, Type apiKeyAuthorizerType) 
        {
            if (string.IsNullOrEmpty(apiKeyQueryParameter))
                throw Error.ArgumentNull("apiKeyQueryParameter");

            if (apiKeyAuthorizerType == null)
                throw Error.ArgumentNull("apiKeyAuthorizerType");

            if (!isTypeOfIApiKeyAuthorizer(apiKeyAuthorizerType)) 
                throw Error.Argument(
                    string.Format(
                        "{0} type has not implemented the TugberkUg.Web.Http.IApiKeyAuthorizer interface",
                        apiKeyAuthorizerType.ToString()
                    ),
                    "apiKeyAuthorizerType"
                );

            _apiKeyQueryParameter = apiKeyQueryParameter;
            _apiKeyAuthorizerType = apiKeyAuthorizerType;
        }

        public override void OnAuthorization(HttpActionContext actionContext) 
        {
            if (actionContext == null)
                throw Error.ArgumentNull("actionContext");

            if (this.skipAuthorization(actionContext))
                return;

            if (!authorizeCore(actionContext.Request))
                HandleUnauthorizedRequest(actionContext);
        }

        /// <summary>
        /// Handles the operation on an unauthorized situation
        /// </summary>
        /// <param name="actionContext"></param>
        protected virtual void HandleUnauthorizedRequest(HttpActionContext actionContext) 
        {

            if (actionContext == null)
                throw Error.ArgumentNull("actionContext");
            
            actionContext.Response = actionContext.ControllerContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
        }

        //private utils

        private bool isTypeOfIApiKeyAuthorizer(Type type) 
        {
            foreach (Type interfaceType in type.GetInterfaces())
                if (interfaceType == typeof(IApiKeyAuthorizer))
                    return true;

            return false;
        }

        private bool skipAuthorization(HttpActionContext actionContext) 
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any<AllowAnonymousAttribute>() ||
                actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any<AllowAnonymousAttribute>();
        }

        private bool authorizeCore(HttpRequestMessage request) 
        {
            var apiKey = HttpUtility.ParseQueryString(request.RequestUri.Query)[_apiKeyQueryParameter];

            return isAuthorized(apiKey);
        }

        private bool isAuthorized(string apiKey)
        {
            object apiKeyAuthorizerClassInstance = Activator.CreateInstance(_apiKeyAuthorizerType);
            object result = null;

            if (_rolesSplit == _emptyArray) 
            {
                result = _apiKeyAuthorizerType.GetMethod(_apiKeyAuthorizerMethodName, new Type[] { typeof(string) }).
                    Invoke(apiKeyAuthorizerClassInstance, new object[] { apiKey });

            }
            else 
            {
                result = _apiKeyAuthorizerType.GetMethod(_apiKeyAuthorizerMethodName, new Type[] { typeof(string), typeof(string[]) }).
                    Invoke(apiKeyAuthorizerClassInstance, new object[] { apiKey, _rolesSplit });
            }

            return (bool)result;
        }

        private static string[] splitString(string original) {

            if (string.IsNullOrEmpty(original))
                return _emptyArray;

            IEnumerable<string> source =
                from piece in original.Split(new char[] { ',' })
                let trimmed = piece.Trim()
                where !string.IsNullOrEmpty(trimmed)
                select trimmed;
            return source.ToArray<string>();
        }
    }
}