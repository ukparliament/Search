namespace Search
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using OpenSearch;

    public class DescriptionFormatter : TextOutputFormatter
    {
        public DescriptionFormatter() : base()
        {
            this.SupportedMediaTypes.Add(Constants.DescriptionMimeType);
            this.SupportedEncodings.Add(Encoding.UTF8);
        }

        protected override bool CanWriteType(Type type)
        {
            return typeof(Description).IsAssignableFrom(type);
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            return new TaskFactory().StartNew(() =>
            {
                using (var writer = context.WriterFactory(context.HttpContext.Response.Body, selectedEncoding))
                {
                    new XmlSerializer(context.ObjectType).Serialize(writer, context.Object, Constants.Namespaces);
                }
            });
        }
    }
}