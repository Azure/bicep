# CycleCloud install using Bicep + Cloud-Init

With CycleCloud 8 now supporting Cloud-Init as a means of configuring VMs it seemed appropriate to look at using cloud-init in the deployment of CycleCloud itself

This exemplar uses [Bicep](https://github.com/Azure/bicep) to deploy the Azure resource and has been tested with v0.1.226-alpha. Much like Terraform, Bicep drastically simplifies the authoring experience and provides a transparent abstraction over ARM.

Just edit or supply parameters to override the defaults

Deployment steps

```bash
bicep build sub.bicep cyclecloud.bicep
az deployment sub create --template-file sub.json --location uksouth
az deployment group create --resource-group rg-bicep --template-file cyclecloud.json
```

One of the great this with Bicep is it determines dependencies so a template such as this can be rapidly created from individual components or snippets. Just copy and paste what you need into a single Bicep file, edit the values to suit your desired configuration and remove unneeded properties.

This example uses the following snippets that may be found in the snippets sub folder:

- Microsoft.Authorization/roleAssignments
- Microsoft.Compute/virtualMachines
- Microsoft.ManagedIdentity/userAssignedIdentities
- Microsoft.Network/networkInterfaces
- Microsoft.Network/networkSecurityGroups
- Microsoft.Network/publicIpAddresses
- Microsoft.Network/virtualNetworks
- Microsoft.Storage/storageAccounts

The sub.bicep file that is used to create the resource group uses:

- Microsoft.Resources/resourceGroups