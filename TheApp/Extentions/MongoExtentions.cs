namespace MongoDB.Profiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bson.Serialization;
    using Bson.Serialization.Attributes;

    using MongoDB.Bson;
    using MongoDB.Driver;

    public static class MongoExtentions
    {
        private const string SystemProfileCollectionName = "system.profile";



        public static ICollection<string> GetSupportedStorageEngines(this MongoClient client)
        {
            var db = client.GetDatabase("local");
            var list = db.GetCollection<StartupLogInfo>("startup_log");
            var startInfo = list
                .AsQueryable()
                .OrderByDescending(x => x.StartTime)
                .FirstOrDefault();

            var vers = startInfo?.BuildInfo?.StorageEngines;
            if (vers != null)
                return vers;

            return new List<string>() {"N/A"};
        }

        public static ICollection<DatabaseInfo> GetDatabasesInfo(this MongoClient client)
        {
            var list = client.ListDatabases().ToList();
            List<DatabaseInfo> ret = new List<DatabaseInfo>();
            foreach (var dbDoc in list)
            {
                var dbInfo = BsonSerializer.Deserialize<DatabaseInfo>(dbDoc);
                ret.Add(dbInfo);
            }

            return ret;
        }

        public static bool CheckSystemProfileLimit(this IMongoDatabase db, long size)
        {
            CollectionInfo prev = GetCollectionsInfo(db, SystemProfileCollectionName);
            ProfilingStatus status = null;
            if (prev == null || !prev.IsCapped || prev.Size < size)
            {
                if (prev != null)
                {
                    status = GetProfilingStatus(db);
                    SetProfilingLevel(db, new ProfilingStatus() {Level = ProfilingLevel.None});
                    db.DropCollection(name: SystemProfileCollectionName);

                }

                db.CreateCollection(SystemProfileCollectionName, new CreateCollectionOptions()
                {
                    Capped = true,
                    MaxSize = size
                });

                if (status != null)
                    SetProfilingLevel(db, status);


                return true;
            }

            return false;
        }

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

        public static ProfilingStatus GetProfilingStatus(this IMongoDatabase db)
        {
            var raw = db.RunCommand<dynamic>(new BsonDocument("profile", -1));
            return new ProfilingStatus()
            {
                Level = (ProfilingLevel) (int) raw.was,
                ThresholdMs = raw.slowms,
            };
        }

        public static void SetProfilingLevel(this IMongoDatabase db, ProfilingStatus status)
        {
            var thresholdMs = status.ThresholdMs;
            if (thresholdMs <= 0)
                thresholdMs = GetProfilingStatus(db).ThresholdMs;

            var req = new BsonDocument
            {
                {"profile", (int) status.Level},
                {"slowms", (int) thresholdMs},
            };

            var raw = db.RunCommand<dynamic>(req).was;
        }

        public static ICollection<CollectionInfo> GetCollections(this IMongoDatabase db)
        {
            FilterDefinition<BsonDocument> f = null;
            return GetCollectionsInfo(db, f);
        }

        public static CollectionInfo GetCollectionsInfo(this IMongoDatabase db, string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");


            FilterDefinition<BsonDocument> f = new BsonDocumentFilterDefinition<BsonDocument>(new BsonDocument()
            {
                {"name", name}
            });

            return GetCollectionsInfo(db, f).FirstOrDefault();
        }

        private static ICollection<CollectionInfo> GetCollectionsInfo(IMongoDatabase db, FilterDefinition<BsonDocument> f)
        {
            var cols = db.ListCollections(new ListCollectionsOptions() {Filter = f});
            var list = cols.ToList().OfType<BsonDocument>();
            List<CollectionInfo> ret = new List<CollectionInfo>();
            foreach (var i in list)
            {
                var collectionInfo = ParseCollectionInfo(i);
                if (collectionInfo != null)
                    ret.Add(collectionInfo);
            }
            return ret;
        }

        public static CollectionInfo ParseCollectionInfo(BsonDocument raw)
        {
            var name = raw["name"];
            if (name != null)
            {
                var collectionInfo = new CollectionInfo() {FullName = name.AsString};
                if (raw.Contains("options"))
                {
                    var options = raw["options"].AsBsonDocument;
                    if (options.Contains("size"))
                    {
                        collectionInfo.Size = options["size"].AsInt32;
                        collectionInfo.HasSize = true;
                    }

                    if (options.Contains("capped"))
                        collectionInfo.IsCapped = options["capped"].AsBoolean;
                }

                return collectionInfo;
            }

            return null;
        }

        public static string GetOsTypeAsString(this IMongoDatabase db)
        {
            var raw = db.RunCommand<BsonDocument>(new BsonDocument("hostInfo", 1));
            var os = raw["os"];
            if (os != null)
            {
                var type = os.AsBsonDocument["type"];
                if (type != null)
                    return type.AsString;
            }

            return "Some OS";
        }

        public static HostInfo GetHostInfo(this IMongoDatabase db)
        {
            HostInfo raw = db.RunCommand<HostInfo>(new BsonDocument("hostInfo", 1));
            return raw;
        }



    }

    public enum ProfilingLevel
    {
        None = 0,
        SlowOnly = 1,
        All = 2
    }

    public class ProfilingStatus
    {
        public ProfilingLevel Level { get; set; }

        public int ThresholdMs { get; set; }

        public override string ToString()
        {
            return $"{{Level {Level}, threshold {ThresholdMs} ms}}";
        }

    }

    public class CollectionInfo
    {
        public string FullName { get; set; }
        public bool IsCapped { get; set; }
        public long Size { get; set; }
        public bool HasSize { get; set; }

        public override string ToString()
        {
            return $"{{{FullName}, IsCapped: {IsCapped}, Size: {(HasSize ? Size.ToString() : "N/A")}}}";
        }
    }

    [BsonIgnoreExtraElements(true)]
    public class DatabaseInfo
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("sizeOnDisk")]
        public decimal? Size { get; set; }

        [BsonElement("empty")]
        public bool? IsEmptyRaw { get; set; }

        public bool IsEmpty
        {
            get { return IsEmptyRaw.HasValue && IsEmptyRaw.Value; }
        }

        public override string ToString()
        {
            return $"{{DB: {Name}, Size: {Size}, IsEmpty: {IsEmpty}}}";
        }
    }

    [BsonIgnoreExtraElements(true)]
    public class StartupLogInfo
    {
        [BsonElement("startTime")]
        public DateTime StartTime { get; set; }

        [BsonElement("buildinfo")]
        public BuildInfo BuildInfo { get; set; }
    }

    [BsonIgnoreExtraElements(true)]
    public class BuildInfo
    {
        [BsonElement("storageEngines")]
        public List<string> StorageEngines { get; set; }
    }
}