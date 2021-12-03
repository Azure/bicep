// $1 = 'name'
// $2 = guestConfigAssignment
// $3 = 'name'
// $4 = 'configurationName'
// $5 = 'ApplyAndMonitor'
// $6 = '1.*'
// $7 = 'parameter1[dscResourceType]dscResourceName;propertyName'
// $8 = 'parameter1Value'
// $9 = 'parameter2[dscResourceType]dscResourceName;propertyName'
// $10 = 'parameter2Value'

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

