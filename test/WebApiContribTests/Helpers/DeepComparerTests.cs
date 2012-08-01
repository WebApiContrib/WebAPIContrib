using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using NUnit.Framework;

namespace WebApiContribTests.Helpers
{
	[TestFixture]
	public class DeepComparerTests
	{
		[Test]
		public void TestNulls()
		{
			Assert.AreEqual(0, DeepComparer.Compare<string>(null, null).Count());
		}

		[Test]
		public void TestNonNulls()
		{
			Assert.AreEqual(1, DeepComparer.Compare<string>(null, "").Count());
		}

		[Test]
		public void TestPrimitive_Pass()
		{
			Assert.AreEqual(0, DeepComparer.Compare(6, 6).Count());
		}

		[Test]
		public void TestPrimitive_Fail()
		{
			Assert.AreEqual(1, DeepComparer.Compare(7, 6).Count());
		}

		[Test]
		public void TestNullable_Pass()
		{
			int? a = 6;
			int? b = 6;
			Assert.AreEqual(0, DeepComparer.Compare(a, b).Count());
		}

		[Test]
		public void TestNullable_Fail()
		{
			int? a = 7;
			int? b = 6;
			Assert.AreEqual(1, DeepComparer.Compare(a, b).Count());
		}

		[Test]
		public void TestNullable_Fail_Null()
		{
			int? a = null;
			int? b = 6;
			Assert.AreEqual(1, DeepComparer.Compare(a, b).Count());
		}

		[Test]
		public void TestClass_Pass()
		{
			var a = new HttpResponseMessage();
			var b = new HttpResponseMessage();
			var list = DeepComparer.Compare(a, b);
			foreach (var s in list)
				Console.WriteLine(s);
			
			Assert.AreEqual(0, list.Count());
		}

		[Test]
		public void TestClass_Fail()
		{
			var a = new HttpResponseMessage();
			var b = new HttpResponseMessage( HttpStatusCode.Continue);
			var list = DeepComparer.Compare(a, b);
			foreach (var s in list)
				Console.WriteLine(s);
			Assert.AreEqual(3, list.Count());
		}

	}
}
