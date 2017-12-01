namespace MongoDB.Profiler
{

    using MongoDB.Bson;
    using MongoDB.Driver;

    public static class MongoExtentions
    {

        public static string GetMongoServerVersionAsString(this IMongoDatabase db)
        {
            var raw = db.RunCommand<dynamic>(new BsonDocument("serverStatus", 1));
            var version = (string) raw.version;
            return version;
        }

        public static void Ping(this IMongoDatabase db)
        {
            var pong = db.RunCommand<dynamic>(new BsonDocument("ping", 1)).ok;
        }

    }

}