@description('The name of the Resource Group the VM(s) was/were created in.')
output virtualMachinesResourceGroup string = resourceGroup().name
