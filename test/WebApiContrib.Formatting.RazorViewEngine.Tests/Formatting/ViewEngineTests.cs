using System.IO;
using NUnit.Framework;
using WebApiContrib.Formatting.RazorViewEngine;

namespace WebApiContribTests.Formatting
{
    [TestFixture]
    public class ViewEngineTests
    {
        [Test]
        public void render_simple_template()
        {
            var formatter = new ViewEngineFormatter(new RazorViewEngine());

            MemoryStream templateStream = GetStreamFromString("Hello @Model.Name! Welcome to Razor!");

            var outputStream = new MemoryStream();

            var view = new View(templateStream, new {Name = "foo"});
            
            var task = formatter.WriteToStreamAsync(typeof(View), view, outputStream, null, null);

            task.Wait();

            outputStream.Position = 0;

            var output = new StreamReader(outputStream).ReadToEnd();

            Assert.AreEqual("Hello foo! Welcome to Razor!",output);
        }

        [Test, Explicit]
        public void render_template_with_embedded_layout()
        {
            MemoryStream templateStream = GetStreamFromString(@"@{_Layout = ""~/Embed.cshtml"";}Hello @Model.Name! Welcome to Razor!");

            var outputStream = new MemoryStream();

            var formatter = new ViewEngineFormatter(new RazorViewEngine(this.GetType()));

            var view = new View(templateStream, new { Name = "foo" });

            var task = formatter.WriteToStreamAsync(typeof(View), view, outputStream, null, null);

            task.Wait();

            outputStream.Position = 0;

            var output = new StreamReader(outputStream).ReadToEnd();

            Assert.AreEqual("<html>Hello foo! Welcome to Razor!</html>", output);
        }

        private MemoryStream GetStreamFromString(string helloModelNameWelcomeToRazor)
        {
            var templateStream = new MemoryStream();
            var sr = new StreamWriter(templateStream);
            sr.Write(helloModelNameWelcomeToRazor);
            sr.Flush();
            templateStream.Position = 0;
            return templateStream;
        }
    }
}
