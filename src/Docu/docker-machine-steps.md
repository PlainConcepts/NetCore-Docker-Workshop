# Create a VM using docker-machine

Ensure you are logged in the desired subscription Refer to [this article](https://docs.microsoft.com/en-us/cli/azure/authenticate-azure-cli) for more details.

1. Install az cli 2.0 [details](https://docs.microsoft.com/es-es/cli/azure/install-azure-cli?view=azure-cli-latest)
2. Use `az account show` to find your subscription id.
3. Use `docker-machine create --driver azure --azure-subscription-id <subs_id> --azure-resource-group <resource_group> --azure-open-port 5000 --azure-ssh-user <login_name> <machine_name>`

After use `docker-machine create` you'll need to authenticate in Azure (even thought if you are logged using `az`, because this is not an Azure CLI 2.0 command). This command will fully create the VM with all the needed settings to run Docker.

**Note** Refer to this article with all the [parameters that docker-machine accepts when creating Azure VMs](https://docs.docker.com/machine/drivers/azure/#options) for finding more parameters.

## Connecting your local environment with docker host running on the VM

Using docker-machine you control the remote VM from your local development environment (you don't need to use ssh to login to remote VM).

Connecting your local environment to a remote host is using by setting some environment variables, but the easiest way is to use again the docker-machine command. Just type `docker-machine env machine_name` (where machine_name is the name you gave when you created the VM). That command **do not change anything**, so do'nt do really nothing, but **outputs the environment variables you have to set**. This is the output of the command (running on a windows workstation):

```
SET DOCKER_TLS_VERIFY=1
SET DOCKER_HOST=tcp://104.42.236.237:2376
SET DOCKER_CERT_PATH=C:\Users\etoma\.docker\machine\machines\ufohost
SET DOCKER_MACHINE_NAME=ufohost
SET COMPOSE_CONVERT_WINDOWS_PATHS=true
REM Run this command to configure your shell:
REM     @FOR /f "tokens=*" %i IN ('docker-machine env ufohost') DO @%i
```

You have to set all these environment variables, or (as the command suggest) just copy and paste the last line in your terminal.

Once you did this, your local development machine is connected to VM running Docker on Azure: all docker and docker-compose commands will run in the VM instead of your local Docker machine!

### Build and execute the services remotely on the vm

1. Create docker images on the vm
```
docker-compose build
```
2. Check images are created
```
docker images
```
3. Run the services on the vm
```
docker-compose up
```
4. Retrieve the public IP of the docker host
```
docker-machine ip machine_name
```
5. Connect to your Api service
```
http://<public_ip>:5000/api/greetings
```