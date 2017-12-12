#!/bin/sh
set -e ;


#docker-compose -f docker-compose.ci.build.yml up
#docker-compose -f docker-compose.ci.build.yml down --remove-orphans

docker-compose -f docker-compose.yml down --remove-orphans
docker-compose -f docker-compose.yml up -d --build

sleep 4

docker ps -a | grep devweek_worker

docker logs src_devweek_worker_1

