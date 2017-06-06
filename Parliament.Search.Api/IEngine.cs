namespace Parliament.Search.Api
{
    using Parliament.Search.OpenSearch;

    internal interface IEngine
    {
        Feed Search(string searchTerms, int startIndex, int pageSize);
    }
}