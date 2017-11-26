FROM microsoft/dotnet:2.0.3-runtime-deps-jessie
WORKDIR /app
COPY . /app
# ENTRYPOINT [ "./TheApp", "-MySQL=oops; hehe2=ahaha" ]
ENTRYPOINT [ "./TheApp",  "-Sleep=20", \
 "-MySQL=      Server = mysql; Port=3306; Uid = root; Pwd = example; ", \
 "-MSSQL=      Data Source = tcp:mssql,1433; Integrated Security=true;", \
 "-PostgreSQL= Host = postgres; Port=5432; User ID=postgres; Password=postgres; Database=postgres;", \
 "-MongoDB=    mongodb://mongodb:27017", \
 "-RabbitMQ=   amqp://rabbitmq:5672", \
 "-Redis=      redis:6379" ]

