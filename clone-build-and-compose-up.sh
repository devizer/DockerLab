#!/bin/bash
work=$HOME/build
mkdir -p $work; cd $work; rm -rf $work/DockerLab;
git clone https://github.com/devizer/DockerLab
cd DockerLab/TheApp
bash patch-build-date.sh
dotnet publish -v:m -c Debug -r linux-x64 -o bin/linux TheApp.sln
cd bin/linux
./WaitFor -Timeout=3 -HttpGet=https://google.com/404

cp ../../../container/* .
docker rmi -f lab/theapp
docker build -t lab/theapp .

export COMPOSE_HTTP_TIMEOUT=121
export COMPOSE_PROJECT_NAME=lab
docker-compose rm -f theapp
# echo "Kill all the services"; docker-compose -p lab kill; docker-compose -p lab rm -f
docker-compose up | tee compose-up.log

