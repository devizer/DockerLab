using System;
using System.Collections.Generic;
using System.Text;

namespace TheApp
{

    // ValidStatus=200,403; Uri=http://mywebapi:80/get-status; Method=POST; *Accept=application/json, text/javascript; Payload={'verbosity':'normal'}"
    public class ConnectionStringParser
    {
        public readonly string ConnectionString;

        public ConnectionStringParser(string connectionString)
        {
            ConnectionString = connectionString;
        }




    }
}
