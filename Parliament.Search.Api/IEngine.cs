namespace Parliament.Search.Api
{
    using Parliament.Search.OpenSearch;

    public interface IEngine
    {
        Feed Search(string searchTerms, int startIndex, int pageSize);
    }
}