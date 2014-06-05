using System.Linq;
using DiscCatalog.Controllers;
using MongoDB.Driver;
using NUnit.Framework;

namespace DiscCatalog.Tests.Controllers
{
    [TestFixture]
    public class CollectionControllerTest
    {
        private MongoDatabase _database;
        private CollectionController _collectionController;
        private const string Collection1 = "collection1";
        private const string Collection2 = "collection2";

        [SetUp]
        public void Init()
        {
            const string connectionString = "mongodb://disccataloguser:disccatalogpwd@ds027308.mongolab.com:27308/disccatalog_test";
            var mongoClient = new MongoClient(connectionString);
            var mongoServer = mongoClient.GetServer();
            _database = mongoServer.GetDatabase("disccatalog_test");

            PrepareDatabase();

            _collectionController = new CollectionController(_database);
        }

        private void PrepareDatabase()
        {
            var collectionNames = _database.GetCollectionNames().ToList().FindAll(x => x != "system.indexes" && x != "system.users");
            collectionNames.ForEach(c => _database.DropCollection(c));

            _database.DropCollection(Collection1);
            _database.DropCollection(Collection2);

            _database.CreateCollection(Collection1);
            _database.CreateCollection(Collection2);
        }

        [Test]
        public void Get()
        {
            // Act
            var collectionList = _collectionController.Get().ToList();

            // Assert
            Assert.IsNotNull(collectionList);
            Assert.AreEqual(4, collectionList.Count());
            Assert.IsTrue(collectionList.Contains(Collection1));
            Assert.IsTrue(collectionList.Contains(Collection2));
        }

        [Test]
        public void GetByName()
        {
            // Act
            var collection = _collectionController.Get(Collection1);

            // Assert
            Assert.IsNotNull(collection);
            Assert.AreEqual(Collection1, collection.Name);
        }

        [Test]
        public void Post()
        {
            // Act
            _collectionController.Post("collection3");

            // Assert
            Assert.IsTrue(_database.CollectionExists("collection3"));
        }

        [Test]
        public void Post_CollectionWithSameName()
        {
            // Act and assert
            Assert.Throws<MongoCommandException>(() => _collectionController.Post(Collection1));
        }

        [Test]
        public void Put()
        {
            // Arrange
            const string collection3 = "collection3";

            // Act
            _collectionController.Put(Collection1, collection3);

            // Assert
            Assert.IsTrue(_database.CollectionExists(collection3));
            Assert.IsFalse(_database.CollectionExists(Collection1));
        }

        [Test]
        public void Put_NotExistingCollection()
        {
            // Arrange
            const string collection3 = "collection3";
            const string collection4 = "collection4";

            // Act
            _collectionController.Put(collection3, collection4);

            // Assert
            Assert.IsFalse(_database.CollectionExists(collection3));
            Assert.IsFalse(_database.CollectionExists(collection4));
        }

        [Test]
        public void Delete()
        {
            // Act
            _collectionController.Delete(Collection2);
            // Assert
            Assert.IsFalse(_database.CollectionExists(Collection2));
        }
    }
}
