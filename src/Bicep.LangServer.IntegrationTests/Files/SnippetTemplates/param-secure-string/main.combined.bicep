// $1 = location

@secure()
param location string

resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: 'name'
  location: location
//@[12:20) [BCP417 (Warning)] The supplied value has been marked as secure but is being assigned to a target that is not expecting sensitive data. (bicep https://aka.ms/bicep/core-diagnostics#BCP417) |location|
  sku: {
    name: 'F1'
    capacity: 1
  }
}

