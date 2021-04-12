// ****************************************
// Azure Bicep Module:
// Create multiple groups from sample JSON records
// ****************************************
@minLength(1)
param apimInstanceName string
var groupsSet = [
  {
    'groupName': 'APIMGroup1'
    'groupDisplayName': 'APIM Group 1'
    'groupDescription': 'Description for this group'
  }
  {
    'groupName': 'APIMGroup2'
    'groupDisplayName': 'APIM Group 2'
    'groupDescription': 'Description for this group'
  }
]

//APIM Groups
resource apimGroup 'Microsoft.ApiManagement/service/groups@2020-06-01-preview' = [for grp in groupsSet: {
  name: '${apimInstanceName}/${grp.groupName}'
  properties: {
    displayName: grp.groupDisplayName
    description: grp.groupDescription
  }
}]

output apimGroups array = [for (name, i) in groupsSet: {
  groupId: apimGroup[i].id
  groupName: apimGroup[i].name
}]
