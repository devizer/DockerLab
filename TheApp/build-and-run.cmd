dotnet publish -v:m -c Debug -o bin\output
pushd bin\output
dotnet TheApp.dll --help
net start mssql$sql2005 1>nul 2>&1
dotnet TheApp.dll ^
 "-MySQL=      Server = 192.168.0.8; Port=3306; Uid = mysql; Pwd = mysql; " ^
 "-MySQL=      Server = 192.168.0.8; Port=3307; Uid = root; Pwd = example; " ^
 "-MSSQL=      Data Source = (local)\SQL2005; Integrated Security=true;" ^
 "-PostgreSQL= Host = 192.168.0.8; Port=5432; User ID=postgres; Password=postgres; Database=postgres;" ^
 "-MongoDB=    mongodb://192.168.0.8:27017" ^
 "-RabbitMQ=   amqp://192.168.0.8:5672" ^
 "-Redis=      192.168.0.8:6379"

rem  "-MySQL=      Server = 192.168.0.8; Port=3306; Uid = mysql; Pwd = mysql; "  "-MySQL=      Server = 192.168.0.8; Port=3307; Uid = root; Pwd = example; "  "-PostgreSQL= Host = 192.168.0.8; Port=5432; User ID=postgres; Password=postgres; Database=postgres;" "-MongoDB=    mongodb://192.168.0.8:27017"  "-RabbitMQ=   amqp://192.168.0.8:5672" "-Redis=      192.168.0.8:6379"
net stop mssql$sql2005 1>nul 2>&1
