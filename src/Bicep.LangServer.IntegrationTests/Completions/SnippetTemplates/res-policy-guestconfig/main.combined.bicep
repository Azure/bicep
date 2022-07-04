// $1 = 'name'
// $2 = location
// $3 = guestConfigAssignment
// $4 = 'name'
// $5 = 'configurationName'
// $6 = 'ApplyAndMonitor'
// $7 = '1.*'

param location string

resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name'
  location: location
}

resource guestConfigAssignment 'Microsoft.GuestConfiguration/guestConfigurationAssignments@2022-01-25' = {
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

