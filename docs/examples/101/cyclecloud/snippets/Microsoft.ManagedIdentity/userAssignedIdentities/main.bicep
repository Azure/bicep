resource mid 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30'= {
  name: 'exampleManagedIdentity'
  location: 'eastus'
  tags: {
      TagA: 'Value A'
      TagB: 'Value B'
  }
}