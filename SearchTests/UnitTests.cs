namespace SearchTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Search;

    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void OpenApi_document_is_valid()
        {
            var document = Resources.OpenApiDocument;
        }
    }
}
