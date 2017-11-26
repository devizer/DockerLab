FROM microsoft/dotnet:2.0.3-runtime-deps-jessie
WORKDIR /app
COPY . /app
ENTRYPOINT [ "./TheApp",  "-MySQL=      Server = 192.168.0.8; Port=3306; Uid = mysql; Pwd = mysql; ", \
 "-MSSQL=      Data Source = (local)\SQL2005; Integrated Security=true;", \
 "-PostgreSQL= Host = 192.168.0.8; Port=5432; User ID=postgres; Password=postgres; Database=postgres;", \
 "-MongoDB=    mongodb://192.168.0.8:27017", \
 "-RabbitMQ=   amqp://192.168.0.8:5672", \
 "-Redis=      192.168.0.8:6379" ]
