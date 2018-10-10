docker run -d --name sql-2017 --restart always -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=`1qazxsw2' -p 1433:1433 microsoft/mssql-server-linux
docker run -d --name sql-2019 --restart always -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=`1qazxsw2' -p 1933:1433 mcr.microsoft.com/mssql/server:vNext-CTP2.0-ubuntu
