using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Threading;
using Dapper;
using MongoDB.Driver;
using MongoDB.Profiler;
using MySql.Data.MySqlClient;
using NDesk.Options;
using Npgsql;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace SandBoxApp
{
    class Program
    {
        static int Main(string[] args)
        {
            bool help = false, nologo = false, isVer = false;
            List<Action> actions = new List<Action>();
            var appVer = Assembly.GetEntryAssembly().GetName().Version.ToString();
            int sleep = -1;
            var p = new OptionSet(StringComparer.InvariantCultureIgnoreCase)
            {
                {"Sleep=", "Sleep in seconds", v => int.TryParse(v, out sleep)},
                {"MySQL=", "MySQL connection", v => actions.Add(() => GoMySQL(v))},
                {"MSSQL=", "MS SQL connection", v => actions.Add(() => GoMSSQL(v))},
                {"PostgreSQL=", "PostgreSQL connection", v => actions.Add(() => GoPostgreSQL(v))},
                {"RabbitMQ=", "RabbitMQ connection", v => actions.Add(() => GoRabbitMQ(v))},
                {"MongoDB=", "MongoDB Connection", v => actions.Add(() => GoMongoDB(v))},
                {"Redis=", "Redis Connection", v => actions.Add(() => GoRedis(v))},
                {"v|Version", "Show version", v => isVer = true},
                {"h|?|Help", "Display this help", v => help = v != null},
                {"n|nologo", "Hide logo", v => nologo = v != null}
            };

            p.Parse(args);

            if (isVer)
            {
                Console.WriteLine(appVer);
                return 0;
            }

            if (!nologo)
            {
                Console.WriteLine($"SandBoxApp {appVer} is living in docker");
                Console.WriteLine();
            }

            if (help)
            {
                p.WriteOptionDescriptions(Console.Out);
                return 0;
            }

            if (sleep > 0)
            {
                for (int i = 0; i < sleep - 1; i++)
                {
                    Console.WriteLine("Sleeping " + i + "/" + sleep);
                    Thread.Sleep(1000);
                }
                
            }

            foreach (var a in actions)
            {
                try
                {
                    a();
                }
                catch (Exception ex)
                {
                    Write("Exception", ex.GetExeptionDigest());

                }
                Console.WriteLine();
            }

            return 0;
        }

        private static void GoMSSQL(string cs)
        {
            cs = cs.Trim();
            Write("MS SQL", cs);
            string ver;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string sql = "Select Cast(ServerProperty('ProductVersion') as nvarchar) + ' (' + Cast(ServerProperty('Edition') as nvarchar) + ')';";
                ver = con.ExecuteScalar<string>(sql);
            }

            Write("Version", ver);
        }

        static void Write(string caption, string value)
        {
            Console.WriteLine(" " + (caption + " ").PadRight(16, '.') + " : " + value);
        }

        private static void GoRedis(string cs)
        {
            cs = cs.Trim();
            Write("Redis", cs);

            var configOptions = new ConfigurationOptions()
            {
                EndPoints = { cs },
                AbortOnConnectFail = false,
            };

            var con = ConnectionMultiplexer.Connect(configOptions);
            var db = con.GetDatabase();
            var key = "Ping " + Guid.NewGuid();
            db.StringSet(key, "sure");
            db.KeyDelete(key);

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
            Write("Version", ver);


        }

        private static void GoMongoDB(string uri)
        {
            Write("MongoDB", uri.Trim());
            var client = new MongoClient(uri.Trim());
            var db = client.GetDatabase("admin");
            db.Ping();
            var ver = db.GetMongoServerVersionAsString();
            Write("Version", ver);
        }

        private static void GoRabbitMQ(string s)
        {

            Write("RabbitMQ", s.Trim());
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

            Write("Version", ver);
        }


        private static void GoPostgreSQL(string cs)
        {
            Write("PostgreSQL", cs.Trim());
            NpgsqlConnection con = new NpgsqlConnection(ExpandEnv(cs, "PostgreSQL"));
            con.Open();
            var ver = con.ExecuteScalar<string>("Select Version();");
            Write("Version", ver);

        }


        private static void GoMySQL(string cs)
        {
            Write("MySQL", cs.Trim());
            MySqlConnection con = new MySqlConnection(ExpandEnv(cs, "MySQL"));
            con.Open();
            var ver = con.ExecuteScalar<string>("Select Version();");
            Write("Version", ver);
        }

        static string ExpandEnv(string arg, string kind)
        {
            return Environment.ExpandEnvironmentVariables(arg);

            if (arg.StartsWith("Env:", StringComparison.InvariantCultureIgnoreCase))
            {
                var name = arg.Substring(4).Trim();
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentException($"{kind} argument is wrong");

                // var ret = Environment.GetEnvironmentVariables();
            }

            throw new NotImplementedException();
        }
    }
}
