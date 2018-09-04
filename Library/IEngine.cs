namespace Library
{
    using System.Threading.Tasks;
    using Parliament.Search.OpenSearch;

    public interface IEngine
    {
        Task<Feed> Query(string searchTerms, int startIndex, int count);
    }
}