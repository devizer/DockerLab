# exec command
container=linux_theapp_1
docker start $container; docker exec -it $container bash -c 'printenv | sort'; docker stop $container

# run app and wait for exit
container=linux_theapp_1
docker start -i $container;
