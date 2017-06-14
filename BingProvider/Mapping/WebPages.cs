namespace BingProvider
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class WebPages
    {
        [JsonProperty("totalEstimatedMatches")]
        public int TotalEstimatedMatches { get; set; }

        [JsonProperty("value")]
        public IList<Value> Values { get; set; }
    }
}
