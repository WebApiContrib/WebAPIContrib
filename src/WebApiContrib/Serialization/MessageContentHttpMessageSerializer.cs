using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiContrib.Serialization
{
	public class MessageContentHttpMessageSerializer : IHttpMessageSerializerAsync, IHttpMessageSerializer
	{
		private bool _bufferContent;

		public MessageContentHttpMessageSerializer()
			: this(false)
		{

		}

		public MessageContentHttpMessageSerializer(bool bufferContent)
		{
			_bufferContent = bufferContent;
		}


		public void Serialize(HttpResponseMessage response, Stream stream)
		{
			byte[] assuranceBuffer = null;
			if (_bufferContent && response.Content != null)
				assuranceBuffer = response.Content.ReadAsByteArrayAsync().Result; // make sure it is buffered

			var httpMessageContent = new HttpMessageContent(response);
			var buffer = httpMessageContent.ReadAsByteArrayAsync().Result;
			stream.Write(buffer, 0, buffer.Length);
		}

		public Task Serialize(Task<HttpResponseMessage> response, Stream stream)
		{
			return response.Then(r =>
			        {
			           	if(r.Content!=null)
			           	{
			           		return r.Content.LoadIntoBufferAsync()
			           			.Then(() =>
			           			      	{
											var httpMessageContent = new HttpMessageContent(r);
											// All in-memory and CPU-bound so no need to async
											var buffer = httpMessageContent.ReadAsByteArrayAsync().Result;
											return Task.Factory.FromAsync(stream.BeginWrite, stream.EndWrite,
												buffer, 0, buffer.Length, null, TaskCreationOptions.None);
										});
			           	}
			           	else
			           	{
							var httpMessageContent = new HttpMessageContent(r);
							// All in-memory and CPU-bound so no need to async
							var buffer = httpMessageContent.ReadAsByteArrayAsync().Result;
			           		return Task.Factory.FromAsync(stream.BeginWrite, stream.EndWrite, 
								buffer, 0, buffer.Length, null, TaskCreationOptions.None);
			           	}
			        }
				);
		}

		public void Serialize(HttpRequestMessage request, Stream stream)
		{
			byte[] assuranceBuffer = null;
			if (_bufferContent && request.Content != null)
				assuranceBuffer = request.Content.ReadAsByteArrayAsync().Result; // make sure it is buffered

			var httpMessageContent = new HttpMessageContent(request);
			var buffer = httpMessageContent.ReadAsByteArrayAsync().Result;
			stream.Write(buffer, 0, buffer.Length);
		}

		Task<HttpResponseMessage> IHttpMessageSerializerAsync.DeserializeToResponse(Stream stream)
		{
			throw new NotImplementedException();
		}

		Task<HttpRequestMessage> IHttpMessageSerializerAsync.DeserializeToRequest(Stream stream)
		{
			throw new NotImplementedException();
		}

		public HttpResponseMessage DeserializeToResponse(Stream stream)
		{
			var response = new HttpResponseMessage();
			var memoryStream = new MemoryStream();
			stream.CopyTo(memoryStream);
			response.Content = new ByteArrayContent(memoryStream.ToArray());
			response.Content.Headers.Add("Content-Type", "application/http;msgtype=response");
			return response.Content.ReadAsHttpResponseMessageAsync().Result;
		}

		public HttpRequestMessage DeserializeToRequest(Stream stream)
		{
			var request = new HttpRequestMessage();
			var memoryStream = new MemoryStream();
			stream.CopyTo(memoryStream);
			request.Content = new ByteArrayContent(memoryStream.ToArray());
			request.Content.Headers.Add("Content-Type", "application/http;msgtype=request");
			return request.Content.ReadAsHttpRequestMessageAsync().Result;
		}
	}
}
