#!/usr/bin/env bash
docker ps -a | grep local-waitfor | awk '{print $1}' | xargs docker rm -f
docker images --format "{{.ID}} {{.Repository}}" -q -a | grep local-waitfor | awk '{print $1}' | xargs docker rmi -f

bash v2-build.sh
docker-compose up --build --exit-code-from theapp
