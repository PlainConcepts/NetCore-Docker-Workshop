# Adding a dotnet program into a Docker container
In this section we will "containerize" a dotnet program. 

**Start from the branch 1-start.**
 
#### Prerequisites: Install Docker and .Net Core SDK
First we need to install Docker and the the dotnet core SDK  in our local machine in order to be able to build the program and create the new container images. Go to the [Docker website](https://www.docker.com/), download the Docker Community Edition and install it.
From the [.Net web page](https://www.microsoft.com/net/)  download the .Net Core SDK and install it.

## Creating an image from the Master application
#### Compile and run the code
We will dockerize the Master application. This is under the folder `.\src\Master` of the project, *in the master branch*. Open a comand prompt under the path and do the following commands:

```
src\Master> dotnet restore
src\Master> dotnet build
```
At this point we must ahve the dll build under the folder `bin\Debug\netcoreapp2.0`

## 'Dockerizing' our application 
Follow the next steps:
1. Create a file named Dockerfile without extension in the Master folder.
2. Open the file with a text editor and add this configuration:
```
FROM microsoft/aspnetcore
WORKDIR /app
EXPOSE 80
COPY bin/Debug/netcoreapp2.0 .
ENTRYPOINT ["dotnet", "Master.dll"]
``` 
3. From the command line execute the next command (**Do not forget to include the '.' at the end, which indicates that the current folder contains the Dockerfile**) 
```
> docker build -t="dockerworkshop/master:manual" .  
```
This command can take some minutes the first time it is executed because the base images for our new image will be downloaded from the Docker Hub repository. After execution an image with name 'Master' should ahve been created locally.  

4. Run `docker images` to see the list of images. The image Master should appear in the list.

5. Run the container for the Master image. Execute:
```
> docker run -d -p 80 --name container1 dockerworkshop/master:manual 
```
The `-d` parameter allows to run the container in detached mode. Otherwise the shell where you execute the run command will be blocked wy the container execution.

To see the containers running in your Docker host you can run `docker ps -a`. The '-a' allows you to see all the containers, not only the running ones. The container must be up and running:

![docker ps -a](https://github.com/PlainConcepts/NetCore-Docker-Workshop/blob/1-start/src/Docu/1.Docker/Img/docker_ps_a.PNG)

Here we see that the internal port is the 80, which is the one that we have set, and externally a rando port has been asigned, in this case the port 32768.

6. Open a browser and write the uri to call our service, which is `http://localhost:#external_port#/api/greetings`. The port is the one that was assigned to our container, in the case of this example the 32768. We can see then the answer from the api:

![browser get master](https://github.com/PlainConcepts/NetCore-Docker-Workshop/blob/1-start/src/Docu/1.Docker/Img/browser_get_Master.PNG)

In the response message we see an exception. This is because the Slave application is not running, and our Master application cannot reach it. 

7. Now we will set a fixed external port for our application in the Docker host. Remember it is random everytime that we create a new container if we do not set it. Execute the next command to fix it to the 5050. The name of the container is changed to 'container2' to avoid a repeated name error:

```
> docker run -d -p 5050:80 --name container2 dockerworkshop/master:manual
```
Now if we do `docker ps` we will see two containers, the one with the random port and another one, the new one, with the port 5050. If we run it in the browser it should show the same json response as the previous one.

![browser get master](https://github.com/PlainConcepts/NetCore-Docker-Workshop/blob/1-start/src/Docu/1.Docker/Img/browser_get_5050_Master.PNG)

8. (Optional) In order to see the logs of the application you can do:
```
> docker logs container_id
```

You can open as well a shell to the container with the command `docker exec -ti <container_id> bash` (if the container is Windows based, put powersehll instead of bash). With a ls we can see that the 'COPY' tag in the Dockerfile worked as expected:

 ![docker exec bash](https://github.com/PlainConcepts/NetCore-Docker-Workshop/blob/1-start/src/Docu/1.Docker/Img/docker_exec_bash.PNG)

 Note that to identify one container we do not need to write the whole id (2f9649994898), but only the first numbers (2f96).

Here some helpful commands to manage your images and containers. You can see the all the information in the [Docker web page](https://docs.docker.com/engine/reference/commandline/docker/):

```
\\ When writting an image or container Id, only with a part of the whole id is enough, no need to write the whole name.

\\ Shows list of all the running containers
> docker ps 

\\ Shows running and stopped containers
> docker ps -a

\\ Stops a container
> docker stop container_id

\\ Stops all the running containers
> docker stop $(docker ps -a -q) ($ substitution only supported with Powershell)

\\ Start an stopped container
> docker start container_id

\\ Remove a container (it has to be stopped)
> docker rm container_id

\\ Remove a container (even if not stopped)
> docker rm -f container_id

\\ Remove all the containers
> docker rm $(docker ps -a -q)

\\ Shows the images
> docker images

\\ Shows all the images, intermediates as well
> docker images -a

\\ Remove one image (required to remove first all the containers)
> docker rmi image_id

\\ Remove all the images
> docker rmi $(docker images -a -q) ($ substitution only supported with Powershell)
``` 

## Creating an image from the Slave application

Repeat the same process for the project Slave. At the end you must have the 2 images generated, Master and Slave. To check if the Slave is up and running you can go to the browser and write `http://localhost:#external_port#/api/identification`

![browser get slave](https://github.com/PlainConcepts/NetCore-Docker-Workshop/blob/1-start/src/Docu/1.Docker/Img/browser_get_slave.PNG)

One last step for connecting the 2 services is to configure for the Master application the URI for the slave. This is set by a parameter. With Docker the parameters are configured by environment variables. We can set the environment variable for a container through the parameter -e when creating the container.

```
> docker run -d -p 5050:80 -e "MasterSettings__SlaveUri=http://<slave_ip>:80" --name master1 dockerworkshop/master:manual
```

The port for the parameter is the 80 because now we are inside the docker host network, and there the port exposed by our container is the 80. The external port is only to external connections. To get the slave IP we have two options, get it directly from the container, or use a DNS and call the container by its name instead of by their IP.

### Obtain the IP for the slave 

We can check the IP for a container with `docker inspect <container_Id>`. The inspect gives us information about the container, including its IP

```
(...)

 "NetworkID": "5b6a1be53184caf9bb80bd3bcb4ed2032caaef6d7b1c757f56c8dc743eaab0fd",
 "EndpointID": "5670923c7247d0625bfb36fac106441f6785ff2041742afd80a7fc56c80d6118",
 "Gateway": "172.19.0.1",
 "IPAddress": "172.19.0.3",
 "IPPrefixLen": 16,

(...)
```
In this case the ip for slave is '172.19.0.3', so we would execute `docker run -d -p 5050:80 -e "MasterSettings__SlaveUri=http://172.19.0.3:80" --name master1 dockerworkshop/master:manual`

Now if we call master from the browser the message shoud include the slave response

![browser get master connected to slave](https://github.com/PlainConcepts/NetCore-Docker-Workshop/blob/1-start/src/Docu/1.Docker/Img/browser_get_Master_connected.PNG)

### Enable name resolution for the Docker network

By default when we create a container it is connected to the default network, called `bridge`. This default network does not support service discovery. To resolve the IP address by container name, we need to create a new network. The easiest way is to create the network and then when we create the containers add the parameter `--net` to select our network:

```
# create network called 'mynet'
docker network create mynet

# create the slave container connected to the new network
docker run -d -p 5051:80 --net mynet --name slave dockerworkshop/slave:manual

# create the master container connected to the new network and with the name of the slave container instead of its ip
docker run -d -p 5050:80 -e "MasterSettings__SlaveUri=http://slave:80" --net mynet --name master1 dockerworkshop/master:manual
```

