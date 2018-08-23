namespace Library
{
    using System.Threading.Tasks;
    using Parliament.Search.OpenSearch;

    public interface IEngine
    {
        Task<Feed> Search(string searchTerms, int startIndex, int count);
    }
}