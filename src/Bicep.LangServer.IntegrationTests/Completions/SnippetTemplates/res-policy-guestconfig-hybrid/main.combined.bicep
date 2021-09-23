// $1 = 'name'
// $2 = guestConfigAssignment
// $3 = 'name'
// $4 = 'configurationName'
// $5 = 'ApplyAndMonitor'
// $6 = '1.*'

resource arcEnabledMachine 'Microsoft.HybridCompute/machines@2021-05-20' = {
  name: 'name'
  location: resourceGroup().location
}

resource guestConfigAssignment 'Microsoft.GuestConfiguration/guestConfigurationAssignments@2020-06-25' = {
  name: 'name'
  scope: arcEnabledMachine
  location: resourceGroup().location
  properties: {
    guestConfiguration: {
      name: 'configurationName'
      assignmentType: 'ApplyAndMonitor'
      version: '1.*'
    }
  }
}
// Insert snippet here

