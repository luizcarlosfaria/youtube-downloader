#!/bin/sh
set -e ;


#docker-compose -f docker-compose.ci.build.yml up
#docker-compose -f docker-compose.ci.build.yml down --remove-orphans

docker-compose -f docker-compose.yml -f docker-compose.prd.yml down --remove-orphans
docker-compose -f docker-compose.yml -f docker-compose.prd.yml up -d --build

sleep 4

docker-compose -f docker-compose.yml -f docker-compose.prd.yml ps
docker-compose -f docker-compose.yml -f docker-compose.prd.yml logs devweek_worker


