// $1 = 'name'
// $2 = 'guestConfigAssignment'
// $3 = 'name'
// $4 = 'configurationName'
// $5 = 'Audit','ApplyAndMonitor','ApplyAndAutoCorrect'

// Guest configuration assignment for virtual machine
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
      name: /*${3:'configurationName'}*/'configurationName'
      assignmentType: /*${4|'Audit','ApplyAndMonitor','ApplyAndAutoCorrect'|}*/'ApplyAndMonitor'
      version: /*${5:'version'}*/'1.*'
    }
  }
}

// Insert snippet here
