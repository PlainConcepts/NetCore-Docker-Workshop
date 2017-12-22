# AppService deployment steps

This section is intended to deploy our containerized applications in Azure hosting the apps to an AppService.

1. Run container with AZ tools

```
docker run -v ${env:UserProfile}:/azscripts -it azuresdk/azure-cli-python:latest 
```

2. Access to the container which contains the AZ Tools

```
docker exec -it <container-ID> sh
```

3. Login to Azure

```
az login
```

4. Navigate to the directory 'deploy' inside your container.

5. Execute the script 'create-resource-group' in order to create an Azure resource group in your subscription.

```
./create-resource-group.sh <your-resourceGroup-name>
```

6. Execute the script 'create-appservices' located at deploy/appservice directory in order to create an Azure App Service for your containerized apps in the resource group previously created.

```
./create-appservices <your-resourceGroup-name>
```
