# AADDS install with dual region replica sets

This example updates https://docs.microsoft.com/en-us/azure/active-directory-domain-services/template-create-instance to demonstrate how Bicep can install AADDS with the new preview features that now support multi region replica sets.

The bicep template deploys two globally peered VNETs, in East US and West Europe, and adds replicas in each region.

The deployment steps in [Azure Docs](https://docs.microsoft.com/en-us/azure/active-directory-domain-services/template-create-instance) are still valid though this update requires a different AppID detailed below in the deployment steps.

Deployment steps

```bash
az ad sp create --id "2565bd9d-da50-47d4-8b85-4c97f669dc36"
az group create -n aadds-rg -l 'East US'
bicep build main.bicep 
az deployment group create --resource-group aadds-rg --template-file main.json --confirm-with-what-if 
```
