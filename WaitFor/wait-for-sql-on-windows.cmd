dotnet publish -v:m -c Debug -o bin\output WaitFor.csproj
pushd bin\output
C:\Apps\VMWare-14.1.2\vmrun.exe start "V:\VM4\Windows Server 2019 DC\Windows Server 2019 DC.vmx" 
dotnet WaitFor.dll --help
dotnet WaitFor.dll -Timeout=600 ^
 "-Ping=       SERVER2019" ^
 "-MSSQL=      Data Source =tcp:SERVER2019\SQL_2005_SP4; User=sa; Password=`1qazxsw2; Connection Timeout=1" ^
 "-MSSQL=      Data Source =tcp:SERVER2019\SQL_2008_R2_SP2; User=sa; Password=`1qazxsw2; Connection Timeout=1" ^
 "-MSSQL=      Data Source =tcp:SERVER2019\SQL_2008_SP3; User=sa; Password=`1qazxsw2; Connection Timeout=1" ^
 "-MSSQL=      Data Source =tcp:SERVER2019\SQL_2012_SP3; User=sa; Password=`1qazxsw2; Connection Timeout=1" ^
 "-MSSQL=      Data Source =tcp:SERVER2019\SQL_2014_SP1; User=sa; Password=`1qazxsw2; Connection Timeout=1" ^
 "-MSSQL=      Data Source =tcp:SERVER2019\SQL_2014_SP2_X86; User=sa; Password=`1qazxsw2; Connection Timeout=1" ^
 "-MSSQL=      Data Source =tcp:SERVER2019\SQL_2016; User=sa; Password=`1qazxsw2; Connection Timeout=1" ^
 "-MSSQL=      Data Source =tcp:SERVER2019\SQL_2017; User=sa; Password=`1qazxsw2; Connection Timeout=1" ^
 "-MSSQL=      Data Source =tcp:SERVER2019\SQL_DEV_2017; User=sa; Password=`1qazxsw2; Connection Timeout=1" ^
 "-Ping=       localhost" 





