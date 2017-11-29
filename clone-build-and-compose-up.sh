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
docker rmi -f vlad/theapp
docker rm -f $(docker ps -a -q)
docker build -t vlad/theapp -f ./Dockerfile .

export COMPOSE_HTTP_TIMEOUT=121
docker-compose -f stack.yml up | tee compose-up.log
