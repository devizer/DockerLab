#!/bin/bash
work=$HOME/build
mkdir -p $work; cd $work; rm -rf $work/DockerLab;
git clone https://github.com/devizer/DockerLab; cd DockerLab
git pull
dotnet publish -v:m -c Debug -r rhel.6-x64 -o out --self-contained true DockerLab.sln
export Wait_For_Ping=google.com
export Wait_For_Ping_facebook=facebook.com
export Wait_For_MySQL_1="Server = localhost; Port=3306; Uid = root; Pwd = root; Connect Timeout = 5; Pooling=false; "
WaitFor/out/WaitFor -Timeout=3 -HttpGet=https://google.com -HttpGet=https://facebook.com -Ping=localhost
