// $1 = 'name'
// $2 = 'guestConfigAssignment'
// $3 = 'name'
// $4 = 'configurationName'
// $5 = 'Audit','ApplyAndMonitor','ApplyAndAutoCorrect'
// $6 = 'version'

// Guest configuration assignment for virtual machine
resource arcEnabledMachine 'Microsoft.HybridCompute/machines@2021-05-20' = {
  name: /*${1:'name'}*/'name'
  location: resourceGroup().location
}

resource /*${2:guestConfigAssignment}*/guestConfigAssignment 'Microsoft.GuestConfiguration/guestConfigurationAssignments@2020-06-25' = {
  name: /*${3:'name'}*/'name'
  scope: arcEnabledMachine
  location: resourceGroup().location
  properties: {
    guestConfiguration: {
      name: /*${4:'configurationName'}*/'configurationName'
      assignmentType: /*${5|'Audit','ApplyAndMonitor','ApplyAndAutoCorrect'|}*/'ApplyAndMonitor'
      version: /*${6:'version'}*/'1.*'
    }
  }
}


// Insert snippet here
