using System.Linq;
using System.Runtime.InteropServices;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using DiscCatalog.Controllers;
using DiscCatalog.Models;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NUnit.Framework;

namespace DiscCatalog.Tests.Controllers
{
    [TestFixture]
    public class DiscControllerTest
    {
        private MongoDatabase _database;
        private MongoCollection<Disc> _discCollection;
        private DiscController _discController;
        Disc _disc1, _disc2, _disc3, _disc4;
        private const string CollectionName = "discs";

        [SetUp]
        public void Init()
        {
            const string connectionString = "mongodb://disccataloguser:disccatalogpwd@ds027308.mongolab.com:27308/disccatalog_test";
            var mongoClient = new MongoClient(connectionString);
            var mongoServer = mongoClient.GetServer();
            _database = mongoServer.GetDatabase("disccatalog_test");

            PrepareDatabase();

            _discController = new DiscController(_database)
            {
                ControllerContext = new HttpControllerContext
                {
                    RouteData = new HttpRouteData(new HttpRoute(""))
                }
            };
            _discController.ControllerContext.RouteData.Values.Add("collectionName", CollectionName);
        }

        private void PrepareDatabase()
        {
            _database.DropCollection(CollectionName);
            _discCollection = _database.GetCollection<Disc>(CollectionName);

            var keys = IndexKeys.Ascending("Name", "Artist", "Year");
            var options = IndexOptions.SetUnique(true);
            _discCollection.CreateIndex(keys, options);

            _discCollection.RemoveAll();

            _disc1 = new Disc { Name = "Disc1", Artist = "Artist1", Gener = "Gener1", Year = "Year1" };
            _disc2 = new Disc { Name = "Disc2", Artist = "Artist2", Gener = "Gener1", Year = "Year1" };
            _disc3 = new Disc { Name = "Disc3", Artist = "Artist1", Gener = "Gener1", Year = "Year2" };
            _disc4 = new Disc { Name = "Disc4", Artist = "Artist3", Gener = "Gener2", Year = "Year2" };

            _discCollection.Insert(_disc1);
            _discCollection.Insert(_disc2);
            _discCollection.Insert(_disc3);
            _discCollection.Insert(_disc4);
        }

        [Test]
        public void Get()
        {
            // Act
            var discList = _discController.Get().ToList();

            // Assert
            Assert.IsNotNull(discList);
            Assert.AreEqual(4, discList.Count());
            Assert.AreEqual("Disc1", discList.ElementAt(0).Name);
            Assert.AreEqual("Artist2", discList.ElementAt(1).Artist);
            Assert.AreEqual("Gener1", discList.ElementAt(2).Gener);
            Assert.AreEqual("Year2", discList.ElementAt(3).Year);
        }

        [Test]
        public void GetById()
        {
            // Act
            var disc = _discController.Get(_disc1.Id);

            // Assert
            Assert.IsNotNull(disc);
            Assert.AreEqual(_disc1.Id, disc.Id);
            Assert.AreEqual(_disc1.Name, disc.Name);
            Assert.AreEqual(_disc1.Artist, disc.Artist);
            Assert.AreEqual(_disc1.Gener, disc.Gener);
            Assert.AreEqual(_disc1.Year, disc.Year);
        }

        [Test]
        public void GetById_MissingDisc()
        {
            //Arrange
            var disc = new Disc();

            // Act and assert
            Assert.Throws<HttpResponseException>(() => _discController.Get(disc.Id));
        }
        
        [Test]
        public void Post()
        {
            // Arrange
            var disc5 = new Disc { Name = "Disc5", Artist = "Artist3", Gener = "Gener2", Year = "Year3" };

            // Act
            _discController.Post(disc5);

            // Assert
            var disc = _discCollection.FindOneById(disc5.Id);
            Assert.IsNotNull(disc);
            Assert.AreEqual(disc5.Name, disc.Name);
            Assert.AreEqual(disc5.Artist, disc.Artist);
            Assert.AreEqual(disc5.Gener, disc.Gener);
            Assert.AreEqual(disc5.Year, disc.Year);
        }

        [Test]
        public void Post_ExistingDisc()
        {
            // Arrange
            long totalDiscsBeforePost = _discCollection.FindAll().Count();

            // Act and assert
            Assert.Throws<HttpResponseException>(() => _discController.Post(_disc1));
            Assert.AreEqual(totalDiscsBeforePost, _discCollection.FindAll().Count());
        }

        [Test]
        public void Post_DiscConflicting()
        {
            // Arrange
            var disc6 = new Disc { Name = "Disc4", Artist = "Artist3", Gener = "Gener3", Year = "Year2" };

            // Act and assert
            Assert.Throws<HttpResponseException>(() => _discController.Post(disc6));
            Assert.IsNull(_discCollection.FindOneById(disc6.Id));
        }

        [Test]
        public void Put()
        {
            // Arrange
            _disc1.Gener = "Gener3";

            // Act
            _discController.Put(_disc1);

            // Assert
            Assert.AreEqual("Gener3", _discCollection.FindOneById(_disc1.Id).Gener);
        }

        [Test]
        public void Put_MissingDisc()
        {
            // Arrange
            var disc5 = new Disc { Name = "Disc5", Artist = "Artist3", Gener = "Gener2", Year = "Year3" };

            // Act and assert
            Assert.Throws<HttpResponseException>(() => _discController.Put(disc5));
            Assert.IsNull(_discCollection.FindOneById(disc5.Id));
        }

        [Test]
        public void Put_DiscConflicting()
        {
            // Arrange
            _disc3.Name = _disc1.Name;
            _disc3.Artist = _disc1.Artist;
            _disc3.Year = _disc1.Year;

            // Act and assert
            Assert.Throws<HttpResponseException>(() => _discController.Put(_disc3));
            _disc3 = _discCollection.FindOneById(_disc3.Id);
            Assert.AreNotEqual(_disc1.Name, _disc3.Name);
        }

        [Test]
        public void Delete()
        {
            // Act
            _discController.Delete(_disc2.Id);

            // Assert
            Assert.IsNull(_discCollection.FindOneById(_disc2.Id));
        }
    }
}
