// Management Group (for tenant scope)
resource /*${1:'managementGroup'}*/managementGroup 'Microsoft.Management/managementGroups@2021-04-01' = {
  name: /*${2:'name'}*/'name'
  properties: {
    displayName: /*${3:'displayName'}*/'displayName'
    details: {
      parent: {
        id: /*${4:'id'}*/'id'
      }
    }
  }
}
