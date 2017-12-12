# Create an Azure private repository ACR

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

4. Execute the following command to create an ACR.

```
az acr create --name <your_acr_name> --resource-group <your_resourcegroupName> --sku Basic
```

# Publish your images to ACR

1. Open a new CMD locally and login to your ACR

```
docker login -u <acr_user> -p <acr_pwd> <registry_server_name>
```

2. Tag the image before publishing it to ACR

```
docker tag <local_image_name> <registry_server_name>/<your_published_image_name>
```

3. Push images to ACR

```
docker push <registry_server_name>/<your_published_image_name>
```