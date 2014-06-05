using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using DiscCatalog.Models;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace DiscCatalog.Controllers
{
    public class DiscController : ApiController
    {
        private readonly MongoDatabase _mongoDatabase;

        public DiscController()
        {
            _mongoDatabase = new MongoDBHandler<Disc>().Database;
        }

        public DiscController(MongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        private MongoCollection<Disc> GetCollection()
        {
            var collectionName = (string)ControllerContext.RouteData.Values["collectionName"];
            return _mongoDatabase.GetCollection<Disc>(collectionName);
        }

        public IEnumerable<Disc> Get()
        {
            return GetCollection().FindAll().AsEnumerable();
        }

        public Disc Get(string id)
        {
            //var disc = GetCollection().FindOne(Query<Disc>.EQ(d => d.Id, id));
            var disc = GetCollection().FindOneById(id);
            if (disc == null) throw new HttpResponseException(HttpStatusCode.NotFound);
            return disc;
        }

        public void Post([FromBody] Disc disc)
        {
            var query = Query.Or(
                Query<Disc>.EQ(d => d.Id, disc.Id),
                Query.And(
                    Query<Disc>.EQ(d => d.Name, disc.Name),
                    Query<Disc>.EQ(d => d.Artist, disc.Artist),
                    Query<Disc>.EQ(d => d.Year, disc.Year)
                    )
                );

            if (GetCollection().FindOne(query) != null)
                throw new HttpResponseException(HttpStatusCode.Conflict);

            GetCollection().Insert(disc);
        }

        public void Put([FromBody] Disc disc)
        {
            //var discToUpdate = GetCollection().FindOne(Query<Disc>.EQ(d => d.Id, disc.Id));
            var discToUpdate = GetCollection().FindOneById(disc.Id);
            if (discToUpdate == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var query = Query.And(
                Query<Disc>.NE(d => d.Id, disc.Id),
                Query<Disc>.EQ(d => d.Name, disc.Name),
                Query<Disc>.EQ(d => d.Artist, disc.Artist),
                Query<Disc>.EQ(d => d.Year, disc.Year)
                );

            if (GetCollection().FindOne(query) != null)
                throw new HttpResponseException(HttpStatusCode.Conflict);

            GetCollection().Save(disc);
        }

        public void Delete(string id)
        {
            GetCollection().Remove(Query<Disc>.EQ(d => d.Id, id));
        }
    }
}