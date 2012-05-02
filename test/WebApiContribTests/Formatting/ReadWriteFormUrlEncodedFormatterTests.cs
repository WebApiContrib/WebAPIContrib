using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using WebApiContrib.Formatting;

namespace WebApiContribTests.Formatting
{
    [TestFixture]
    public class ReadWriteFormUrlEncodedFormatterTests
    {
        [Test]
        public void FlatObjectTests()
        {
            TestJObjectAndFormsEncodedConversion("a=1", new JObject { { "a", 1 } }, false);
            TestJObjectAndFormsEncodedConversion("a=1", new JObject { { "a", "1" } }, true);

            TestJObjectAndFormsEncodedConversion("a=1&b=2", new JObject { { "a", "1" }, { "b", "2" } }, true);
            TestJObjectAndFormsEncodedConversion("a=1&b=2.5", new JObject { { "a", "1" }, { "b", "2.5" } }, true);
        }

        [Test]
        public void SimpleArrays()
        {
            TestJObjectAndFormsEncodedConversion("a[]=1&a[]=2", new JObject { { "a", new JArray("1", "2") } }, true);
            TestJObjectAndFormsEncodedConversion("a[]=1&a[]=2&b=3", new JObject { { "a", new JArray("1", "2") }, { "b", "3" } }, true);
        }

        [Test]
        public void MultiDimArrays()
        {
            TestJObjectAndFormsEncodedConversion("a[0][0][]=1", JObject.Parse("{\"a\":[[[\"1\"]]]}"), true);
            TestJObjectAndFormsEncodedConversion("a[0][]=1&a[0][]=2&a[1][]=3&a[1][]=4", JObject.Parse("{\"a\":[[1,2],[3,4]]}"), false);
            TestJObjectAndFormsEncodedConversion("a[0][]=1&a[0][]=2&a[1][]=3&a[1][]=4", JObject.Parse("{'a':[['1','2'],['3','4']]}".Replace('\'', '\"')), true);
            TestJObjectAndFormsEncodedConversion("a[0][]=1&a[0][]=2&a[0][]=3&a[1][]=4", JObject.Parse("{'a':[['1','2','3'],['4']]}".Replace('\'', '\"')), true);
        }

        [Test]
        public void DeepObjects()
        {
            TestJObjectAndFormsEncodedConversion("a[b]=1&a[c]=2&a[d][]=3&a[e][f]=4", JObject.Parse("{'a':{'b':'1','c':'2','d':['3'],'e':{'f':'4'}}}" .Replace('\'', '\"')), true);
        }

        [Test]
        public void NullValues()
        {
            TestJObjectAndFormsEncodedConversion("a[b]=1&a[d][]=3", JObject.Parse("{'a':{'b':'1','c':null,'d':['3'],'e':{'f':null}}}" .Replace('\'', '\"')), false);
        }

        void TestJObjectAndFormsEncodedConversion(string formUrlEncoded, JObject json, bool formsToJsonShouldSucceed)
        {
            var formatter = new ReadWriteFormUrlEncodedFormatter();
            var ms = new MemoryStream();
            formatter.WriteToStreamAsync(typeof(JObject), json, ms, null, null).Wait();
            Assert.AreEqual(formUrlEncoded, Uri.UnescapeDataString(Encoding.UTF8.GetString(ms.ToArray())));

            if (formsToJsonShouldSucceed)
            {
            	var jo = formatter.ReadFromStreamAsync(typeof(JObject), new MemoryStream(Encoding.UTF8.GetBytes(formUrlEncoded)), null, null);
                Assert.AreEqual(json.ToString(), Uri.UnescapeDataString(jo.ToString()));
            }
        }
    }
}
