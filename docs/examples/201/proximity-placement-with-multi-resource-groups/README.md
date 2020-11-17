# Proximity Placement With Multi Resource Groups
The example builds on the [Anchored Proximity Placement Groups containing Availability Sets](https://github.com/Azure/bicep/tree/main/docs/examples/201/anchored-proximity-placement-group) example splitting the deployment across three resource groups; shared network resources, anchored proximity placement group resources, and the HA workload itself.

This example uses [Bicep](https://github.com/Azure/bicep) to deploy the Azure resources and demonstrates the use of [Resource Scopes](https://github.com/Azure/bicep/blob/main/docs/spec/resource-scopes.md). This enables a simple definition of multiple resource groups and the resources that should be deployed into each which can be deployed in a single deployment. It has been tested with v0.2.14 (alpha). 

Just edit or supply parameters to override the defaults.

Deployment steps
```
bicep build *.bicep
az deployment sub create --location uksouth --template-file main.json --confirm-with-what-if
```

The [--confirm-with-what-if](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-deploy-what-if?tabs=azure-powershell#install-azure-cli-module) Argument has been added which you are encouraged to explore. This lets you preview the changes that will happen before deploying an Azure Resource Manager template.
