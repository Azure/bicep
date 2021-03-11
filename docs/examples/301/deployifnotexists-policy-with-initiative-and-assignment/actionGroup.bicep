// DEPLOYMENT SCOPE
targetScope = 'resourceGroup'

// PARAMETERS
param actionGroupName string = 'AzureSpringCleanAG'
param actionGroupEnabled bool = true
param actionGroupShortName string = 'azspgcln'
param actionGroupEmailName string = 'jloudon'
param actionGroupEmail string = 'jesse.loudon@lab3.com.au'
param actionGroupAlertSchema bool = true

// VARIABLES

// OUTPUTS
output actionGroupId string = actionGroup.id
output actionGroupName string = actionGroup.name

// RESOURCES
resource actionGroup 'microsoft.insights/actionGroups@2019-06-01' = {
  location: 'global'
  name: actionGroupName
  properties: {
    enabled: actionGroupEnabled
    groupShortName: actionGroupShortName
    emailReceivers: [
      {
        name: actionGroupEmailName
        emailAddress: actionGroupEmail
        useCommonAlertSchema: actionGroupAlertSchema
      }
    ]
  }
}