// $1 = 'name'
// $2 = location
// $3 = guestConfigAssignment
// $4 = 'name'
// $5 = 'configurationName'
// $6 = 'ApplyAndMonitor'
// $7 = '1.*'
// $8 = 'parameter1[dscResourceType]dscResourceName;propertyName'
// $9 = 'parameter1Value'
// $10 = 'parameter2[dscResourceType]dscResourceName;propertyName'
// $11 = 'parameter2Value'

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
      configurationParameter: [
        {
          name: 'parameter1[dscResourceType]dscResourceName;propertyName'
          value: 'parameter1Value'
        }
        {
          name: 'parameter2[dscResourceType]dscResourceName;propertyName'
          value: 'parameter2Value'
        }
      ]
    }
  }
}
// Insert snippet here

