namespace BingProvider
{
    using System;

    internal class BingQuery
    {
        private const string prefix = "https://api.cognitive.microsoft.com/bing/v5.0/search";

        public string QueryString { get; set; }

        public string Site { get; set; }

        public int Count { get; set; }

        public int Offset { get; set; }

        public override string ToString()
        {
            var queryString = this.QueryString;

            if (!string.IsNullOrWhiteSpace(this.Site))
            {
                queryString = string.Format("site:{0}+{1}", this.Site, this.QueryString);
            }

            return string.Format("{0}?q={1}&count={2}&offset={3}", BingQuery.prefix, queryString, this.Count, this.Offset);
        }

        public static implicit operator Uri(BingQuery original)
        {
            return new Uri(original.ToString());
        }
    }
}
