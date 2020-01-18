#!/bin/sh
set -e ;


#docker-compose -f docker-compose.ci.build.yml up
#docker-compose -f docker-compose.ci.build.yml down --remove-orphans

docker-compose -f docker-compose.yml -f docker-compose.dev.yml down --remove-orphans
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up -d --build --scale devweek_worker=2

sleep 4

docker-compose -f docker-compose.yml -f docker-compose.dev.yml ps
docker-compose -f docker-compose.yml -f docker-compose.dev.yml logs devweek_worker


