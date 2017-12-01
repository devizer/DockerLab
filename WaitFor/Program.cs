using System;
using MongoDB.Profiler;

namespace TheApp
{
    class Program
    {
        static int Main(string[] args)
        {
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

        private static void WriteError(string text)
        {
            var f = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ForegroundColor = f;
        }
    }
}