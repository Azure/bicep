// Resource Group
resource /*${1:resourceGroup}*/ resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: /*${2:'name'}*/ 'name'
  location: location
}
