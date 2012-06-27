using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using WebApiContrib.Caching;

namespace WebApiContribTests.Caching
{
	public static class TimedEntityTagHeaderValueTests
	{
		[TestCase("\"1234567\"", false)]
		[TestCase("\"1234567\"", true)]
		public static void ToStringAndTryParseTest(string tag, bool isWeak)
		{
			var headerValue = new TimedEntityTagHeaderValue(tag, isWeak);
			var s = headerValue.ToString();
			TimedEntityTagHeaderValue headerValue2 = null;
			Assert.IsTrue(TimedEntityTagHeaderValue.TryParse(s, out headerValue2));
			Assert.AreEqual(headerValue.Tag, headerValue2.Tag);
			Assert.AreEqual(headerValue.LastModified.ToString(), headerValue2.LastModified.ToString());
			Assert.AreEqual(headerValue.IsWeak, headerValue2.IsWeak);
			Assert.AreEqual(headerValue.ToString(), headerValue2.ToString());
		}
	}
}
