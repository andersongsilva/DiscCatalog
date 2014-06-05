using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DiscCatalog.Models
{
    public class Disc
    {
        public Disc()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }

        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Artist { get; set; }
        public string Gener { get; set; }
        public string Year { get; set; }
    }
}