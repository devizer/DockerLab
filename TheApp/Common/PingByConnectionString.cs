using System;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace WaitFor.Common
{
    public class PingByConnectionString
    {
        public static decimal Go(PingConnectionString connectionString)
        {
            var size = connectionString.Size;
            byte[] buffer = new byte [size];
            for (int i = 0; i < size; i++) buffer[i] = 42;

            PingOptions options = new PingOptions(connectionString.Ttl, connectionString.AllowFragment);
            Ping p = new Ping();
            Stopwatch sw = Stopwatch.StartNew();
            p.Send(connectionString.Host, connectionString.Timeout, buffer, options);
            var ret = sw.ElapsedTicks / Convert.ToDecimal(Stopwatch.Frequency);
            return ret;
        }
    }
}
