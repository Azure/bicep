// Guest configuration assignment for virtual machine
resource arcEnabledMachine 'Microsoft.HybridCompute/machines@2021-05-20' = {
  name: /*${1:'name'}*/'name'
  location: /*${2:location}*/'location'
}

resource /*${3:guestConfigAssignment}*/guestConfigAssignment 'Microsoft.GuestConfiguration/guestConfigurationAssignments@2020-06-25' = {
  name: /*${4:'name'}*/'name'
  scope: arcEnabledMachine
  location: /*${2:location}*/'location'
  properties: {
    guestConfiguration: {
      name: /*${5:'configurationName'}*/'configurationName'
      assignmentType: /*${6|'ApplyAndMonitor','ApplyAndAutoCorrect','Audit'|}*/'ApplyAndMonitor'
      version: /*${7:'1.*'}*/'1.*'
    }
  }
}
