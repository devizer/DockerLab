FROM microsoft/dotnet:2.0.3-runtime-deps-jessie
WORKDIR /app
COPY . /app
# ENTRYPOINT [ "./TheApp", "-MySQL=oops; hehe2=ahaha" ]
ENTRYPOINT [ "./TheApp",  "-Timeout=120", \
 "-MySQL=      Server = mysql; Port=3306; Uid = root; Pwd = example; Connect Timeout = 10", \
 "-PostgreSQL= Host = postgres; Port=5432; User ID=postgres; Password=postgres; Database=postgres; Timeout = 5", \
 "-PostgreSQL= Host = postgres101; Port=15432; User ID=postgres; Password=postgres; Database=postgres; Timeout = 5", \
 "-MongoDB=    mongodb://mongodb:27017", \
 "-RabbitMQ=   amqp://rabbitmq:5672", \
 "-Redis=      redis:6379", \
 "-MSSQL=      Data Source = sqlserver ; User ID=sa; Password=~1qazxsw2; Timeout = 10" \
]
