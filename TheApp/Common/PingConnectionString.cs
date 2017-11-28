using System;

namespace WaitFor.Common
{
    public class PingConnectionString : ConnectionStringParser
    {
        private Lazy<string> _Host;
        private Lazy<int> _Timeout;
        private Lazy<int> _Ttl;
        private Lazy<bool> _AllowFragment;
        private Lazy<int> _Size;


        public PingConnectionString(string connectionString) : base(connectionString)
        {
            _Host = new Lazy<string>(() =>
            {
                return Pairs.GetFirstString("Address") ?? Pairs.GetFirstWithoutKey();
            });

            _Timeout = new Lazy<int>(() =>
            {
                return Pairs.GetFirstInt("Timeout", defVal: 2000, min: 1, max: 120000);
            });

            _Ttl = new Lazy<int>(() =>
            {
                return Pairs.GetFirstInt("Ttl", defVal: 64, min: 1, max: 255);
            });

            _Size = new Lazy<int>(() =>
            {
                return Pairs.GetFirstInt("Size", defVal: 32, min: 1, max: 65535);
            });

            _AllowFragment = new Lazy<bool>(() =>
            {
                return Pairs.GetFirstBool("AllowFragment", defVal: true);
            });
        }

        public string Host => _Host.Value;

        public int Timeout => _Timeout.Value;

        public int Ttl => _Ttl.Value;

        public bool AllowFragment => _AllowFragment.Value;

        public int Size => _Size.Value;
    }
}