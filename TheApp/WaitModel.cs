using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TheApp
{
    public enum ConnectionFamily
    {
        MSSQL,
        MySQL,
        Postgres,
        RabbitMQ,
        MongoDB,
        Redis
    }

    public class WaitModel
    {
        public Stopwatch StartAt = Stopwatch.StartNew();
        public List<ConnectionInfo> Connections = new List<ConnectionInfo>();
        readonly object Sync = new object();
        public int Timeout = 30;

        public WaitModel Clone()
        {
            lock (Sync)
                return new WaitModel()
                {
                    Connections = new List<ConnectionInfo>(Connections.Select(x => x.Clone())),
                    StartAt = StartAt,
                };
        }
    }

    public class ConnectionInfo
    {
        public ConnectionFamily Family;
        public string ConnectionString;
        public bool IsOk;
        public string Version;
        public string Exception;
        public decimal OkTime;

        public ConnectionInfo Clone()
        {
            return (ConnectionInfo) this.MemberwiseClone();
        }
    }

}
