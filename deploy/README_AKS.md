
# Kubernetes deployment
In this section we will create an Azure Kubernetes Service (AKS) and deploy Master and Slave containers.

Requirements:
- Install Azure cli 2.0[Az Tools installation readme](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest) 
- Install Kubernetes cli [Az Tools installation readme](https://kubernetes.io/docs/tasks/tools/install-kubectl/) 

NOTE: Optionally you can skip previous steps by launching a container which already contains these tools:
```C#
docker run -v ${env:UserProfile}:/k8scripts -it ramontc/k8stools
```

## Create Kubernetes Cluster
 Follow the next steps:

1. Log in your Azure subscription:

```C#
az login
```

2. Enable AKS for your Azure subscription:

```C#
az provider register -n Microsoft.ContainerService
```

3. Create AKS cluster:

```C#
az aks create -g myResourceGroup -n myK8sCluster --generate-ssh-keys --kubernetes-version 1.8.1
```

4. Install kubernetes cli to connect to the cluster and download credentials:
```C#
az aks install-cli
az aks get-credentials -g myResourceGroup -n myK8sCluster
```

## Deploy Master and Slave containers
1. Execute the following script:

```C#
deploy\k8s\win scripts\deploy.ps1
```

2. Verify that the cluster nodes are created with the following command:
```C#
kubectl get nodes
```

3. After having deployed the containers, execute the command shown bellow to access to the kubernetes admin page

```C#
az aks browse -g MyResourceGroup -n MyManagedCluster 
```

4. Scale your services with the following command:

```C#
kubectl scale --replicas=2 deployments/master
```
Check that the Master guid that is shown in the browser is changing depending on which instance the request is sent to.
