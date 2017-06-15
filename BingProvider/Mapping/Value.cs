namespace BingProvider
{
    using Newtonsoft.Json;
    using System;
    using System.Web;

    public class Value
    {
        [JsonProperty("displayUrl")]
        public Uri DisplayUrl { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        public Uri Uri
        {
            get
            {
                var queryParameters = HttpUtility.ParseQueryString(Url.Query);
                var uriString = queryParameters["r"];

                return new Uri(uriString);
            }
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("snippet")]
        public string Snippet { get; set; }

        [JsonProperty("dateLastCrawled")]
        public DateTime DateLastCrawled { get; set; }
    }
}
