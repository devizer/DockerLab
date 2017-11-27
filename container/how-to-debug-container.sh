container=linux_theapp_1
docker start $container; docker exec -it $container bash -c 'printenv | sort'; docker stop $container
