# CycleCloud install using Bicep + Cloud-Init

With [CycleCloud 8.1](https://techcommunity.microsoft.com/t5/azure-compute/azure-cyclecloud-8-1-is-now-available/ba-p/1898011) now supporting Cloud-Init as a means of configuring VMs it seemed appropriate to look at using cloud-init in the deployment of CycleCloud itself

This example uses [Bicep](https://github.com/Azure/bicep) to deploy the Azure resource and has been tested with v0.2.14 (alpha). Much like Terraform, Bicep drastically simplifies the authoring experience and provides a transparent abstraction over ARM.

Just edit or supply parameters to override the defaults

Deployment steps

```bash
bicep build *.bicep
az deployment sub create --template-file sub.json --location uksouth --confirm-with-what-if
az deployment group create --resource-group rg-bicep --template-file main.json --confirm-with-what-if
```

In this example Modules have been used to separate out the definition of the network and virtual machine resources simplifying the main Bicep template but also enabling me to explore reusing the modules in other deployments
