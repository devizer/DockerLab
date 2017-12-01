#!/bin/bash

printf "\n ------------- BUILD WaitFor as lab/theapp -------------\n"
pushd WaitFor/bin/linux
./WaitFor -Timeout=3 -HttpGet=https://google.com/404
cp ../../../containers/WaitFor/* .
(docker rmi -f lab/theapp || true)
docker build -t lab/theapp .
docker run -it lab/theapp -Timeout=3 -Ping=google.com
popd

printf "\n ------------- BUILD HelloRest as lab/hello-rest -------------\n"
pushd HelloRest/bin/linux
cp ../../../containers/HelloRest/* .
(docker rmi -f lab/hello-rest || true)
docker build -t lab/hello-rest .
popd
