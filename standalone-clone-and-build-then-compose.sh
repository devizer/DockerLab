#!/bin/bash
work=$HOME/transient-builds
if [[ -d "/transient-builds" ]]; then work=/transient-builds; fi

mkdir -p $work; cd $work; rm -rf $work/DockerLab;
git clone https://github.com/devizer/DockerLab; cd DockerLab
git pull
sudo bash build-and-compose-up.sh
