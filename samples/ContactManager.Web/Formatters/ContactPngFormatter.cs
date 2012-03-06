using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using ContactManager.Models;

namespace ContactManager.Web.Formatters
{
    public class ContactPngFormatter : BufferedMediaTypeFormatter
    {
        public ContactPngFormatter()
        {
            SupportedMediaTypes.Add(
                new MediaTypeHeaderValue("image/png"));
        }

        protected override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, FormatterContext formatterContext, TransportContext context)
        {
            var contact = value as Contact;

            if (contact != null)
            {
                var imageId = contact.Id % 8;
                if (imageId == 0)
                {
                    imageId++;
                }

                var path = string.Format(CultureInfo.InvariantCulture, @"{0}bin\Images\Image{1}.png", AppDomain.CurrentDomain.BaseDirectory, imageId);
                
                using (var fileStream = new FileStream(path, FileMode.Open))
                {
                    byte[] bytes = new byte[fileStream.Length];
                    fileStream.Read(bytes, 0, (int)fileStream.Length);
                    stream.Write(bytes, 0, (int)fileStream.Length);
                }
            }
        }

        protected override bool CanWriteType(Type type)
        {
            return true;
        }
    }
}
