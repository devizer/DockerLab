#!/bin/bash
dotnet publish -v:m -c Debug -r linux-x64 -o bin/linux DockerLab.sln

printf "\n ------------- BUILD WaitFor as lab/theapp -------------\n"
pushd WaitFor/bin/linux
./WaitFor -Timeout=3 -HttpGet=https://google.com/404
cp ../../../containers/WaitFor/* .
docker rmi -f lab/theapp
docker build -t lab/theapp .
docker run -it lab/theapp -Timeout=3 -Ping=google.com
popd

printf "\n ------------- BUILD containers -------------\n"
cd containers
export COMPOSE_HTTP_TIMEOUT=121
export COMPOSE_PROJECT_NAME=lab

docker network create --driver=bridge "$COMPOSE_PROJECT_NAME"_default
time (docker-compose create);

printf "\n ------------- RUN Tests -------------\n"
test=theapp;
other_services=$(docker-compose ps | tail -n +3 | awk '{print $1}' | grep -vE '(_'$test'_)')
docker start $other_services; 
echo Starting unit tests
docker start -i "$COMPOSE_PROJECT_NAME"_"$test"_1; 
exit $?;

