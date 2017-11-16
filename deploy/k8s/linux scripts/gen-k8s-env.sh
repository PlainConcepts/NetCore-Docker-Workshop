
az acs create --orchestrator-type=kubernetes --resource-group $1 --name=$2 --dns-prefix=$1 --generate-ssh-keys

# Retrieve kubernetes cluster configuration and save it under ~/.kube/config 
az acs kubernetes get-credentials --resource-group=$1 --name=$2
