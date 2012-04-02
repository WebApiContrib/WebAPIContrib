using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using NUnit.Framework;
using ServiceStack.Text;
using WebApiContrib.MediaTypeFormatters;
using Should;

namespace WebApiContribTests.MediaTypeFormatters
{
    [TestFixture]
    public class ServiceStackTextJsonMediaTypeFormatterTests
    {
        [Test]
        public void Should_support_only_json_media_type()
        {
            var formatter = new ServiceStackTextJsonMediaTypeFormatter();

            formatter.SupportedMediaTypes.Count.ShouldEqual(1);
            formatter.SupportedMediaTypes.ShouldContain(StandardMediaTypeHeaderValues.ApplicationJson);
        }

        [Test]
        public void Should_write_serialized_object_to_stream()
        {
            var formatter = new ServiceStackTextJsonMediaTypeFormatter();
            var value = GetTestObject();

            var contentHeader = new StringContent(string.Empty).Headers;
            contentHeader.Clear();
            var memoryStream = new MemoryStream();

            var resultTask = formatter.WriteToStreamAsync(typeof(RootClass), value, memoryStream, contentHeader, new FormatterContext(StandardMediaTypeHeaderValues.ApplicationJson, isRead: false), transportContext: null);

            resultTask.Wait();

            memoryStream.Position = 0;
            string serializedString = new StreamReader(memoryStream).ReadToEnd();

            // Formatter uses ISO8601 dates by default
            JsConfig.DateHandler = JsonDateHandler.ISO8601;
            var expextedResult = value.ToJson();
            JsConfig.Reset();
            serializedString.ShouldEqual(expextedResult);
        }

        [Test]
        public void Should_write_serialized_object_to_stream_using_date_handler()
        {
            var formatter = new ServiceStackTextJsonMediaTypeFormatter(JsonDateHandler.TimestampOffset);
            var value = GetTestObject();

            var contentHeader = new StringContent(string.Empty).Headers;
            contentHeader.Clear();
            var memoryStream = new MemoryStream();

            var resultTask = formatter.WriteToStreamAsync(typeof(RootClass), value, memoryStream, contentHeader, new FormatterContext(StandardMediaTypeHeaderValues.ApplicationJson, isRead: false), transportContext: null);

            resultTask.Wait();

            memoryStream.Position = 0;
            string serializedString = new StreamReader(memoryStream).ReadToEnd();

            JsConfig.DateHandler = JsonDateHandler.TimestampOffset;
            var expected = value.ToJson();
            JsConfig.Reset();
            serializedString.ShouldEqual(expected);
        }

        [Test]
        public void Should_read_serialized_object_from_stream()
        {
            var formatter = new ServiceStackTextJsonMediaTypeFormatter();
            var value = GetTestObject();
            var utf8Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

            //Media Type Formatter uses ISO8601 date formatting by default;
            JsConfig.DateHandler = JsonDateHandler.ISO8601;
            byte[] data = utf8Encoding.GetBytes(value.ToJson());
            JsConfig.Reset();
            var memoryStream = new MemoryStream(data);

            var contentHeader = new StringContent(string.Empty).Headers;
            contentHeader.Clear();

            var resultTask = formatter.ReadFromStreamAsync(typeof(RootClass), memoryStream, contentHeader, new FormatterContext(StandardMediaTypeHeaderValues.ApplicationJson, isRead: false));

            resultTask.Wait();

            resultTask.Result.ShouldBeType<RootClass>();

            var result = (RootClass)resultTask.Result;

            result.StringProperty.ShouldEqual(value.StringProperty);
            result.DateProperty.ShouldEqual(value.DateProperty);
            result.Child.BooleanProperty.ShouldEqual(value.Child.BooleanProperty);
            result.Child.DecimalProperty.ShouldEqual(value.Child.DecimalProperty);
            result.Child.DoubleProperty.ShouldEqual(value.Child.DoubleProperty);
            result.Child.IntegerProperty.ShouldEqual(value.Child.IntegerProperty);
            result.Child.StringProperty.ShouldEqual(value.Child.StringProperty);
        }

        [Test]
        public void Should_read_serialized_object_from_stream_using_date_handler()
        {
            var formatter = new ServiceStackTextJsonMediaTypeFormatter(JsonDateHandler.DCJSCompatible);
            var value = GetTestObject();
            var utf8Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

            JsConfig.DateHandler = JsonDateHandler.DCJSCompatible;
            byte[] data = utf8Encoding.GetBytes(value.ToJson());
            JsConfig.Reset();

            var memoryStream = new MemoryStream(data);

            var contentHeader = new StringContent(string.Empty).Headers;
            contentHeader.Clear();

            var resultTask = formatter.ReadFromStreamAsync(typeof(RootClass), memoryStream, contentHeader, new FormatterContext(StandardMediaTypeHeaderValues.ApplicationJson, isRead: false));

            resultTask.Wait();

            resultTask.Result.ShouldBeType<RootClass>();

            var result = (RootClass)resultTask.Result;

            result.StringProperty.ShouldEqual(value.StringProperty);
            result.DateProperty.ShouldEqual(value.DateProperty);
            result.Child.BooleanProperty.ShouldEqual(value.Child.BooleanProperty);
            result.Child.DecimalProperty.ShouldEqual(value.Child.DecimalProperty);
            result.Child.DoubleProperty.ShouldEqual(value.Child.DoubleProperty);
            result.Child.IntegerProperty.ShouldEqual(value.Child.IntegerProperty);
            result.Child.StringProperty.ShouldEqual(value.Child.StringProperty);
        }

        private static RootClass GetTestObject()
        {
            var @object = new RootClass()
            {
                StringProperty = "Root String Value",
                DateProperty = new DateTime(2012, 3, 31, 16, 27, 55),
                Child = new ChildClass()
                {
                    IntegerProperty = 23,
                    StringProperty = "Child String Value",
                    DecimalProperty = 12.5m,
                    DoubleProperty = 23.45,
                    BooleanProperty = false
                }

            };
            return @object;
        }

        public class RootClass
        {
            public string StringProperty { get; set; }
            public DateTime DateProperty { get; set; }
            public ChildClass Child { get; set; }

            public override bool Equals(object obj)
            {
                return true;
            }

        }

        public class ChildClass
        {
            public int IntegerProperty { get; set; }
            public string StringProperty { get; set; }
            public decimal DecimalProperty { get; set; }
            public double DoubleProperty { get; set; }
            public bool BooleanProperty { get; set; }
        }

    }
}