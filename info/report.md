WaitFor's final report:
 Ping ........... : mongodb
 Status ......... : OK. 1.15 msecs (at the 3rd second, 1st try)

 Ping ........... : mysql
 Status ......... : OK. 1.37 msecs (at the 3rd second, 1st try)

 Ping ........... : mysql_57
 Status ......... : OK. 1.37 msecs (at the 3rd second, 1st try)

 Ping ........... : nginx2
 Status ......... : OK. 1.36 msecs (at the 3rd second, 1st try)

 Ping ........... : sqlserver
 Status ......... : OK. 1.37 msecs (at the 3rd second, 1st try)

 Ping ........... : redis
 Status ......... : OK. 1.40 msecs (at the 3rd second, 1st try)

 Ping ........... : nginx1
 Status ......... : OK. 1.39 msecs (at the 3rd second, 1st try)

 HttpGet ........ : http://adminer:8080
 Status ......... : OK (200). Server: N/A. 4577 bytes recieved (at the 5th second, 1st try)

 HttpGet ........ : http://nginx1
 Status ......... : OK (200). Server: nginx/1.15.12. 612 bytes recieved (at the 5th second, 1st try)

 HttpGet ........ : http://nginx2
 Status ......... : OK (200). Server: nginx/1.15.12. 612 bytes recieved (at the 5th second, 1st try)

 Ping ........... : google.com
 Status ......... : OK. 3.59 msecs (at the 5th second, 1st try)

 Memcached ...... : memcached:11211
 Version ........ : 1.5.14 [64 bits] (at the 14th second, 1st try)

 HttpGet ........ : https://docs.oracle.com
 Status ......... : OK (200). Server: AkamaiNetStorage. 485 bytes recieved (at the 15th second, 1st try)

 HttpGet ........ : https://google.com/404
 Status ......... : NotFound (404). Server: N/A. 1564 bytes recieved (at the 15th second, 1st try)

 Redis .......... : redis:6379
 Version ........ : 5.0.4 (Standalone) (at the 15th second, 1st try)

 Redis .......... : redis_another:6379
 Version ........ : 3.2.12 (Standalone) (at the 15th second, 1st try)

 HttpGet ........ : https://portal.azure.com
 Status ......... : OK (200). Server: N/A. 33365 bytes recieved (at the 15th second, 1st try)

 HttpGet ........ : https://docs.microsoft.com
 Status ......... : OK (200). Server: N/A. 26931 bytes recieved (at the 15th second, 1st try)

 HttpGet ........ : https://google.com
 Status ......... : OK (200). Server: gws. 46055 bytes recieved (at the 16th second, 1st try)

 HttpGet ........ : http://hellorest/api/v1/ping
 Status ......... : OK (200). Server: Kestrel. 6 bytes recieved (at the 16th second, 2nd try)

 MySQL .......... : Server = mysql; Port=3306; Uid = root; Pwd = example; Connect Timeout = 5; Pooling=false;
 Version ........ : 5.5.62 (at the 17th second, 2nd try)

 MySQL .......... : Server = mysql_57; Port=3306; Uid = root; Pwd = example; Connect Timeout = 5; Pooling=false;
 Version ........ : 5.7.26 (at the 17th second, 5th try)

 RabbitMQ ....... : amqp://rabbitmq:5672
 Version ........ : 3.4.4 (at the 18th second, 7th try)

 MongoDB ........ : mongodb://mongodb:27017
 Version ........ : 3.4.20 (at the 19th second, 1st try)

 Postgres ....... : Host = postgres; Port=5432; User ID=postgres; Password=postgres; Database=postgres; Timeout = 5; Pooling=false;
 Version ........ : PostgreSQL 9.4.22 on x86_64-pc-linux-gnu (Debian 9.4.22-1.pgdg90+1), compiled by gcc (Debian 6.3.0-18+deb9u1) 6.3.0 20170516, 64-bit (at the 20th second, 2nd try)

 MSSQL .......... : Data Source=sqlserver; User ID=sa; Password=~1qazxsw2; Timeout = 5; Pooling=false;
 Version ........ : 14.0.3048.4 [Developer Edition (64-bit)] (at the 37th second, 6th try)

 Oracle ......... : DATA SOURCE=oracle11xe; USER ID=system; PASSWORD=oracle;CONNECTION TIMEOUT=3
 Version ........ : Oracle Database 11g Express Edition Release 11.2.0.2.0 - 64bit Production (at the 45th second, 25th try)

 Cassandra ...... : Contact Points = cassandra; Port = 9042
 Version ........ : Cassandra: 2.1.21; CQL: 3.2.1; Protocol: 3; Thrift: 19.39.0 (at the 47th second, 33rd try)

Total 28 of 28 dependencies are alive. Mem: 98 Mb
