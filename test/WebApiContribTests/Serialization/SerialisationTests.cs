using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace WebApiContribTests.Serialization
{
	[TestFixture]
	public class SerialisationTests
	{
		[Test]
		public void TestRequest()
		{
			var a = new MyStruct(){X = 5, Y = 3};
			var b = new MyStruct(){X = 5, Y = 3};
			Assert.AreEqual(a, b);
			
		}

		private struct MyStruct
		{
			public int X;
			public int Y;
		}
	}
}
