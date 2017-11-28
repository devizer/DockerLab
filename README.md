## DockerLab

Here is a proof-of-concept for test-environment of an app with composed dbs (MS SQL, MySQL, PostreSQL, Redis, RabbitMQ, MongoDB)

## ./WaitFor [-Timeout=second]

It's a sandbox app which intended to check initialization of storage services. It supports native protocols of 5 kinds of storage:
* MSSQL Server
* MongoDB
* ProgreSQL
* RabbitMQ
* Redis

`./WaitFor` app supports parameters via both command line and environment variobales. 
This example will check two RDBMS server: SQLServer sqlserver1 and MySQL mysql1
```
export WAIT_FOR_MySQL="Server = mysql1; Port=3306; Uid = root; Pwd = example; Connect Timeout = 5"
./WaitFor -Timeout=60 "-MSSQL=Data Source=sqlserver1; User ID=sa; Password=~1qazxsw2; Timeout = 5"
```

## Root of this sandbox
This [stack.yml](container/stack.yml) is a compose/stack composition defenition

## report
Below is a fragment of composition above:
```
 Ping ........... : nginx
 Status ......... : OK. 0.02 msecs (at the 6th second)

 Ping ........... : google.com
 Status ......... : OK. 0.22 msecs (at the 6th second)

 HttpGet ........ : http://nginx
 Status ......... : OK (200). 612 bytes recieved (at the 7th second)

 HttpGet ........ : https://google.com/404
 Status ......... : NotFound (404). 1564 bytes recieved (at the 8th second)

 Redis .......... : redis:6379
 Version ........ : 4.0.2 (Standalone) (at the 8th second)

 HttpGet ........ : https://google.com
 Status ......... : OK (200). 46092 bytes recieved (at the 8th second)

 MongoDB ........ : mongodb://mongodb:27017
 Version ........ : 3.4.10 (at the 14th second)

 RabbitMQ ....... : amqp://rabbitmq:5672
 Version ........ : 3.4.4 (at the 33rd second)

 Postgres ....... : Host = postgres; Port=5432; User ID=postgres; Password=postgres; Database=postgres; Timeout = 5
 Version ........ : PostgreSQL 9.4.15 on x86_64-unknown-linux-gnu, compiled by gcc (Debian 4.9.2-10) 4.9.2, 64-bit (at the 41st second)

 MySQL .......... : Server = mysql_another; Port=3306; Uid = root; Pwd = example; Connect Timeout = 5
 Version ........ : 5.6.38 (at the 53rd second)

 MSSQL .......... : Data Source=sqlserver; User ID=sa; Password=~1qazxsw2; Timeout = 5
 Version ........ : 14.0.3006.16 [Developer Edition (64-bit)] (at the 63rd second)
```


## Not yet named para
Here is a sample priliminary sample output:
```
dotnet SandBoxApp.dll \
 "-MySQL=      Server = 192.168.0.8; Port=3306; Uid = mysql; Pwd = mysql; " \
 "-MSSQL=      Data Source = (local)\SQL2005; Integrated Security=true;" \
 "-PostgreSQL= Host = 192.168.0.8; Port=5432; User ID=postgres; Password=postgres; Database=postgres;" \
 "-MongoDB=    mongodb://192.168.0.8:27017" \
 "-RabbitMQ=   amqp://192.168.0.8:5672" \
 "-Redis=      192.168.0.8:6379"

 MySQL .......... : Server = 192.168.0.8; Port=3306; Uid = mysql; Pwd = mysql;
 Version ........ : 5.5.58-0+deb8u1

 MS SQL ......... : Data Source = (local)\SQL2005; Integrated Security=true;
 Version ........ : 9.00.5000.00 (Express Edition)

 PostgreSQL ..... : Host = 192.168.0.8; Port=5432; User ID=postgres; Password=postgres; Database=postgres;
 Version ........ : PostgreSQL 9.4.15 on x86_64-unknown-linux-gnu, compiled by gcc (Debian 4.9.2-10) 4.9.2, 64-bit

 MongoDB ........ : mongodb://192.168.0.8:27017
 Version ........ : 3.4.10

 RabbitMQ ....... : amqp://192.168.0.8:5672
 Version ........ : 3.6.14

 Redis .......... : 192.168.0.8:6379
 Version ........ : 3.2.11 (Standalone)
```
