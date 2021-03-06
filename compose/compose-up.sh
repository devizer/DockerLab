#!/usr/bin/env bash
set -e
# docker ps -a | grep local-waitfor | awk '{print $1}' | xargs docker rm -f || true
# docker images --format "{{.ID}} {{.Repository}}" -q -a | grep local-waitfor | awk '{print $1}' | xargs docker rmi -f || true

time bash -e build.sh
time docker-compose up --no-start --build --force-recreate 
time docker-compose up --exit-code-from theapp
