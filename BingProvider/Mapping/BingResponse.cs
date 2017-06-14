namespace BingProvider
{
    using Newtonsoft.Json;

    internal class BingResponse
    {
        [JsonProperty("webPages")]
        public WebPages WebPages { get; set; }
    }
}
