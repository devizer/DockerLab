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
                return WaitForProgram.Main_Impl(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ooops. Something gone wrong. Full Exception details:{0}{1}{0}{0}.Short Problem description: {2}",
                    Environment.NewLine,
                    ex,
                    ex.GetExeptionDigest());

                return 666;
            }
        }
    }
}