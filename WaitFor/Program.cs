using System;
using MongoDB.Profiler;
using Oracle.ManagedDataAccess.Client;
using TheApp;

namespace WaitFor
{
    class Program
    {
        static int Main(string[] args)
        {
            // return DebugOracle();

            try
            {
                return WaitForProgram.WaitFor_Impl(args);
            }
            catch (Exception ex)
            {
                WriteError(string.Format(
                    "Ooops. Something gone wrong. Full Exception details:{0}{1}{0}{0}.Short Problem description: {2}",
                    Environment.NewLine,
                    ex,
                    ex.GetExeptionDigest()));

                return 666;
            }
        }

        private static int DebugOracle()
        {
            var cs = "DATA SOURCE=192.168.0.18; USER ID=system; PASSWORD=oracle;CONNECTION TIMEOUT=3;";
            OracleConnectionStringBuilder bb = new OracleConnectionStringBuilder(cs);
            OracleConnection con = new OracleConnection(bb.ConnectionString);
            con.Open();
            con.Close();
            var ver = SimpleVersionInfo.GoOracle(bb.ConnectionString);
            Console.WriteLine("ORACLE Ver: " + ver);
            return 0;
        }

        private static void WriteError(string text)
        {
            var f = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ForegroundColor = f;
        }
    }
}