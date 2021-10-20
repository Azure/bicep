// $1 = 'name'
// $2 = automationJobSchedule
// $3 = 'name'
// $4 = 'name'
// $5 = 'name'

param location string
//@[6:14) [no-unused-params (Warning)] Parameter "location" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |location|

resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: 'name'
}

resource automationJobSchedule 'Microsoft.Automation/automationAccounts/jobSchedules@2019-06-01' = {
  parent: automationAccount
  name: 'name'
  properties: {
    schedule: {
      name: 'name'
    }
    runbook: {
      name: 'name'
    }
  }
}
// Insert snippet here

