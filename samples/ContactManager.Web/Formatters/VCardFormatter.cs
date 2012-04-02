using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using ContactManager.Models;

namespace ContactManager.Web.Formatters
{
    public class VCardFormatter : BufferedMediaTypeFormatter
    {
        public VCardFormatter()
        {
            SupportedMediaTypes.Add(
                new MediaTypeHeaderValue("text/directory"));
        }

        protected override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, FormatterContext formatterContext, TransportContext transportContext)
        {
            var contacts = value as IEnumerable<Contact>;

            if (contacts != null)
            {
                foreach (var contact in contacts)
                {
                    WriteContact(contact, stream);
                }
            }

            else
            {
                var singleContact = value as Contact;

                if (singleContact != null)
                {
                    WriteContact(singleContact, stream);
                }
            }
        }

        private void WriteContact(Contact contact, Stream stream)
        {
            var writer = new StreamWriter(stream);
            writer.WriteLine("BEGIN:VCARD");
            writer.WriteLine(string.Format("FN:{0}", contact.Name));
            writer.WriteLine(string.Format("ADR;TYPE=HOME;{0};{1};{2}", contact.Address, contact.City, contact.Zip));
            writer.WriteLine(string.Format("EMAIL;TYPE=PREF,INTERNET:{0}", contact.Email));
            writer.WriteLine("END:VCARD");
            writer.Flush();
        }

        protected override bool CanWriteType(Type type)
        {
            return true;
        }
    }
}