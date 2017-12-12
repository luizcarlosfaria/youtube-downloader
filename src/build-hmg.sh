#!/bin/sh
set -e ;


#docker-compose -f docker-compose.ci.build.yml up
#docker-compose -f docker-compose.ci.build.yml down --remove-orphans

docker-compose -f docker-compose.yml -f docker-compose.hmg.yml down --remove-orphans
docker-compose -f docker-compose.yml -f docker-compose.hmg.yml up -d --build

sleep 4

docker-compose -f docker-compose.yml -f docker-compose.hmg.yml ps
docker-compose -f docker-compose.yml -f docker-compose.hmg.yml logs devweek_worker


