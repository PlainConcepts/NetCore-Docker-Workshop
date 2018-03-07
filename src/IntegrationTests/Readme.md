To run the test locally, you must run the containers for Master and Slave, and then execute the docker-compose files for the integration test image. Just build the images for the Master and Slave and eecute:

```
docker-compose -f "docker-compose.test.yml" -f "docker-compose.override.yml" up -d master
docker-compose -f "docker-compose.test.yml" -f "docker-compose.override.yml" up -d slave
docker-compose -f "docker-compose.test.yml" -f "docker-compose.override.yml" up integrationtests
```

The resul of the tests will be stored in the folder test-results.