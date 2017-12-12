Param(
    [parameter(Mandatory=$true)][string]$resourceName
)

az group create --location westus --name $resourceName