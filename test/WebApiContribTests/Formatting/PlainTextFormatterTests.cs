using System;
using System.IO;
using System.Net.Http;
using NUnit.Framework;
using Should;
using WebApiContrib.Formatting;

namespace WebApiContribTests.Formatting
{
     [TestFixture]
    public class PlainTextFormatterTests
    {
        [Test]
        public void Should_support_only_json_media_type()
        {
            var formatter = new PlainTextFormatter();

            formatter.SupportedMediaTypes.Count.ShouldEqual(1);
            formatter.SupportedMediaTypes.ShouldContain(StandardMediaTypeHeaderValues.TextPlain);
        }

        [Test]
        public void Should_write_string_to_stream()
        {
            var formatter = new PlainTextFormatter();
            

            var content = new StringContent(string.Empty);
            content.Headers.Clear();
            var memoryStream = new MemoryStream();
            var value = "Hello World";
            var resultTask = formatter.WriteToStreamAsync(typeof(string), value, memoryStream, content, transportContext: null);

            resultTask.Wait();

            memoryStream.Position = 0;
            string serializedString = new StreamReader(memoryStream).ReadToEnd();

            
            serializedString.ShouldEqual(value);
        }

      
        [Test]
        public void Should_read_serialized_object_from_stream()
        {
            var formatter = new PlainTextFormatter();
            var value = "Hello World";
            
            var memoryStream = new MemoryStream();
            var sr = new StreamWriter(memoryStream);
            sr.Write(value);
            sr.Flush();
            memoryStream.Position = 0;
            var content = new StringContent(string.Empty);
            content.Headers.Clear();

            var resultTask = formatter.ReadFromStreamAsync(typeof(string), memoryStream, content, null);

            resultTask.Wait();

            resultTask.Result.ShouldBeType<String>();

            var result = (String)resultTask.Result;

            result.ShouldEqual(value);
        }
    }
}
