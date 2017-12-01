#!/bin/bash
dotnet publish -v:m -c Debug -r linux-x64 -o bin/linux DockerLab.sln

printf "\n ------------- BUILD WaitFor as lab/theapp -------------"
pushd WaitFor/bin/linux
./WaitFor -Timeout=3 -HttpGet=https://google.com/404
cp ../../../containers/WaitFor/* .
docker rmi -f lab/theapp
docker build -t lab/theapp .
docker run -it lab/theapp -Timeout=3 -Ping=google.com
popd

printf "\n ------------- BUILD containers -------------"
cd containers
export COMPOSE_HTTP_TIMEOUT=121
export COMPOSE_PROJECT_NAME=lab
docker-compose rm -f theapp
# echo "Kill all the services"; docker-compose -p lab kill; docker-compose -p lab rm -f

# it will output into the log of all the services, will never stop
docker-compose up | tee compose-up.log; exit;

# UNIT TESTS: it will output to log the tests *only* and forward exit code to build server
test=theapp;
export COMPOSE_PROJECT_NAME=lab
docker-compose create;
other_services=$(docker-compose ps | tail -n +3 | awk '{print $1}' | grep -vE '(_'$test'_)')
docker start $other_services; 
echo Starting unit tests
docker start -i "$COMPOSE_PROJECT_NAME"_"$test"_1; 
exit $?;
