// Guest configuration assignment for virtual machine
resource virtualMachine 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: /*${1:'name'}*/'name'
  location: resourceGroup().location
}

resource /*${2:guestConfigAssignment}*/guestConfigAssignment 'Microsoft.GuestConfiguration/guestConfigurationAssignments@2020-06-25' = {
  name: /*${3:'name'}*/'name'
  scope: virtualMachine
  location: resourceGroup().location
  properties: {
    guestConfiguration: {
      name: /*${4:'configurationName'}*/'configurationName'
      assignmentType: /*${5|'ApplyAndMonitor','ApplyAndAutoCorrect','Audit'|}*/'ApplyAndMonitor'
      version: /*${6:'1.*'}*/'1.*'
    }
  }
}
