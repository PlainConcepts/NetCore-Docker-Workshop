export PATH=$PATH:/tools

echo Removing existing services and deployments...

kubectl delete deployments --all
kubectl  delete services --all
kubectl delete configmap config-files
kubectl delete configmap externalcfg

kubectl create configmap config-files --from-file=nginx-conf=nginx.conf
kubectl label configmap config-files app=myapp

kubectl create -f services.yaml -f frontend.yaml

echo Waiting for k8s external ip

external_ip=""
while [ -z $external_ip ]; do
    sleep 10
    external_ip=$(kubectl get svc frontend -o=jsonpath="{.status.loadBalancer.ingress[0].ip}")
done

echo The k8s IP is $external_ip


echo Deploying configuration from conf.yaml

kubectl create -f conf.yaml

echo Creating deployments..

kubectl create -f deployments.yaml

echo Execute rollout...

kubectl rollout resume deployments/master
kubectl rollout resume deployments/slave


