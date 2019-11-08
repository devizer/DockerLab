#!/usr/bin/env bash
set -e
docker ps -a | grep local-waitfor | awk '{print $1}' | xargs docker rm -f || true
docker images --format "{{.ID}} {{.Repository}}" -q -a | grep local-waitfor | awk '{print $1}' | xargs docker rmi -f || true

bash -e v2-build.sh
docker-compose up --build --exit-code-from theapp
