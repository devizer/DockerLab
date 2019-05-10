#!/bin/bash
dotnet publish -v:m -c Debug -r linux-x64 -o bin/linux DockerLab.sln

bash build-images.sh

printf "\n ------------- BUILD containers -------------\n"
cd containers
export COMPOSE_HTTP_TIMEOUT=121
export COMPOSE_PROJECT_NAME=lab
docker network create --driver=bridge "$COMPOSE_PROJECT_NAME"_default
time (docker-compose create);

printf "\n ------------- /SKIPPED/ RUN Test's dependencies -------------\n"
test=theapp;
# other_services=$(docker-compose ps | tail -n +3 | awk '{print $1}' | grep -vE '(_'$test'_)')
# docker start $other_services; 

printf "\n ------------- RUN Tests -------------\n"
docker start -i "$COMPOSE_PROJECT_NAME"_"$test"_1; 
exit $?;

