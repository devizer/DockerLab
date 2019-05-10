## DockerLab [![Build Status](https://travis-ci.org/devizer/DockerLab.svg?branch=master)](https://travis-ci.org/devizer/DockerLab)

Here is a proof-of-concept for test-environment of an app with composed dbs (MS SQL, MySQL, PostreSQL, Redis, RabbitMQ, MongoDB)

## ./WaitFor [-Timeout=second]

It's a sandbox app which intended to check initialization of storage services. It supports native protocols of 5 kinds of storage/APIs:
* MSSQL Server
* MongoDB
* ProgreSQL
* RabbitMQ
* Redis
* Http

`./WaitFor` app supports parameters via both command line and environment variobales. 
This example will check two RDBMS server: SQLServer sqlserver1 and MySQL mysql1
```
export WAIT_FOR_MySQL="Server = mysql1; Port=3306; Uid = root; Pwd = your_password; Connect Timeout = 5"
./WaitFor -Timeout=60 "-MSSQL=Data Source=sqlserver1; User ID=sa; Password=your_password; Timeout = 5"
```

## Root of this sandbox
This [docker-compose.yml](containers/docker-compose.yml) is a compose/stack composition defenition

## report
Below is a fragment of `docker-compose up` with stack defenition above:
```
 Postgres ....... : Host = postgres; Port=5432; User ID=postgres; Password=postgres; Database=postgres; Timeout = 5; Pooling=false;
 Version ........ : PostgreSQL 9.4.15 on x86_64-unknown-linux-gnu, compiled by gcc (Debian 4.9.2-10) 4.9.2, 64-bit (at the 2nd second)
 
 Ping ........... : google.com
 Status ......... : OK. 1.63 msecs (at the 3rd second)

 HttpGet ........ : http://nginx
 Status ......... : OK (200). Server: nginx/1.13.7. 612 bytes recieved (at the 3rd second)

 RabbitMQ ....... : amqp://rabbitmq:5672
 Version ........ : 3.4.4 (at the 3rd second)
 
 HttpGet ........ : https://portal.azure.com
 Status ......... : OK (200). Server: Microsoft-IIS/10.0. 5194 bytes recieved (at the 3rd second)

 MySQL .......... : Server = mysql; Port=3306; Uid = root; Pwd = example; Connect Timeout = 5; Pooling=false;
 Version ........ : 5.5.58 (at the 3rd second)
 
 Redis .......... : redis:6379
 Version ........ : 4.0.2 (Standalone) (at the 3rd second)
 
 HttpGet ........ : https://google.com
 Status ......... : OK (200). Server: gws. 47380 bytes recieved (at the 3rd second)
 
 MySQL .......... : Server = mysql_another; Port=3306; Uid = root; Pwd = example; Connect Timeout = 5; Pooling=false;
 Version ........ : 5.7.20 (at the 3rd second)
 
 Memcached ...... : memcached:11211
 Version ........ : 1.5.3 [64 bits] (at the 3rd second)
 
 MongoDB ........ : mongodb://mongodb:27017
 Version ........ : 3.4.10 (at the 4th second)

 MSSQL .......... : Data Source=sqlserver; User ID=sa; Password=~1qazxsw2; Timeout = 5; Pooling=false;
 Version ........ : 14.0.3008.27 [Developer Edition (64-bit)] (at the 4th second)
```
Full Report is available by travis-ci link above

## Build and run locally
Just copy into shell the content of [clone-build-and-compose-up.sh](clone-build-and-compose-up.sh)
Prerequesites: git, docker, docker-compose and dotnet sdk 2+
