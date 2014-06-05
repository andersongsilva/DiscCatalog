using System.Configuration;
using MongoDB.Driver;

namespace DiscCatalog
{
    public class MongoDBHandler<T>
    {
        public MongoDatabase Database { get; private set; }

        public MongoDBHandler()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MongoLab"].ConnectionString;
            var mongoClient = new MongoClient(connectionString);
            var mongoServer = mongoClient.GetServer();
            Database = mongoServer.GetDatabase("disccatalog");
        }
    }
}