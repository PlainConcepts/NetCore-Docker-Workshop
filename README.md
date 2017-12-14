# NetCore-Docker-Workshop

The workshop is divided in several branches, each one with instructions to follow in .md files and the code prepared for the step. Each branch-step provides the code in the state that shoud be after ending the previous step. This way in case you want to start in one specific step you do not need to do all the previous steps-branches tutorials. In the master branch there are presentations about diferent parts of the workshop and an introduction about microservices.

## Branch Master
Includes the presentations [under the Docu folder in the root path](https://github.com/PlainConcepts/NetCore-Docker-Workshop/tree/master/Docu). It also includes the projects non dockerized under `src folder`.

## Branch '1-start': dockerize manually the apis
This branch has the same code as master, the two apis without any Docker file. It contains the instructions to dockerize the apis in [/src/Docu/1.Docker/Add Docker file.md](https://github.com/PlainConcepts/NetCore-Docker-Workshop/blob/1-start/src/Docu/1.Docker/Add%20Docker%20file.md) file. The result should be the two apis in docker containers able to comunicate within them.

## Branch '2-dockerfiles': adding docker compose
The branch shows how we can simplify our work when managing several containers by using Docker Compose. The starting point is the result of the previous branch (1-start): the two apis with the docker files. It contains the instructions to add docker-compose in [/src/Docu/2. Docker compose/Add docker compose.md](https://github.com/PlainConcepts/NetCore-Docker-Workshop/tree/2-dockerfiles/src/Docu/2.%20Docker%20compose) file. The result should be the two apis running in containers and comunicating.

## Branch '3-compose'
This branch is simply the final state of the branch '2-dockerfiles' after adding docker compose tutorial.

## Branch '4-appservice': register you images to ACR and publish to App Service
The branch contains the two apis with docker files and docker-compose, ready to be published. Here we have two guidelines:
1. **Register your app in a container repository**: at [/deploy/Create_ACR.md](https://github.com/PlainConcepts/NetCore-Docker-Workshop/blob/4-appservice/deploy/Create_ACR.md) there are the steps to pubish the Docker images of the apis in Azure Container Registry (ACR). We will use the AZ tools to do it, and instead of install them in our local machine we will execute them from a container with AZ tools. The final result should be our Docker iamges published in one private ACR.
2. **Publish the apis on App Service**: at [/deploy/deployment.md](https://github.com/PlainConcepts/NetCore-Docker-Workshop/blob/4-appservice/deploy/deployment.md) there are the steps to publish our images stored in one ACR in App service.

## Branch '5-k8s': deploying on kubernetes
Under [/deploy/README.md](https://github.com/PlainConcepts/NetCore-Docker-Workshop/blob/5-k8s/deploy/README.md) there is the steps to deploy the apis on kubernetes and all the required scripts and configuration files.

## Branch '6-docker.machine': create and connect to a Docker machine
 Documentation is in [/src/Docu/docker-machine-steps.md](https://github.com/PlainConcepts/NetCore-Docker-Workshop/blob/6-docker-machine/src/Docu/docker-machine-steps.md)

 ## Branch '7-resilience': adding Polly for Circuit breaker and HealthChecks for health checks
 Health checks tutorial is in [/src/Docu/7. HealthChecks/Add HealhChecks.md](https://github.com/PlainConcepts/NetCore-Docker-Workshop/blob/7-resilience/src/Docu/7.%20HealthChecks/Add%20HealhChecks.md).
 
 Polly tutorial is in [/src/Docu/Polly/circuitbreaker_demo.md](https://github.com/PlainConcepts/NetCore-Docker-Workshop/blob/7-resilience/src/Docu/Polly/circuitbreaker_demo.md)

 You can find some information about health checks and circuit breaker in the presentation 'BestPractices&Antipatterns.pptx' under the master branch.

 ## Branch 'AzureFunctions': create a simple Azure functions example
 [/AzureFunctions.md](https://github.com/PlainConcepts/NetCore-Docker-Workshop/blob/AzureFunctions/AzureFunctions.md) explains how publish an Azure function. The 'All.sln' contains one project of Azure function.

 ## 




