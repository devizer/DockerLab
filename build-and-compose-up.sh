#!/bin/bash
set -e
dotnet publish -v:m -c Debug -r linux-x64 -o bin/linux DockerLab.sln

bash build-images.sh

printf "\n ------------- BUILD containers -------------\n"
cd containers
export COMPOSE_HTTP_TIMEOUT=121
export COMPOSE_PROJECT_NAME=lab
docker-compose rm -f theapp

echo "Kill all the services"; docker-compose -p lab kill; docker-compose -p lab rm -f

# it will output into the log of all the services, will never stop
docker-compose up --exit-code-from theapp | tee compose-up.log; exit $?;
