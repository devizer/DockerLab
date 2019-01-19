docker run -d --name sql-2017 --restart always -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=`1qazxsw2' -p 1433:1433 microsoft/mssql-server-linux
tag2019=vNext-CTP2.0-ubuntu
tag2019=2019-CTP2.2-ubuntu
docker run -d --name sql-2019 --restart always -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=`1qazxsw2' -p 1933:1433 mcr.microsoft.com/mssql/server:$tag2019
exit;



docker run -it --name sql-2019 --restart always -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=`1qazxsw2' -p 1933:1433 msint.azurecr.io/mssql/server:latest

docker pull mcr.microsoft.com/mssql/server:vNext-CTP2.2-ubuntu
docker pull mcr.microsoft.com/mssql/server:vNext-CTP2.1-ubuntu
docker pull mcr.microsoft.com/mssql/server:vNext-CTP2.0-ubuntu
