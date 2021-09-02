// $1 = 'name'
// $2 = 'guestConfigAssignment'
// $3 = 'name'
// $4 = 'configurationName'
// $5 = 'Audit','ApplyAndMonitor','ApplyAndAutoCorrect'

resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name'
  location: resourceGroup().location
  identity: {
    type:'SystemAssigned'
  }
}

resource guestConfigAssignment 'Microsoft.GuestConfiguration/guestConfigurationAssignments@2020-06-25' = {
  name: 'name'
  scope: virtualMachine
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
