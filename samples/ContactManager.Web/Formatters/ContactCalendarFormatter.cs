using System;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using ContactManager.Models;

namespace ContactManager.Web.Formatters
{
    public class ContactCalendarFormatter : BufferedMediaTypeFormatter
    {
        public ContactCalendarFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/calendar"));
        }

        protected override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, FormatterContext formatterContext, TransportContext transportContext)
        {
            var singleContact = value as Contact;

            if (singleContact != null)
            {
                WriteEvent(singleContact, stream);
            }
        }

        protected override bool CanWriteType(Type type)
        {
            return (type == typeof (Contact));
        }

        private void WriteEvent(Contact contact, Stream stream)
        {
            const string dateFormat = "yyyyMMddTHHmmssZ";
            var eventDate = DateTime.Now.ToUniversalTime().AddDays(2).AddHours(4);
            var writer = new StreamWriter(stream);
            writer.WriteLine("BEGIN:VCALENDAR");
            writer.WriteLine("VERSION:2.0");
            writer.WriteLine("BEGIN:VEVENT");
            writer.WriteLine(string.Format("UID:{0}", contact.Email));
            writer.WriteLine(string.Format("DTSTAMP:{0}", DateTime.Now.ToUniversalTime().ToString(dateFormat)));
            writer.WriteLine(string.Format("DTSTART:{0}", eventDate.ToString(dateFormat)));
            writer.WriteLine(string.Format("DTEND:{0}", eventDate.AddHours(1).ToString(dateFormat)));
            writer.WriteLine("SUMMARY:Discuss WCF Web API");
            writer.WriteLine("END:VEVENT");
            writer.WriteLine("END:VCALENDAR");
            writer.Flush();
        }
    }
}