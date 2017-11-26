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

namespace TheApp
{
    class Program
    {

        public static WaitModel Model = new WaitModel();

        static int Main(string[] args)
        {
            return Main_Impl(args);
        }

        private static int Main_Impl(string[] args)
        {
            bool help = false, nologo = false, isVer = false;
            var appVer = Assembly.GetEntryAssembly().GetName().Version.ToString();
            int sleep = -1;
            var p = new OptionSet(StringComparer.InvariantCultureIgnoreCase)
            {
                {"Timeout=", "Timeout in seconds", v => Int32.TryParse(v, out sleep)},
                {"MySQL=", "MySQL connection", v => Add(ConnectionFamily.MySQL, v)},
                {"MSSQL=", "MS SQL connection", v => Add(ConnectionFamily.MSSQL, v)},
                {"PostgreSQL=", "PostgreSQL connection", v => Add(ConnectionFamily.Postgres, v)},
                {"RabbitMQ=", "RabbitMQ connection", v => Add(ConnectionFamily.RabbitMQ, v)},
                {"MongoDB=", "MongoDB Connection", v => Add(ConnectionFamily.MongoDB, v)},
                {"Redis=", "Redis Connection", v => Add(ConnectionFamily.Redis, v)},
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
                for (int i = 1; i <= sleep; i++)
                {
                    Console.WriteLine("Sleeping " + i + "/" + sleep);
                    Thread.Sleep(1000);
                }
            }

            foreach (ConnectionInfo info in Model.Connections)
            {
                var item = info;
                try
                {
                    var ver = SimpleVersionInfo.GetVersion(item.Family, item.ConnectionString);
                    Write(item.Family.ToString(), item.ConnectionString);
                    Write("Version", ver);
                }
                catch (Exception ex)
                {
                    Write(item.Family.ToString(), item.ConnectionString);
                    WriteError("Exception", ex.GetExeptionDigest());
                }

                Console.WriteLine();
            }

            return 0;
        }

        // Single Threaded only
        static void Add(ConnectionFamily family, string connectionString)
        {
            Model.Connections.Add(new ConnectionInfo()
            {
                Family = family,
                ConnectionString = connectionString.Trim(),
                Exception = null,
                IsOk = false,
                OkTime = 0m,
                Version = null,
            });
        }

        static void Write(string caption, string value)
        {
            Console.WriteLine(" " + (caption + " ").PadRight(16, '.') + " : " + value);
        }

        private static void WriteError(string caption, string value)
        {
            var f = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" " + (caption + " ").PadRight(16, '.') + " : " + value);
            Console.ForegroundColor = f;
        }


        static string ExpandEnv(string arg, string kind)
        {
            return Environment.ExpandEnvironmentVariables(arg);

            if (arg.StartsWith("Env:", StringComparison.InvariantCultureIgnoreCase))
            {
                var name = arg.Substring(4).Trim();
                if (String.IsNullOrEmpty(name))
                    throw new ArgumentException($"{kind} argument is wrong");

                // var ret = Environment.GetEnvironmentVariables();
            }

            throw new NotImplementedException();
        }
    }
}
