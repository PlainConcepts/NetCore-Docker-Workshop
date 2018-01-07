# KUBERNETES WORKSHOP

In this section we will create an Azure Kubernetes Service (AKS) and deploy Master and Slave containers.

Requirements:
- Install Azure cli 2.0 [Az Tools installation readme](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest) 
- Install Kubernetes cli [Kubernetes cli installation readme](https://kubernetes.io/docs/tasks/tools/install-kubectl/) 

NOTE: Optionally you can skip previous steps and launch a container which already contains these tools:
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
Verify that the cluster nodes are created with the following command:
```C#
kubectl get nodes
```

Verify that the deployment rollout has succeded:
```C#
kubectl rollout status deployments/master-v1
kubectl rollout status deployments/slave-v1
```

2. After having deployed the containers, execute the command shown bellow to access to the kubernetes admin page

```C#
az aks browse -g MyResourceGroup -n MyManagedCluster 
```

3. Scale your services with the following command:

```C#
kubectl scale --replicas=2 deployments/master-v1
kubectl scale --replicas=2 deployments/slave-v1
```
Verify in the Replication controller that the instances are ready with the following command:

```C#
kubectl get rs
```
Verify that the version is v1 and the Master guid that is shown in the browser is changing depending on which instance the request is sent to.


## Deployment strategies

### Rolling Update Deployment

By default k8s uses Rolling Update deployment strategy which kills and created new Pods gradually.

### Recreate Deployment

With Recreate deployment all existing Pods are killed before new ones are created

### Blue/green Deployment

Another approach is to follow a Blue/green deployment and just redirect traffic directly to the new version. In case of having issues with the new version, you can simply point to the old version.

### Canary Deployment

In this section it is shown an example of a canary deployment. When deploying a new version, both old and new versions receive live production traffic before fully rolling out the new version. 

1. Deploy version 2:

```C#
kubectl create -f .\deployments-v2.yaml
kubectl rollout resume deployments/master-v2
kubectl rollout resume deployments/slave-v2
```
Verify that both versions v1 and v2 coexist by making multiple requests to master and checking the response.

2. Switch to version 2. Execute the following command and set the label selector '**version: v1**' to v2 in services definition.

```C#
kubectl edit svc/master
kubectl edit svc/slave
```
NOTE: use kubectl patch for modifying fields as non-interactive mode

Verify that requests are redirect to version 2.


## Rolling Back deployments

1. Update slave-v2 by setting a wrong image 

```C#
kubectl set image deployment/slave-v2 slave=slave-wrong_image
```

Check the status of the pod slave-v2 to verify that it has really failed

```C#
>kubectl get pods
NAME                         READY     STATUS             RESTARTS   AGE
frontend-84fdc95c76-t4wxd    1/1       Running            0          1h
master-v1-5d76bcdff9-wb8wn   1/1       Running            1          26m
master-v2-67cd948d99-rqkzt   1/1       Running            3          45m
slave-v1-59f9c47fd6-wl872    1/1       Running            0          26m
slave-v2-dd747bfd9-g85sw     0/1       ImagePullBackOff   0          4m
```
Check the history of versions applied to slave-v2 deployment

```C#
>kubectl rollout history deployment/slave-v2
deployments "slave-v2"
REVISION        CHANGE-CAUSE
1               <none>
2               <none>
```

Rollback slave-v2 to previous version

```C#
>kubectl rollout undo deployment/slave-v2 --to-revision=1
deployment "slave-v2" rolled back
```

## Node affinity

Deploy all pods with version 2 to the node 0 in k8s cluster

1. Retrieve nodes available

```C#
>kubectl get nodes
NAME                       STATUS    AGE       VERSION
aks-nodepool1-76005534-0   Ready     19h       v1.8.1
aks-nodepool1-76005534-1   Ready     19h       v1.8.1
aks-nodepool1-76005534-2   Ready     19h       v1.8.1
```
2. Add label '**version=v2**' to node 0

```C#
>kubectl label nodes aks-nodepool1-76005534-0 version=v2
node "aks-nodepool1-76005534-0" labeled
```

3. Deploy pods

```C#
>kubectl apply -f .\deployments-v2-nodeAffinity.yaml
deployment "master-v2" created
deployment "slave-v2" created

>kubectl rollout resume deployments/master-v2
deployment "master-v2" resumed

>kubectl rollout resume deployments/slave-v2
deployment "slave-v2" resumed
```
4. Verify that all pods with label v2 have being deployed to node 'aks-nodepool1-76005534-0'

```C#
>kubectl get pods -l version=v2 -o wide
NAME                        READY     STATUS    RESTARTS   AGE       IP            NODE
master-v2-74b8f6f48-6hcmg   1/1       Running   0          4m        10.244.2.12   aks-nodepool1-76005534-0
master-v2-74b8f6f48-9mstd   1/1       Running   0          4m        10.244.2.14   aks-nodepool1-76005534-0
master-v2-74b8f6f48-ctgxq   1/1       Running   0          4m        10.244.2.13   aks-nodepool1-76005534-0
slave-v2-7666ddbf85-ngxft   1/1       Running   0          4m        10.244.2.15   aks-nodepool1-76005534-0
```