using WebApiContrib.Messages;

namespace WebApiContrib.Data
{
    // Code based on: http://weblogs.asp.net/pglavich/archive/2012/02/26/asp-net-web-api-request-response-usage-logging.aspx
	public interface ILoggingRepository
	{
		void Log(ApiLoggingInfo loggingInfo);
	}
}
