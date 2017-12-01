docker rm -f $(docker ps -aq)
docker rmi microsoft/mssql-server-linux
