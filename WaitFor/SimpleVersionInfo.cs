using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using Dapper;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Driver;
using MongoDB.Profiler;
using MySql.Data.MySqlClient;
using Npgsql;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace TheApp
{
    class SimpleVersionInfo
    {
        public static string GetVersion(ConnectionFamily family, string connectionString)
        {

            connectionString = connectionString.Trim();
            switch (family)
            {
                case ConnectionFamily.MySQL:
                    return GoMySQL(connectionString);

                case ConnectionFamily.Redis:
                    return GoRedis(connectionString);

                case ConnectionFamily.MSSQL:
                    return GoMSSQL(connectionString);

                case ConnectionFamily.MongoDB:
                    return GoMongoDB(connectionString);

                case ConnectionFamily.Postgres:
                    return GoPostgreSQL(connectionString);

                case ConnectionFamily.RabbitMQ:
                    return GoRabbitMQ(connectionString);

                case ConnectionFamily.Ping:
                    return GoPing(connectionString);

                case ConnectionFamily.HttpGet:
                    return GoHttpsGet(connectionString);

                case ConnectionFamily.Memcached:
                    return GoMemcached(connectionString);

                default:
                    throw new ArgumentOutOfRangeException($"Family {family} is not valid argument");

            }
        }

        private static string GoHttpsGet(string connectionString)
        {
            Stopwatch startAt = Stopwatch.StartNew();
            HttpClient c = new HttpClient();
            var response = c.GetAsync(connectionString).Result;
            var statusCode = response.StatusCode;
            IEnumerable<string> values;
            var server =
                response.Headers.TryGetValues("server", out values)
                    ? values.FirstOrDefault()
                    : "N/A";
            
            var bytes = response.Content.ReadAsByteArrayAsync().Result;
            return $"{statusCode} ({(int)statusCode}). Server: {server}. {bytes.Length} bytes recieved";
        }

        private static string GoHttpsGet_Strict(string connectionString)
        {
            HttpClient c = new HttpClient();
            var bytes = c.GetByteArrayAsync(connectionString).Result;
            return $"OK. {bytes.Length} bytes recieved";
        }

        private static string GoPing(string connectionString)
        {
            byte[] buffer = Enumerable.Repeat((byte)42, 32).ToArray();

            PingOptions options = new PingOptions(64, false);

            try
            {
                Ping p = new Ping();
                p.Send("localhost", 2, buffer, options);
            }
            catch
            {
            }

            int timeout = 2000;

            Ping pingSender = new Ping();

            Stopwatch sw = Stopwatch.StartNew();
            pingSender.Send(connectionString, timeout, buffer, options);
            var msecs = sw.ElapsedTicks / Convert.ToDecimal(Stopwatch.Frequency);
            return $"OK. {msecs:f2} msecs";
        }


        private static string GoMSSQL(string cs)
        {
            cs = cs.Trim();
            string ver;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string sql = "Select Cast(ServerProperty('ProductVersion') as nvarchar) + ' [' + Cast(ServerProperty('Edition') as nvarchar) + ']';";
                ver = con.ExecuteScalar<string>(sql);
            }

            return ver;
        }

        private static string GoRedis(string cs)
        {
            cs = cs.Trim();

            var configOptions = new ConfigurationOptions()
            {
                EndPoints = {cs},
                AbortOnConnectFail = false,
            };

            var con = ConnectionMultiplexer.Connect(configOptions);
            var db = con.GetDatabase();
            var key = "Ping " + Guid.NewGuid();
            db.StringGet(key);

            var first = cs.Split(',')[0].Split(':');
            string host = first[0];
            int port = 6379;
            if (first.Length == 2)
            {
                host = first[0];
                port = int.Parse(first[1]);
            }

            var server = con.GetServer(host, port);
            var ver = $"{server.Version} ({server.ServerType})";
            return ver;


        }

        private static string GoMongoDB(string uri)
        {
            var client = new MongoClient(uri.Trim());
            var db = client.GetDatabase("admin");
            db.Ping();
            var ver = db.GetMongoServerVersionAsString();
            return ver;
        }

        private static string GoRabbitMQ(string s)
        {
            string ver = null;
            var factory = new ConnectionFactory() {Uri = new Uri(s.Trim())};
            using (IConnection connection = factory.CreateConnection())
            {

                IDictionary<string, object> serverProperties = connection.ServerProperties;
                object verRaw = serverProperties["version"];
                // ver = ASCIIEncoding.ASCII.GetString((verRaw as byte[]) ?? new byte[0]);
                ver = new UTF8Encoding(false).GetString((verRaw as byte[]) ?? new byte[0]);


                /*
                                foreach (string key in serverProperties.Keys)
                                {
                                    var raw = serverProperties[key] as byte[];
                                    if (raw != null)
                                    {
                                        var val = new UTF8Encoding(false).GetString(raw);
                                        Write(key, val);
                                    }
                                }
                */


                using (var channel = connection.CreateModel())
                {
                    var queue = "Temp Queue " + Guid.NewGuid().ToString("N");
                    channel.QueueDeclare(queue, false, false, true, null);
                    channel.BasicPublish("", queue, false);
                }
            }

            return ver;
        }


        private static string GoPostgreSQL(string cs)
        {
            NpgsqlConnection con = new NpgsqlConnection(cs);
            con.Open();
            var ver = con.ExecuteScalar<string>("Select Version();");
            return ver;
        }


        private static string GoMySQL(string cs)
        {
            MySqlConnection con = new MySqlConnection(cs);
            con.Open();
            var ver = con.ExecuteScalar<string>("Select Version();");
            return ver;
        }

        private static string GoMemcached(string cs)
        {

            var parts = cs.Split(':');
            var host = cs;
            int port = 11211;
            if (parts.Length == 2)
            {
                host = parts[0];
                Int32.TryParse(parts[1], out port);
            }

            Func<IPAddress, int> order = ip => ip.ToString().IndexOf(".") >= 0 ? 1 : 2;
            IPAddress address;
            if (!IPAddress.TryParse(host, out address))
            {
                IPAddress[] resolved = Dns.GetHostAddresses(host);
                var ordered = resolved.OrderBy(x => order(x)).ToArray();
                address = ordered.FirstOrDefault();
            }

            if (address == null)
                throw new ArgumentException($"Unable to resolve IP Address of {cs}");


            MemcachedClientOptions opts = new MemcachedClientOptions();
            opts.Protocol = MemcachedProtocol.Binary;
            var ipEndPoint = new IPEndPoint(address, port);
            var nullLoggerFactory = Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance;
            MemcachedClientConfiguration config = new MemcachedClientConfiguration(nullLoggerFactory, opts);
            config.Servers.Add(ipEndPoint);
            config.Protocol = MemcachedProtocol.Binary;

            // config.Authentication.Type = typeof(PlainTextAuthenticator);
            // config.Authentication.Parameters["userName"] = "demo";
            // config.Authentication.Parameters["password"] = "demo";

            var mc = new MemcachedClient(nullLoggerFactory, config);
            Stopwatch sw = Stopwatch.StartNew();
            var stats = mc.Stats();
            var rawVersion = stats.GetRaw("version");
            var rawPointerSize = stats.GetRaw("pointer_size");
            var version = rawVersion.FirstOrDefault().Value ?? "N/A";
            var pointerSize = rawPointerSize.FirstOrDefault().Value;
            if (!string.IsNullOrEmpty(pointerSize))
                version += $" [{pointerSize} bits]";

            return version;

            mc.Get($"PING_({Guid.NewGuid().ToString("N")})");
            return "OK. TryGet took " + (sw.ElapsedTicks / Convert.ToDecimal(Stopwatch.Frequency)).ToString("f2") + " msec";
        }
    }
}