using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using MongoDB.Profiler;
using NDesk.Options;

namespace TheApp
{
    class Program
    {

        public static WaitModel Model = new WaitModel();
        private static string EnvPrefix = "WAIT_FOR_";

        static int Main(string[] args)
        {
            return Main_Impl(args);
        }

        private static int Main_Impl(string[] args)
        {
            bool needHelp = false, needLogo = false, needVer = false;
            var appVer = Assembly.GetEntryAssembly().GetName().Version.ToString();
            int timeout = -1;
            var p = new OptionSet(StringComparer.InvariantCultureIgnoreCase)
            {
                {"Timeout=", "Timeout in seconds", v => Int32.TryParse(v, out timeout)},
                {"MySQL=", "MySQL connection", v => Add(ConnectionFamily.MySQL, v)},
                {"MSSQL=", "MS SQL connection", v => Add(ConnectionFamily.MSSQL, v)},
                {"PostgreSQL=", "PostgreSQL connection", v => Add(ConnectionFamily.Postgres, v)},
                {"RabbitMQ=", "RabbitMQ connection", v => Add(ConnectionFamily.RabbitMQ, v)},
                {"MongoDB=", "MongoDB Connection", v => Add(ConnectionFamily.MongoDB, v)},
                {"Redis=", "Redis Connection", v => Add(ConnectionFamily.Redis, v)},
                {"v|Version", "Show version", v => needVer = true},
                {"h|?|Help", "Display this help", v => needHelp = v != null},
                {"n|nologo", "Hide logo", v => needLogo = v != null}
            };


            p.Parse(args);
            Model.Timeout = timeout;

            if (needVer)
            {
                Console.WriteLine(appVer);
                return 0;
            }

            if (!needLogo)
            {
                Console.WriteLine($"WaitFor {appVer} is living in docker");
                Console.WriteLine();
            }

            if (needHelp)
            {
                p.WriteOptionDescriptions(Console.Out);
                return 0;
            }

            // Wait_Prev_Implementation();

            StringBuilder startup = new StringBuilder("Waiting for network services:");
            foreach (var m in Model.Connections)
            {
                startup.AppendLine();
                startup.Append(Format(m.Family.ToString(), m.ConnectionString));
            }

            Console.WriteLine(startup + Environment.NewLine);
            PopulateTargetsByEnv();

            CountdownEvent done = new CountdownEvent(Model.Connections.Count);
            foreach (ConnectionInfo infoCopy in Model.Connections)
            {
                var info = infoCopy;
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    while (true)
                    {
                        try
                        {
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
                        }

                        if (Model.StartAt.Elapsed > TimeSpan.FromSeconds(Model.Timeout))
                        {
                            InformNewStatus(info);
                            break;
                        }

                        Thread.Sleep(1000);
                    }

                    done.Signal();
                });
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

            StringBuilder final = new StringBuilder("WaitFor final report:");
            foreach (var item in Model.Connections)
            {
                var line1 = Format(item.Family.ToString(), $"{item.ConnectionString}");
                var line2 = Format("Version", item.Version + $" (in {item.OkTime.ToString("f1")} secs)");
                if (item.Exception != null && item.Version == null)
                    line2 = Format("Exception", item.Exception);

                final.AppendFormat("{0}{1}{0}{2}{0}", Environment.NewLine, line1, line2);
            }

            Console.WriteLine(final + Environment.NewLine);

            return 0;
        }

        static void PopulateTargetsByEnv()
        {
            var families = Enum.GetValues(typeof(ConnectionFamily)).Cast<ConnectionFamily>().ToArray();
            IDictionary all = Environment.GetEnvironmentVariables();
            foreach (object keyRaw in all.Keys)
            {
                var key = Convert.ToString(keyRaw);
                if (!key.StartsWith(EnvPrefix, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                foreach (var fam in families)
                {
                    if (key.StartsWith(EnvPrefix + fam, StringComparison.CurrentCultureIgnoreCase))
                    {
                        Add(fam, Convert.ToString(all[key]));
                    }
                }
            }
        }

        private static void InformNewStatus(ConnectionInfo item)
        {
            var line1 = Format(item.Family.ToString(), $"{item.ConnectionString}");
            string time = new DateTime(0).AddSeconds((double)item.OkTime).ToString("HH:mm:ss.f");
            var line2 = Format("Version", item.Version + $" (in {item.OkTime.ToString("f1")} secs)");
            if (item.Exception != null && item.Version == null)
                line2 = Format("Exception", item.Exception);

            string title = item.IsOk ? "Dependency is Ready!" : "Unable to connect to the dependency ;(";
            Console.WriteLine("{0}{3}{0}{1}{0}{2}{0}", Environment.NewLine, line1, line2, title);
        }

        private static void Wait_Prev_Implementation()
        {
            var timeout = Model.Timeout;
            if (timeout > 0)
            {
                for (int i = 1; i <= timeout; i++)
                {
                    Console.WriteLine("Sleeping " + i + "/" + timeout);
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

        static string Format(string caption, string value)
        {
            return " " + (caption + " ").PadRight(16, '.') + " : " + value;
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
