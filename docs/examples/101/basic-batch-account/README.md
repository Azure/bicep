# Azure Batch Account

This Bicep template deploys an Azure Batch account in Batch Service allocation mode.

## Deployment steps ##

* [Install the Bicep CLI](https://docs.microsoft.com/azure/azure-resource-manager/bicep/install) by following the instruction.
* Build the `main.bicep` file by running the Bicep CLI command:
  
```bash
bicep build ./main.bicep

New-AzResourceGroupDeployment -TemplateFile ./main.json -ResourceGroupName <resource group name> -Verbose
```