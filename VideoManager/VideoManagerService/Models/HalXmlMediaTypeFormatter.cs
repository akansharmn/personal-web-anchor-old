using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web;
using System.Xml;
using Models.Models;

namespace VideoManagerService.Models
{
    public class HalXmlMediaTypeFormatter : BufferedMediaTypeFormatter
    {
        public HalXmlMediaTypeFormatter() : base()
        {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/hal+xml"));
        }
        public override bool CanReadType(Type type)
        {
            return type.BaseType == typeof(LinkedResource);// || type.BaseType.GetGenericTypeDefinition() == typeof()
        }

        public override bool CanWriteType(Type type)
        {
            return type.BaseType == typeof(LinkedResource);
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            var encoding = base.SelectCharacterEncoding(content.Headers);
            var settings = new XmlWriterSettings();
            settings.Encoding = encoding;
            var writer = XmlWriter.Create(writeStream, settings);
            var resource = (LinkedResource)value;
            if(resource is IEnumerable)
            {
                writer.WriteStartElement("resource");
                writer.WriteAttributeString("href", resource.HRef);
                foreach(LinkedResource innerResource in (IEnumerable)resource)
                {
                   // SerializeInnerResource(writer, innerResource);
                }
                writer.WriteEndElement();
            }
            else
            {
               // SerializeInnerResource(writer, resource);
            }
            writer.Flush();
            writer.Close();
            base.WriteToStream(type, value, writeStream, content);
        }
    }
}