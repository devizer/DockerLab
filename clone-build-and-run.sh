#!/bin/bash
work=$HOME/build
mkdir -p $work; rm -rf $work/DockerLab; cd $work
git clone https://github.com/devizer/DockerLab
cd DockerLab/TheApp
dotnet publish -v:n -c Debug -r linux-x64 -o bin/linux
cd bin/linux
./TheApp \
 "-MySQL=      Server = 192.168.0.8; Port=3306; Uid = mysql; Pwd = mysql; " \
 "-MSSQL=      Data Source = (local)\SQL2005; Integrated Security=true;" \
 "-PostgreSQL= Host = 192.168.0.8; Port=5432; User ID=postgres; Password=postgres; Database=postgres;" \
 "-MongoDB=    mongodb://192.168.0.8:27017" \
 "-RabbitMQ=   amqp://192.168.0.8:5672" \
 "-Redis=      192.168.0.8:6379"

docker build -t vlad/theapp -f ../../../App.Dockerfile 
