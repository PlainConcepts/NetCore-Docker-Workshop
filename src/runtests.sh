#!/bin/bash
mkdir TestResults
ls
docker-compose -f "docker-compose.test.yml" -f "docker-compose.override.yml" up -d master
docker-compose -f "docker-compose.test.yml" -f "docker-compose.override.yml" up -d slave
docker-compose -f "docker-compose.test.yml" -f "docker-compose.override.yml" up integrationtests