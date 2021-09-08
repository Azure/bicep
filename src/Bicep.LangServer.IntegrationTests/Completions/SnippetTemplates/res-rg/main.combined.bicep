// $1 = resourceGroup
// $2 = 'name'
// $3 = 'westeurope'
targetScope = 'subscription'
resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: 'name'
  location: 'westeurope'
  tags:{
    'tag': 'tagValue'   
  }
}
// Insert snippet here

