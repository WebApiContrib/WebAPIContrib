using System.Diagnostics;
using Thinktecture.Web.Http.Data;
using Thinktecture.Web.Http.Messages;

namespace Thinktecture.Web.Http.Testing
{
	public class DummyLoggingRepository : ILoggingRepository
	{
		public int LogMessageCount { get; set; }

		public bool HasRequestMessageTypeBeenReceived { get; set; }
		public bool HasResponseMessageTypeBeenReceived { get; set; }

		public void Log(Messages.ApiLoggingInfo loggingInfo)
		{
			LogMessageCount++;

			if (loggingInfo.MessageType == HttpMessageType.Response)
			{
				HasResponseMessageTypeBeenReceived = true;
			}
			else
			{
				HasRequestMessageTypeBeenReceived = true;
			}

			Trace.WriteLine(string.Format("Message: Type:{0}, Uri:{1}, Method:{2}", loggingInfo.MessageType, loggingInfo.UriAccessed, loggingInfo.HttpMethod));
		}
	}
}
