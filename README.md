## DockerLab

Here is a proof-of-concept for test-environment of an app with composed dbs (MS SQL, MySQL, PostreSQL, Redis, RabbitMQ, MongoDB)

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
