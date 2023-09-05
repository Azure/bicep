// $1 = containerRegistry
// $2 = 'name'
// $3 = location
// $4 = 'Basic'
// $5 = false

param location string

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2021-06-01-preview' = {
  name: 'name'
//@[8:14) [BCP333 (Error)] The provided value (whose length will always be less than or equal to 4) is too short to assign to a target for which the minimum allowable length is 5. (CodeDescription: none) |'name'|
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: false
  }
}


