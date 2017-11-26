FROM microsoft/dotnet:2.0.3-runtime-deps-jessie
WORKDIR /app
COPY . /app
# ENTRYPOINT [ "./TheApp", "-MySQL=oops; hehe2=ahaha" ]
ENTRYPOINT [ "./TheApp",  "-Sleep=44", \
 "-MySQL=      Server = localhost; Port=3306; Uid = root; Pwd = example; ", \
 "-MSSQL=      Data Source = tcp:localhost,1433; Integrated Security=true;", \
 "-PostgreSQL= Host = localhost; Port=5432; User ID=postgres; Password=postgres; Database=postgres;", \
 "-MongoDB=    mongodb://localhost:27017", \
 "-RabbitMQ=   amqp://localhost:5672", \
 "-Redis=      localhost:6379" ]

