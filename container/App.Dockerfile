FROM microsoft/dotnet:2.0.3-runtime-deps-jessie
WORKDIR /app
COPY . /app
# ENTRYPOINT [ "./TheApp", "-MySQL=oops; hehe2=ahaha" ]
ENTRYPOINT [ "./TheApp",  "-Sleep=47", \
 "-MySQL=      Server = mysql; Port=3306; Uid = root; Pwd = example; Connect Timeout = 10", \
 "-MSSQL=      Data Source = tcp:sqlserver,1433; Integrated Security=true; Timeout = 10", \
 "-PostgreSQL= Host = postgres; Port=5432; User ID=postgres; Password=postgres; Database=postgres; Timeout = 5", \
 "-MongoDB=    mongodb://mongodb:27017", \
 "-RabbitMQ=   amqp://rabbitmq:5672", \
 "-Redis=      redis:6379" ]

