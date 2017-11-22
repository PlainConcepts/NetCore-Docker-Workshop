
# Kubernetes deployment
In this section we will create an Azure Container service for Kubernetes and deploy Master and Slave containers.

## Create Kubernetes Cluster
 Follow the next steps:

1. Install the k8s cli by downloading the kubectl.exe from "https://storage.googleapis.com/kubernetes-release/release/v1.8.0/bin/windows/amd64/kubectl.exe" and put it into C:\Users\<user>\kubectl\ directory. Add the kubectl directory path to env. variables Path.

2. Go to the "NetCore-Docker-Workshop\deploy\k8s\win scripts" directory and execute the following command to create a k8s cluster:

```C#
.\gen-k8s-env.ps1 -resourceGroupName sage-k8s -location westeurope -orchestratorName sagek8s -dnsName sagek8s
```
3. Check that the cluster is created with the following command:
```C#
kubectl get nodes
```
## Deploy Master and Slave containers
1. Execute the following script:

```C#
Deploy.ps1
```
2. After having deployed the containers, execute the command shown bellow and hit the url "http://localhost:8001/api/v1/namespaces/kube-system/services/kubernetes-dashboard/proxy/" to access to the kubernetes admin page

```C#
kubectl proxy
```
3. Scale your services with the following command:

```C#
kubectl scale --replicas=2 deployments/master
```
Check that the Master guid that is shown in the browser is changing depending on which instance the request is sent to.
