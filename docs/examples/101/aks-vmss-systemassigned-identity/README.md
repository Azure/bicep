# Azure Kubernetes Service

This Bicep template deploys a managed **Azure hosted Kubernetes cluster** via **Azure Kubernetes Service (AKS)** with **Virtual Machine Scale Sets** Agent Pool and **System-assigned managed identity**.

## Deployment steps ##

* [Install the Bicep CLI](https://github.com/Azure/bicep/blob/main/docs/installing.md) by following the instruction.
* Build the `main.bicep` file by running the Bicep CLI command:
  
```bash
bicep build ./main.bicep

New-AzResourceGroup -Name my-rg-aks -Location westeurope
New-AzResourceGroupDeployment -TemplateFile ./main.json -ResourceGroupName my-rg-aks -Verbose
```