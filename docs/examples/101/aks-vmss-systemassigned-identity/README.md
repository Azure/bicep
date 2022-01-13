# Azure Kubernetes Service

This Bicep template deploys a managed **Azure hosted Kubernetes cluster** via **Azure Kubernetes Service (AKS)** with **Virtual Machine Scale Sets** Agent Pool and **System-assigned managed identity**.

## Deployment steps ##

* [Install the Bicep CLI](https://docs.microsoft.com/azure/azure-resource-manager/bicep/install) by following the instruction.
* Create Resource Group
```
az group create -n <resource-group-name> -l westus
```
* Execute deployment
```
 az deployment group create -g <resource-group-name> -f .\main.bicep
```