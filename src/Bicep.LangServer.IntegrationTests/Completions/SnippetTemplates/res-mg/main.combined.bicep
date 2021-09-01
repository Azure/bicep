// $1 = managementGroup
// $2 = 'name'
// $3 = 'displayName'
// $4 = 'id'
targetScope = 'tenant'
resource managementGroup 'Microsoft.Management/managementGroups@2021-04-01' = {
  name: 'name'
  properties: {
    displayName: 'displayName'
    details: {
      parent: {
        id: 'id'
      }
    }
  }
}
// Insert snippet here

