using System;
using System.Collections.Generic;
using System.Web.Http;
using DiscCatalog.Models;
using MongoDB.Driver;

namespace DiscCatalog.Controllers
{
    public class CollectionController : ApiController
    {
        private readonly MongoDatabase _database;

        public CollectionController()
        {
            _database = new MongoDBHandler<Disc>().Database;
        }

        public CollectionController(MongoDatabase database)
        {
            _database = database;
        }

        public IEnumerable<String> Get()
        {
            return _database.GetCollectionNames();
        }

        public MongoCollection<Disc> Get(string name)
        {
            return _database.GetCollection<Disc>(name);
        }

        public void Post([FromBody] string name)
        {
            _database.CreateCollection(name);
        }

        public void Put([FromBody] string oldName, string newName)
        {
            if (_database.CollectionExists(oldName))
                _database.RenameCollection(oldName, newName);
        }

        public void Delete(string name)
        {
            _database.DropCollection(name);
        }
    }
}
