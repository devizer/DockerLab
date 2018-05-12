using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using MongoDB.Profiler;
using NDesk.Options;
using TheApp.Extentions;
using Universe;
using WaitFor;

namespace TheApp
{
    class WaitForProgram
    {

        public static WaitModel Model = new WaitModel();
        private static string EnvPrefix = "WAIT_FOR_";
        static readonly object SyncWrite = new object();
        private static readonly int CaptionWidth = 16;



        public static int WaitFor_Impl(string[] args)
        {
            bool needHelp = false, needLogo = false, needVer = false;
            var appVer = Assembly.GetEntryAssembly().GetName().Version.ToString();
            var appVerShort = appVer;
            var date = AssemblyBuildDateTimeAttribute.CallerUtcBuildDate;
            if (date.HasValue && date.Value.Year > 2000)
                appVer += $" (built at {date.Value})";

            int timeout = -1;
            var p = new OptionSet(StringComparer.InvariantCultureIgnoreCase)
            {
                {"Timeout=", "Timeout in seconds", v => Int32.TryParse(v, out timeout)},
                {"MySQL=", "MySQL connection", v => Add(ConnectionFamily.MySQL, v)},
                {"MSSQL=", "MS SQL connection", v => Add(ConnectionFamily.MSSQL, v)},
                {"PostgreSQL=", "PostgreSQL connection", v => Add(ConnectionFamily.Postgres, v)},
                {"Oracle=", "Oracle connection", v => Add(ConnectionFamily.Oracle, v)},
                {"RabbitMQ=", "RabbitMQ connection", v => Add(ConnectionFamily.RabbitMQ, v)},
                {"Cassandra=", "Cassandra connection", v => Add(ConnectionFamily.Cassandra, v)},
                {"MongoDB=", "MongoDB Connection", v => Add(ConnectionFamily.MongoDB, v)},
                {"Redis=", "Redis Connection", v => Add(ConnectionFamily.Redis, v)},
                {"Memcached=", "Memcached Connection", v => Add(ConnectionFamily.Memcached, v)},
                {"Ping=", "Host or ip address", v => Add(ConnectionFamily.Ping, v)},
                {"HttpGet=", "https(s)://host:port/path", v => Add(ConnectionFamily.HttpGet, v)},
                {"EnvPrefix=", "default is WAIT_FOR_", v => EnvPrefix = string.IsNullOrWhiteSpace(v) ? EnvPrefix : v.Trim()},
                {"v|Version", "Show version", v => needVer = true},
                {"h|?|Help", "Display this help", v => needHelp = v != null},
                {"n|nologo", "Hide logo", v => needLogo = v != null}
            };

            p.Parse(args);
            Model.Timeout = timeout;
            PopulateTargetsByEnv();

            if (needVer)
            {
                Console.WriteLine(appVerShort);
                return 0;
            }

            if (!needLogo)
            {
                Console.WriteLine($"WaitFor {appVer} is living in docker {Environment.NewLine}");
            }

            if (needHelp)
            {
                StringBuilder b = new StringBuilder();
                p.WriteOptionDescriptions(new StringWriter(b));
                Console.WriteLine(b);
                return 0;
            }

            StringBuilder startup = new StringBuilder("Waiting for network services:");
            foreach (var m in Model.Connections.OrderBy(x => x.Family.ToString()))
            {
                startup.AppendLine();
                startup.Append(Format(m.Family.ToString(), m.ConnectionString));
            }

            Console.WriteLine(startup + Environment.NewLine);

            CountdownEvent done = new CountdownEvent(Model.Connections.Count);
            int delay = 0;
            const int delayIncrement = 111;
            var ordredByHavy = Model.Connections.OrderBy(x => x.Family.IsHavy()).ToList();
            foreach (ConnectionInfo infoCopy in ordredByHavy)
            {
                int delayLocal = delay;
                var info = infoCopy;
                if (info.Family.IsHavy()) delay += delayIncrement;
                Thread thread = new Thread(() =>
                {
                    Thread.Sleep(delayLocal);
                    while (true)
                    {
                        try
                        {
                            info.TryNumber++;
                            info.Version = SimpleVersionInfo.GetVersion(info.Family, info.ConnectionString);
                            info.Exception = null;
                            info.OkTime = Model.StartAt.ElapsedMilliseconds / 1000m;
                            info.IsOk = true;
                            InformNewStatus(info);
                            break;
                        }
                        catch (Exception ex)
                        {
                            var exeptionDigest = ex.GetExeptionDigest();
                            // MongoDB exception includes all the stack trace in the Message ;(
                            if (info.Family == ConnectionFamily.MongoDB)
                                exeptionDigest = exeptionDigest.Split('\r', '\n').Select(x => x.Trim()).FirstOrDefault(x => x.Length > 0);

                            info.Exception = exeptionDigest;
                            Debug.WriteLine($"Dependency {info.Family} does not respond to requests{Environment.NewLine + ex}");
                        }

                        if (Model.StartAt.Elapsed > TimeSpan.FromSeconds(Model.Timeout))
                        {
                            InformNewStatus(info);
                            break;
                        }

                        Thread.Sleep(1000);
                    }

                    done.Signal();

                }) { IsBackground = true };

                thread.Start();
            }

            if (Model.Timeout > 0)
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    Func<bool> isProgress = () => !done.WaitHandle.WaitOne(0);
                    while (isProgress())
                    {
                        Thread.Sleep(3000);
                        var secs = (int) Model.StartAt.Elapsed.TotalSeconds;
                        var stillWaiting = Model.Clone().Connections.Where(x => !x.IsOk).Select(x => x.Family.ToString());
                        if (isProgress())
                            Console.WriteLine($"Waiting for dependencies ({secs} / {Model.Timeout}): " + string.Join(", ", stillWaiting));
                    }
                });

