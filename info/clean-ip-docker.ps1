docker ps -a -q | % { docker rm -f $_ }
docker images -a -q | % { docker rmi -f $_ }

