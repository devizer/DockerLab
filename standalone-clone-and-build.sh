#!/bin/bash
work=$HOME/build
mkdir -p $work; cd $work; rm -rf $work/DockerLab;
git clone https://github.com/devizer/DockerLab; cd DockerLab
git pull
if [[ $(uname -m) == armv7* ]]; then rid=linux-arm; else rid=linux-arm64; fi; echo ".NET Core runtime identifier: $arch"
dotnet publish -v:m -c Debug -r $rid -o out --self-contained DockerLab.sln

