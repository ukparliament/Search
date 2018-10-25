namespace Search
{
    using Microsoft.AspNetCore.Mvc;

    public class QueryParameters
    {
        [FromQuery(Name = "q")]
        public string SearchTerms { get; set; }

        [FromQuery(Name = "start")]
        public int StartIndex { get; set; } = 1;

        [FromQuery(Name = "count")]
        public int Count { get; set; } = 10;

        public string Format { get; set; }
    }
}