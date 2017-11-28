using System;
using System.Collections.Generic;
using System.Linq;

namespace WaitFor.Common
{

    // Valid Status=200,403,100-499; Uri=http://mywebapi:80/get-status; Method=POST; *Accept=application/json, text/javascript; Payload={'verbosity':'normal'}"
    public class ConnectionStringParser
    {
        public readonly string ConnectionString;
        private readonly Lazy<List<Pair>> LazyPairs;

        public class Pair
        {
            public string Key { get; set; }
            public string Value { get; set; }

            public bool HasKey => Key != null;

            public bool IsValueTrue
            {
                get
                {
                    return Value != null &&
                           ("True".Equals(Value, StringComparison.InvariantCultureIgnoreCase)
                            || "On".Equals(Value, StringComparison.InvariantCultureIgnoreCase)
                            || "Yes".Equals(Value, StringComparison.InvariantCultureIgnoreCase));
                }

            }

        }

        private ConnectionStringParser()
        {
            LazyPairs = new Lazy<List<Pair>>(() => Parse_Impl());
        }

        public ConnectionStringParser(string connectionString) : this()
        {
            ConnectionString = connectionString;
        }


        List<Pair> Parse_Impl()
        {
            List<Pair> ret = new List<Pair>();
            var vars = this.
                ConnectionString.Split(';')
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0);

            foreach (var v in vars)
            {
                string key = null, value = null;
                int p = v.IndexOf('=');
                if (p < 0)
                {
                    key = v;
                    value = v;
                }
                else
                {
                    key = p > 0 ? v.Substring(0, p) : "";
                    value = p<v.Length-1 ? v.Substring(p + 1) : "";
                }

                ret.Add(new Pair() { Key = key, Value = value});
            }

            return ret;
        }

        public List<Pair> Pairs
        {
            get { return LazyPairs.Value; }
        }
    }

    public static class ConnectionStringBuilderExtentions
    {
        const StringComparison Ignore = StringComparison.InvariantCultureIgnoreCase;

        public static bool GetFirstBool(this IEnumerable<ConnectionStringParser.Pair> pairs, string parameterName, bool defVal = false)
        {
            return pairs.FirstOrDefault(x => parameterName.Equals(x.Key, Ignore))
                       ?.IsValueTrue ?? defVal;
        }

        public static int GetFirstInt(this IEnumerable<ConnectionStringParser.Pair> pairs, string parameterName, int defVal, int min = 0, int max = Int32.MaxValue)
        {
            var raw = pairs.FirstOrDefault(x => parameterName.Equals(x.Key, Ignore))?.Value;
            int ret;
            if (raw == null || !int.TryParse(raw, out ret))
                ret = defVal;

            return Math.Max(Math.Min(ret, max), min);
        }

        public static string GetFirstString(this IEnumerable<ConnectionStringParser.Pair> pairs, string parameterName)
        {
            return pairs.FirstOrDefault(x => parameterName.Equals(x.Key, Ignore))?.Value;
        }

        public static string GetFirstWithoutKey(this IEnumerable<ConnectionStringParser.Pair> pairs)
        {
            return pairs.FirstOrDefault(x => !x.HasKey && !string.IsNullOrWhiteSpace(x.Value))?.Value;
        }
    }
}
