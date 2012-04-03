using System;
using System.IO;
using System.Json;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using NUnit.Framework;
using WebApiContrib.Formatting;

namespace WebApiContribTests.Formatting
{
    [TestFixture]
    public class ReadWriteFormUrlEncodedFormatterTests
    {
        [Test]
        public void FlatObjectTests()
        {
            TestJsonObjectAndFormsEncodedConversion("a=1", new JsonObject { { "a", 1 } }, false);
            TestJsonObjectAndFormsEncodedConversion("a=1", new JsonObject { { "a", "1" } }, true);

            TestJsonObjectAndFormsEncodedConversion("a=1&b=2", new JsonObject { { "a", "1" }, { "b", "2" } }, true);
            TestJsonObjectAndFormsEncodedConversion("a=1&b=2.5", new JsonObject { { "a", "1" }, { "b", "2.5" } }, true);
        }

        [Test]
        public void SimpleArrays()
        {
            TestJsonObjectAndFormsEncodedConversion("a[]=1&a[]=2", new JsonObject { { "a", new JsonArray("1", "2") } }, true);
            TestJsonObjectAndFormsEncodedConversion("a[]=1&a[]=2&b=3", new JsonObject { { "a", new JsonArray("1", "2") }, { "b", "3" } }, true);
        }

        [Test]
        public void MultiDimArrays()
        {
            TestJsonObjectAndFormsEncodedConversion("a[0][0][]=1", (JsonObject)JsonValue.Parse("{\"a\":[[[\"1\"]]]}"), true);
            TestJsonObjectAndFormsEncodedConversion("a[0][]=1&a[0][]=2&a[1][]=3&a[1][]=4", (JsonObject)JsonValue.Parse("{\"a\":[[1,2],[3,4]]}"), false);
            TestJsonObjectAndFormsEncodedConversion("a[0][]=1&a[0][]=2&a[1][]=3&a[1][]=4",
                (JsonObject)JsonValue.Parse("{'a':[['1','2'],['3','4']]}".Replace('\'', '\"')), true);
            TestJsonObjectAndFormsEncodedConversion("a[0][]=1&a[0][]=2&a[0][]=3&a[1][]=4",
                (JsonObject)JsonValue.Parse("{'a':[['1','2','3'],['4']]}".Replace('\'', '\"')), true);
        }

        [Test]
        public void DeepObjects()
        {
            TestJsonObjectAndFormsEncodedConversion("a[b]=1&a[c]=2&a[d][]=3&a[e][f]=4",
                (JsonObject)JsonValue.Parse("{'a':{'b':'1','c':'2','d':['3'],'e':{'f':'4'}}}"
                    .Replace('\'', '\"')), true);
        }

        [Test]
        public void NullValues()
        {
            TestJsonObjectAndFormsEncodedConversion("a[b]=1&a[d][]=3",
                (JsonObject)JsonValue.Parse("{'a':{'b':'1','c':null,'d':['3'],'e':{'f':null}}}"
                    .Replace('\'', '\"')), false);
        }

        void TestJsonObjectAndFormsEncodedConversion(string formUrlEncoded, JsonObject json, bool formsToJsonShouldSucceed)
        {
            var formatter = new ReadWriteFormUrlEncodedFormatter();
            var ms = new MemoryStream();
            formatter.WriteToStreamAsync(
                typeof(JsonObject),
                json,
                ms,
                null,
                new FormatterContext(new MediaTypeHeaderValue("application/x-www-form-urlencoded"), false),
                null).Wait();
            Assert.AreEqual(formUrlEncoded, Uri.UnescapeDataString(Encoding.UTF8.GetString(ms.ToArray())));

            if (formsToJsonShouldSucceed)
            {
                var jo = formatter.ReadFromStreamAsync(
                    typeof(JsonObject),
                    new MemoryStream(Encoding.UTF8.GetBytes(formUrlEncoded)),
                    null,
                    new FormatterContext(new MediaTypeHeaderValue("application/x-www-form-urlencoded"), true)).Result as JsonObject;
                Assert.AreEqual(json.ToString(), Uri.UnescapeDataString(jo.ToString()));
            }
        }
    }
}