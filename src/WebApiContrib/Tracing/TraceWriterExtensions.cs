using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Tracing;

namespace WebApiContrib.Tracing
{
	public static class TraceWriter
	{
		public static void Debug(this ITraceWriter tracer, HttpRequestMessage request, ApiController controller, Exception exception)
		{
			tracer.Debug(request, controller.ControllerContext.ControllerDescriptor.ControllerType.FullName, exception);
		}

		public static void Debug(this ITraceWriter tracer, HttpRequestMessage request, ApiController controller, Exception exception, string messageFormat, params object[] messageArguments)
		{
			tracer.Debug(request, controller.ControllerContext.ControllerDescriptor.ControllerType.FullName, exception, messageFormat, messageArguments);
		}

		public static void Debug(this ITraceWriter tracer, HttpRequestMessage request, ApiController controller, string messageFormat, params object[] messageArguments)
		{
			tracer.Debug(request, controller.ControllerContext.ControllerDescriptor.ControllerType.FullName, messageFormat, messageArguments);
		}

		public static void Error(this ITraceWriter tracer, HttpRequestMessage request, ApiController controller, Exception exception)
		{
			tracer.Error(request, controller.ControllerContext.ControllerDescriptor.ControllerType.FullName, exception);
		}

		public static void Error(this ITraceWriter tracer, HttpRequestMessage request, ApiController controller, Exception exception, string messageFormat, params object[] messageArguments)
		{
			tracer.Error(request, controller.ControllerContext.ControllerDescriptor.ControllerType.FullName, exception, messageFormat, messageArguments);
		}

		public static void Error(this ITraceWriter tracer, HttpRequestMessage request, ApiController controller, string messageFormat, params object[] messageArguments)
		{
			tracer.Error(request, controller.ControllerContext.ControllerDescriptor.ControllerType.FullName, messageFormat, messageArguments);
		}

		public static void Fatal(this ITraceWriter tracer, HttpRequestMessage request, ApiController controller, Exception exception)
		{
			tracer.Fatal(request, controller.ControllerContext.ControllerDescriptor.ControllerType.FullName, exception);
		}

		public static void Fatal(this ITraceWriter tracer, HttpRequestMessage request, ApiController controller, Exception exception, string messageFormat, params object[] messageArguments)
		{
			tracer.Fatal(request, controller.ControllerContext.ControllerDescriptor.ControllerType.FullName, exception, messageFormat, messageArguments);
		}

		public static void Fatal(this ITraceWriter tracer, HttpRequestMessage request, ApiController controller, string messageFormat, params object[] messageArguments)
		{
			tracer.Fatal(request, controller.ControllerContext.ControllerDescriptor.ControllerType.FullName, messageFormat, messageArguments);
		}

		public static void Info(this ITraceWriter tracer, HttpRequestMessage request, ApiController controller, Exception exception)
		{
			tracer.Info(request, controller.ControllerContext.ControllerDescriptor.ControllerType.FullName, exception);
		}

		public static void Info(this ITraceWriter tracer, HttpRequestMessage request, ApiController controller, Exception exception, string messageFormat, params object[] messageArguments)
		{
			tracer.Info(request, controller.ControllerContext.ControllerDescriptor.ControllerType.FullName, exception, messageFormat, messageArguments);
		}

		public static void Info(this ITraceWriter tracer, HttpRequestMessage request, ApiController controller, string messageFormat, params object[] messageArguments)
		{
			tracer.Info(request, controller.ControllerContext.ControllerDescriptor.ControllerType.FullName, messageFormat, messageArguments);
		}

		public static void Warn(this ITraceWriter tracer, HttpRequestMessage request, ApiController controller, Exception exception)
		{
			tracer.Warn(request, controller.ControllerContext.ControllerDescriptor.ControllerType.FullName, exception);
		}

		public static void Warn(this ITraceWriter tracer, HttpRequestMessage request, ApiController controller, Exception exception, string messageFormat, params object[] messageArguments)
		{
			tracer.Warn(request, controller.ControllerContext.ControllerDescriptor.ControllerType.FullName, exception, messageFormat, messageArguments);
		}

		public static void Warn(this ITraceWriter tracer, HttpRequestMessage request, ApiController controller, string messageFormat, params object[] messageArguments)
		{
			tracer.Warn(request, controller.ControllerContext.ControllerDescriptor.ControllerType.FullName, messageFormat, messageArguments);
		}
	}
}
