﻿// $1 = 'name'
// $2 = 'guestConfigAssignment'
// $3 = 'name'
// $4 = 'configurationName'
// $5 = 'Audit','ApplyAndMonitor','ApplyAndAutoCorrect'
// $6 = 'version'

resource arcEnabledMachine 'Microsoft.HybridCompute/machines@2021-05-20' = {
  name: 'name'
  location: resourceGroup().location
}

resource guestConfigAssignment 'Microsoft.GuestConfiguration/guestConfigurationAssignments@2020-06-25' = {
  name: 'name'
  scope: arcEnabledMachine
  location: resourceGroup().location
  properties: {
    guestConfiguration: {
      name: 'configurationName'
      assignmentType: 'ApplyAndMonitor'
      version: '1.*'
    }
  }
}


// Insert snippet here
