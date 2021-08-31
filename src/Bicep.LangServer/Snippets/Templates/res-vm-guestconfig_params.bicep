// Guest configuration assignment with parameters, for virtual machine
resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: /*${1:'name'}*/'name'
  location: resourceGroup().location
  identity: {
    type:'SystemAssigned'
  }
}

resource /*${2:windowsVMGuestConfigAssignment}*/windowsVMGuestConfigAssignment 'Microsoft.GuestConfiguration/guestConfigurationAssignments@2020-06-25' = {
  name: /*${3:'name'}*/'name'
  scope: virtualMachine
  location: resourceGroup().location
  properties: {
    guestConfiguration: {
      name: /*${3:'configurationName'}*/'configurationName'
      assignmentType: /*${4|'Audit','ApplyAndMonitor','ApplyAndAutoCorrect'|}*/'ApplyAndMonitor'
      version: /*${5:'version'}*/'1.*'
      configurationParameter: [
        {
          name: /*${6:'parameter1[dscResourceType]dscResourceName;propertyName'}*/'parameter1[dscResourceType]dscResourceName;propertyName'
          value: /*${7:'parameter1Value'}*/'parameter1Value'
        }
        {
          name: /*${6:'parameter2[dscResourceType]dscResourceName;propertyName'}*/'parameter2[dscResourceType]dscResourceName;propertyName'
          value: /*${7:'parameter2Value'}*/'parameter2Value'
        }
      ]
    }
  }
}
