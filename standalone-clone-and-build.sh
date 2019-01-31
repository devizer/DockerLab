#!/bin/bash
work=$HOME/build
mkdir -p $work; cd $work; rm -rf $work/DockerLab;
git clone https://github.com/devizer/DockerLab; cd DockerLab
git pull
if [[ $(uname -m) == armv7* ]]; then rid=linux-arm; else rid=linux-arm64; fi; if [[ "$(uname -m)" == "x86_64" ]]; then rid=linux-x64; fi; echo ".NET Core runtime identifier: $rid"
dotnet publish -v:m -c Debug -r $rid -o out --self-contained true DockerLab.sln
export Wait_For_Ping=google.com
export Wait_For_Ping_facebook=facebook.com
WaitFor/out/WaitFor -Timeout=3 -HttpGet=https://google.com -HttpGet=https://facebook.com -Ping=localhost


