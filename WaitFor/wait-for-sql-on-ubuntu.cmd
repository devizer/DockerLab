dotnet publish -v:m -c Debug -o bin\output WaitFor.csproj
pushd bin\output
C:\Apps\VMWare-14.1.2\vmrun.exe start "C:\VM4\Ubuntu Server Bionic x64\Ubuntu Server Bionic x64.vmx"
dotnet WaitFor.dll --help
dotnet WaitFor.dll -Timeout=600 ^
 "-Ping=       192.168.0.18" ^
 "-MSSQL=      Data Source = tcp:192.168.0.18,1433; User=sa; Password=`1qazxsw2; Connection Timeout=1" ^
 "-MSSQL=      Data Source = tcp:192.168.0.18,1933; User=sa; Password=`1qazxsw2; Connection Timeout=1" ^
 "-Ping=       localhost" 
