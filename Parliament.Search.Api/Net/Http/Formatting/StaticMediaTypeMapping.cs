namespace Parliament.Net.Http.Formatting
{
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;

    public class StaticMediaTypeMapping : MediaTypeMapping
    {
        public StaticMediaTypeMapping(string mediaType) : base(mediaType) { }

        public StaticMediaTypeMapping(MediaTypeHeaderValue mediaType) : base(mediaType) { }

        public override double TryMatchMediaType(HttpRequestMessage request)
        {
            return 1.0;
        }
    }
}