using IntegrationTests.Factory;

namespace IntegrationTests.Collections
{
    [CollectionDefinition("Database", DisableParallelization = true)]
    public class DatabaseCollection : ICollectionFixture<WebFactory>
    {
    }
}
