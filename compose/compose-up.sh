#!/usr/bin/env bash
bash v2-build.sh
docker-compose up --exit-code-from theapp
