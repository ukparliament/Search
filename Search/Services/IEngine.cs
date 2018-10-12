namespace Search
{
    using System.Threading.Tasks;
    using OpenSearch;

    public interface IEngine
    {
        Task<Feed> Query(string searchTerms, int startIndex, int count);
    }
}