            done.Wait();

            var sorted = Model.Connections
                .OrderBy(x => !x.IsOk)
                .ThenBy(x => x.OkTime)
                .ThenBy(x => x.Family)
                .ThenBy(x => x.ConnectionString);

            Console.WriteLine("WaitFor's final report:");
            foreach (var item in sorted)
            {
                WriteItemReport(item);
            }

            Console.WriteLine("Total {0} of {1} dependencies are alive. Mem: {2} Mb",
                Model.Connections.Count(x => x.IsOk),
                Model.Connections.Count,
                Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024
            );


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

        static void PopulateTargetsByEnv()
        {
            var families = Enum.GetValues(typeof(ConnectionFamily)).Cast<ConnectionFamily>().ToArray();
            IDictionary all = Environment.GetEnvironmentVariables();
            var allKeys = all.Keys.OfType<object>().ToArray();
            foreach (object keyRaw in allKeys)
            {
                var key = Convert.ToString(keyRaw);
                // if (Debugger.IsAttached && key.ToLower().IndexOf("google") >= 0) Debugger.Break();
                var isIt = key.StartsWith(EnvPrefix, StringComparison.InvariantCultureIgnoreCase);
                // Console.WriteLine($"{key}: {isIt}");
                if (!isIt)
                    continue;

                foreach (var fam in families)
                {
                    var isItFamily = key.StartsWith(EnvPrefix + fam, StringComparison.InvariantCultureIgnoreCase);
                    // Console.WriteLine($"  - {fam}: {isItFamily}");

                    if (isItFamily)
                    {
                        Add(fam, Convert.ToString(all[key]));
                    }
                }
            }
        }


        private static void InformNewStatus(ConnectionInfo item)
        {
            lock (SyncWrite)
            {
                string title = item.IsOk ? "Progress moved forward. Dependency is Ready:" : "Unable to connect to the dependency ;(";
                Console.WriteLine(title);
                WriteItemReport(item);
            }
        }

        private static void WriteItemReport(ConnectionInfo item)
        {
            const int captionLength = 16;
            var fore = Console.ForegroundColor;

            string caption2 = item.Family == ConnectionFamily.Ping || item.Family == ConnectionFamily.HttpGet ? "Status" : "Version";
            if (item.Exception != null) caption2 = "Exception";
            var time = string.Format(" (at the {0} second, {1} try)", 
                OrdinalNumbers.AddOrdinal((int) Math.Ceiling(item.OkTime)),
                OrdinalNumbers.AddOrdinal(item.TryNumber)
                );
            
            var lines = new[]
            {
                new
                {
                    t1 = item.Family + " ",
                    c1 = ConsoleColor.Yellow,
                    t2 = item.ConnectionString,
                    c2 = fore,
                    c3 = fore,
                },
                new
                {
                    t1 = caption2 + " ",
                    c1 = !item.IsOk ? ConsoleColor.Red : fore,
                    t2 = item.IsOk ? (item.Version + time) : item.Exception,
                    c2 = item.IsOk ? ConsoleColor.Green : ConsoleColor.Red,
                    c3 = item.IsOk ? fore : ConsoleColor.Red
                }
            };

            int n = 0;
            foreach (var line in lines)
            {
                bool isLast = (++n) == lines.Length;
                Console.Write(" ");
                Console.ForegroundColor = line.c1;
                Console.Write(line.t1);
                Console.ForegroundColor = line.c3;
                int padding = captionLength - line.t1.Length;
                if (padding >= 1)
                    Console.Write(new string('.', padding) + " : ");
                else
                    Console.Write(" : ");

                Console.ForegroundColor = line.c2;
                Console.Write(line.t2);
                Console.ForegroundColor = fore;
                Console.WriteLine(isLast ? Environment.NewLine : "");
            }

        }

        static string Format(string caption, string value)
        {
            return " " + (caption + " ").PadRight(CaptionWidth, '.') + " : " + value;
        }
    }
}

