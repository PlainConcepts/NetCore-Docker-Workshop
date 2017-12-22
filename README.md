# Managing our containerized applications through docker compose
In this section we will use docker compose tool to create and run our images for our sample project. We will see how docker compose is much more convenient that Docker command tool when we have more than one image to manage.

**We must start from the code in the branch 2-dockerfiles**. This code already contains the Docker files for Master and Slave applications.

## Pre-requisite: docker compose
Apart of the Docker command tool, you must have the Docker compose tool installed to follow this section. Depending on the platform you are using, the installation of Docker will have already installed docker compose as well. To check it you can just execute `docker compose` in a comand prompt. In case it is not installed, install it from the [compose section in the Docker web page](https://docs.docker.com/compose/).

## Compiling and publishing our code
1. Go to the `src/Master/` folder and execute:
```
> dotnet restore
> dotnet build
> dotnet publish -c Release  -o obj/Docker/publish 
```
The publish command packs the application and its dependencies into a folder for deployment. In this case the deployment content is copied to the folder `src/Master/obj/Docker/publish`. This is the place set in our Dockerfile to get the reosurces that will be copied when the Master image is created. Here the Dockerfile:

```
FROM microsoft/aspnetcore:2.0
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "Master.dll"]
``` 
2. Repeat the previous step (build and publish) for the Slave folder. The result should be that the Slave.dll and the rest of files to be deployed are copied into the folder `src/Slave/obj/Docker/publish`

## Creating our images from docker compose
1. In the 'src' folder where we have the folders for our applications Master and Slave, create a new file named 'docker-compose.yml'
2. Fill the file with the configuration:

```
version: '2.1'

services:

  master:
    image: pcdockerws/master
    build:
      context: ./Master
      dockerfile: Dockerfile

  slave:
    image: pcdockerws/slave
    build:
      context: ./Slave
      dockerfile: Dockerfile
``` 

We have in this code the next parts:
- version: the version of the config language. Different versions allows different tags for instance.
- services: defines the different services (images) in a list
  - master/slave: names for our services in the yml files. We must reference a service in the yml file by this name.
    - image: determines the name of the image when created, plus repository and tags if desired in format `{repository}/{imgage}:{label}`.
    - build: if present, determines that the image will be build. If not present, the image will be pulled from the container repository.
      - context: Sets the base path where to start the build. In this case the folder where there is the Dockerfile file.
      - dockerfile: the name of the Docker file to use for the building of the image.

3. Once the configuration file is created, we can execute `docker-compose` from the `src` folder to build the images and run containers:
```
> docker-compose up
```
If we want to build the images but not start any containers, we execute a build option instead:
```
> docker-compose build
```

Once the command has finished, check that the images has been created executing in the command prompt or shell `docker images`. To see the the containers running list execute `docker ps`. Note that this containers do not have the external port since we have not defined one. This implies that the applications will not receive our requests. We will open the external ports in the next step.

## Extending the base configuration
By convention the `docker-compose.yml` file contains the base configuration of our services. As a good practice, we extend this configuration adding more files that overrides the values of the precedent ones. By default the docker-compose command reads the files `docker-compose.yml` and `docker-compose.override.yml`. The override file is used to override values of the base configuration and extend it. If configuration files with other names are required (f.i. we can create one file `docker-compose.prod.yml` with production configuration) we need to specify them explicitly using the `-f` parameter, as in this example:

```
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up
``` 

Now we will add an override file to our base configuration:
1. Create a file named `docker-compose.override.yml` in the same folder as `docker-compose.yml`.
2. Copy this configuration to it:
```
version: '2.1'

services:
      
  master:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - MasterSettings__SlaveUri=http://slave
    ports:
      - "5000:80"

  slave:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    ports:
      - "5001:80"
```

Here we have some tags already seen in the docker-compose file. We are adding here some extra tags:
- environment: under this tag we put the list of environment variables set under a structure of `{key}={value}`. 
- ports:  define the ports to open with a mapping between the Docker host net and the container net. For instance for the master service the external port is the 5000 and will connect to the port 80 of the container (internal port).

Yo can see how we reference the addresses by the name of the service. In the case of the address for Slave we put http://slave and Docker will manage the names redirecting to the proper IPs and ports.

3. Start the containers with the modified configuration as before:
```
> docker-compose up
```
You can see all the containers running with `docker ps`. Note that the ports of the services corresponds to the ones set in the override file (5000=>80 and 5001=>80). In the browser you can see the messages of Master and Slave calling the APIS:

![Get master](https://raw.githubusercontent.com/PlainConcepts/NetCore-Docker-Workshop/2-dockerfiles/src/Docu/2.%20Docker%20compose/img/Get_master.PNG)

![Get slave](https://raw.githubusercontent.com/PlainConcepts/NetCore-Docker-Workshop/2-dockerfiles/src/Docu/2.%20Docker%20compose/img/Get_master.PNG)


## Dockerize the application using Visual Studio 2017
We have exposed until now how to introduce Docker and Docker Compose to our service manually. This is very useful since allow us to understand how the prcess works and to see how it is possible to customize every part. Nevertheless, the process can be done automatically through Visual Studio 2017 in a couple of clicks from scratch.
1. Start clean from the branch **1-start**. Remove any Docker file or docker-compose.
2. Open the 'src/All.sln' solution.
3. Go to the Master project and right-click. Go to Add

![Add Docker](https://raw.githubusercontent.com/PlainConcepts/NetCore-Docker-Workshop/2-dockerfiles/src/Docu/2.%20Docker%20compose/img/VS2017_add_docker.PNG)

4. A selector will apppear to choose the OS. We choose Linux for the workshop.

![VS2017_add_docker_linux](https://raw.githubusercontent.com/PlainConcepts/NetCore-Docker-Workshop/2-dockerfiles/src/Docu/2.%20Docker%20compose/img/VS2017_add_docker_linux.PNG)

Automatically VS will add the Dockerfile files for each project:

![VS2017_with_docker](https://raw.githubusercontent.com/PlainConcepts/NetCore-Docker-Workshop/2-dockerfiles/src/Docu/2.%20Docker%20compose/img/VS2017_with_docker.PNG)

5. You can run the application simply building the application and clicking Play as usual with VS. You can run the application inside your container when you have as Startup project the Docker project:

![docker_run](https://raw.githubusercontent.com/PlainConcepts/NetCore-Docker-Workshop/2-dockerfiles/src/Docu/2.%20Docker%20compose/img/docker_run.PNG) 

![docker_run2](https://raw.githubusercontent.com/PlainConcepts/NetCore-Docker-Workshop/2-dockerfiles/src/Docu/2.%20Docker%20compose/img/docker_run_2.PNG)

In case that you want to run the application without the containers, in the clasical way, you can do it setting as Startup project another project different from the Docker:

![docker_run](https://raw.githubusercontent.com/PlainConcepts/NetCore-Docker-Workshop/2-dockerfiles/src/Docu/2.%20Docker%20compose/img/iis_run.PNG) 

![docker_run2](https://raw.githubusercontent.com/PlainConcepts/NetCore-Docker-Workshop/2-dockerfiles/src/Docu/2.%20Docker%20compose/img/iis_run_2.PNG)
