using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using NUnit.Framework;
using WebApiContrib.Conneg;

namespace WebApiContribTests.Conneg
{
    public static class UnitTest
    {
        [TestCase("text/xml")]
        [TestCase("text/json")]
        public static void WhenAcceptHeaderIsMediaTypeThenNegotiatedMediaTypeMatches(string expected)
        {
            var actual = new DefaultContentNegotiator().Negotiate(new[] { "text/xml", "text/json" }, new[] { expected });

            Assert.AreEqual(expected, actual);
        }

        [TestCase("text/xml", 0.8, "text/json", 0.9, "text/json")]
        [TestCase("text/xml", 0.9, "text/xml", 0.8, "text/xml")]
        public static void WhenAcceptHeaderValuesHaveQualityThenNegotiatedMediaTypeMatchesHigherQuality(string mediaType1, double quality1, string mediaType2, double quality2, string expected)
        {
            var actual = new DefaultContentNegotiator().Negotiate(
                new[] { "text/xml", "text/json" },
                new[]
                {
                    new MediaTypeWithQualityHeaderValue(mediaType1, quality1),
                    new MediaTypeWithQualityHeaderValue(mediaType2, quality2)
                });

            Assert.AreEqual(expected, actual);
        }

        [TestCase("text/xml;q=0.8", "text/json;q=0.9", "text/json")]
        [TestCase("text/xml;q=0.9", "text/xml;q=0.8", "text/xml")]
        public static void WhenAcceptHeaderStringsHaveQualityThenNegotiatedMediaTypeMatchesHigherQuality(string mediaType1, string mediaType2, string expected)
        {
            var actual = new DefaultContentNegotiator().Negotiate(new[] { "text/xml", "text/json" }, new[] { mediaType1, mediaType2 });

            Assert.AreEqual(expected, actual);
        }

        [TestCase("text/xml;q=0.8, text/json;q=0.9", "text/json")]
        [TestCase("text/xml;q=0.9, text/xml;q=0.8", "text/xml")]
        public static void WhenAcceptHeaderStringHasQualityThenNegotiatedMediaTypeMatchesHigherQuality(string acceptHeader, string expected)
        {
            var actual = new DefaultContentNegotiator().Negotiate(new[] { "text/xml", "text/json" }, acceptHeader);

            Assert.AreEqual(expected, actual);
        }
    }
}
