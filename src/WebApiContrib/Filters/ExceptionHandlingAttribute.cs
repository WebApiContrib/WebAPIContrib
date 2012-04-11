using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Filters;
using System.Net;
using System.Web;
using System.Net.Http;
using WebApiContrib.Messages;

namespace WebApiContrib.Filters
{
    /// <summary>
    /// This filter maps exceptions to well-know status codes.
    /// </summary>
    /// <example>
    /// var exceptionHandler = new ExceptionHandlerFilter();   
    /// exceptionHandler.Mappings.Add(typeof(NotAuthorizedException), HttpStatusCode.Forbidden);   
    /// GlobalConfiguration.Configuration.Filters.Add(exceptionHandler);
    /// </example>
    public class ExceptionHandlingAttribute : ExceptionFilterAttribute 
    {
        public ExceptionHandlingAttribute() 
        { 
            this.Mappings = new Dictionary<Type, HttpStatusCode>(); 
            
            this.Mappings.Add(typeof(ArgumentNullException), HttpStatusCode.BadRequest); 
            this.Mappings.Add(typeof(ArgumentException), HttpStatusCode.BadRequest); 
        } 
        
        public IDictionary<Type, HttpStatusCode> Mappings 
        { 
            get; 
            private set; 
        } 
        
        public override void OnException(HttpActionExecutedContext actionExecutedContext) 
        { 
            if (actionExecutedContext.Exception != null) 
            { 
                var exception = actionExecutedContext.Exception; 
                
                if (actionExecutedContext.Exception is HttpException) 
                { 
                    var httpException = (HttpException)exception; 
                    
                    actionExecutedContext.Result = new HttpResponseMessage<Error>(new Error { Message = exception.Message }, 
                        (HttpStatusCode)httpException.GetHttpCode()); 
                } 
                else if (this.Mappings.ContainsKey(exception.GetType())) 
                { 
                    var httpStatusCode = this.Mappings[exception.GetType()]; 
                    actionExecutedContext.Result = new HttpResponseMessage<Error>(new Error 
                        { 
                            Message = exception.Message 
                        }, httpStatusCode); 
                } 
                else 
                { 
                    actionExecutedContext.Result = new HttpResponseMessage<Error>(new Error 
                        { 
                            Message = exception.Message 
                        }, 
                        HttpStatusCode.InternalServerError); 
                } 
            } 
        } 
    }
}
