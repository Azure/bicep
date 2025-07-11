@description('Name of skuName')
@allowed([
  'Premium_AzureFrontDoor'
  'Standard_AzureFrontDoor'
])
param skuName string

@description('Name of cdnProfileName')
@metadata({ other: 'other metadata' })
param cdnProfileName string

output output1 string = concat(cdnProfileName, skuName)
//@[24:55) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(cdnProfileName, skuName)|

