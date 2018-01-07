

# Removing previous services & deployments
Write-Host "Removing existing services & deployments.." -ForegroundColor Yellow
kubectl delete deployments --all
kubectl  delete services --all
kubectl delete configmap config-files
kubectl delete configmap externalcfg

kubectl create configmap config-files --from-file=nginx-conf=nginx.conf
kubectl label configmap config-files app=myapp

Write-Host 'Creating services..' -ForegroundColor Yellow
kubectl create -f services.yaml -f frontend.yaml

if ([string]::IsNullOrEmpty($externalDns)) {
        Write-Host "Waiting for frontend's external ip..." -ForegroundColor Yellow
        while ($true) {
            $frontendUrl = kubectl get svc frontend -o=jsonpath="{.status.loadBalancer.ingress[0].ip}"
            if ([bool]($frontendUrl -as [ipaddress])) {
                break
            }
            Start-Sleep -s 15
        }
        $externalDns = $frontendUrl
}

Write-Host "The K8s public IP is $externalDns"

Write-Host "Deploying configuration from conf.yaml"
kubectl create -f conf.yaml

Write-Host " Creating deployments.."
kubectl create -f deployments.yaml

Write-Host "Execute rollout.."
kubectl rollout resume deployments/master-v1
kubectl rollout resume deployments/slave-v1

Write-Host "Master API is exposed at http://$($externalDns)/master-api/, Slave API at http://$($externalDns)/slave-api/" -ForegroundColor Yellow

