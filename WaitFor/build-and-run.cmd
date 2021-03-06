pushd Properties && call inc-version-info.cmd && popd

dotnet publish -v:m -c Debug -o bin\output WaitFor.csproj
pushd bin\output
dotnet WaitFor.dll --help
net start mssql$sql2005 1>nul 2>&1
dotnet WaitFor.dll -Timeout=9 ^
 "-MySQL=      Server = 192.168.0.8; Port=3306; Uid = mysql; Pwd = mysql; " ^
 "-MySQL=      Server = 192.168.0.8; Port=3307; Uid = root; Pwd = example; " ^
 "-MSSQL=      Data Source = (local)\SQL2005; Integrated Security=true;" ^
 "-PostgreSQL= Host = 192.168.0.8; Port=5432; User ID=postgres; Password=postgres; Database=postgres;" ^
 "-MongoDB=    mongodb://192.168.0.8:27017" ^
 "-RabbitMQ=   amqp://192.168.0.8:5672" ^
 "-Redis=      192.168.0.8:6379" ^
 "-Ping=       google.com" ^
 "-HttpGet=    https://google.com/404" ^
 "-MemCached=  192.168.0.8:11211"

rem  "-MySQL=      Server = 192.168.0.8; Port=3306; Uid = mysql; Pwd = mysql; "  "-MySQL=      Server = 192.168.0.8; Port=3307; Uid = root; Pwd = example; "  "-PostgreSQL= Host = 192.168.0.8; Port=5432; User ID=postgres; Password=postgres; Database=postgres;" "-MongoDB=    mongodb://192.168.0.8:27017"  "-RabbitMQ=   amqp://192.168.0.8:5672" "-Redis=      192.168.0.8:6379"
net stop mssql$sql2005 1>nul 2>&1
