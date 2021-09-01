// $1 = 'name'
// $2 = 'guestConfigAssignment'
// $3 = 'name'
// $4 = 'configurationName'
// $5 = 'Audit','ApplyAndMonitor','ApplyAndAutoCorrect'
// $6 = 'version'
// $7 = 'parameter1[dscResourceType]dscResourceName;propertyNam'
// $8 = 'parameter1Value'
// $9 = 'parameter2[dscResourceType]dscResourceName;propertyName'
// $10 = 'parameter2Value'

// Guest configuration assignment with parameters, for virtual machine
resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: /*${1:'name'}*/'name'
  location: resourceGroup().location
  identity: {
    type:'SystemAssigned'
  }
}

resource /*${2:guestConfigAssignment}*/guestConfigAssignment 'Microsoft.GuestConfiguration/guestConfigurationAssignments@2020-06-25' = {
  name: /*${3:'name'}*/'name'
  scope: virtualMachine
  location: resourceGroup().location
  properties: {
    guestConfiguration: {
      name: /*${4:'configurationName'}*/'configurationName'
      assignmentType: /*${5|'Audit','ApplyAndMonitor','ApplyAndAutoCorrect'|}*/'ApplyAndMonitor'
      version: /*${6:'version'}*/'1.*'
      configurationParameter: [
        {
          name: /*${7:'parameter1[dscResourceType]dscResourceName;propertyName'}*/'parameter1[dscResourceType]dscResourceName;propertyName'
          value: /*${8:'parameter1Value'}*/'parameter1Value'
        }
        {
          name: /*${9:'parameter2[dscResourceType]dscResourceName;propertyName'}*/'parameter2[dscResourceType]dscResourceName;propertyName'
          value: /*${10:'parameter2Value'}*/'parameter2Value'
        }
      ]
    }
  }
}

// Insert snippet here
