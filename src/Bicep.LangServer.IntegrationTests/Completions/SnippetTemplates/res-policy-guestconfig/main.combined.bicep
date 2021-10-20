// $1 = 'name'
// $2 = guestConfigAssignment
// $3 = 'name'
// $4 = 'configurationName'
// $5 = 'ApplyAndMonitor'
// $6 = '1.*'

param location string

resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name'
  location: location
}

resource guestConfigAssignment 'Microsoft.GuestConfiguration/guestConfigurationAssignments@2020-06-25' = {
  name: 'name'
  scope: virtualMachine
  location: location
  properties: {
    guestConfiguration: {
      name: 'configurationName'
      assignmentType: 'ApplyAndMonitor'
      version: '1.*'
    }
  }
}
// Insert snippet here

