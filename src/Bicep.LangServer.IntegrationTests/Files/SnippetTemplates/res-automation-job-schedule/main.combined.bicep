// $1 = 'name'
// $2 = automationJobSchedule
// $3 = '00000000-0000-0000-0000-000000000000'
// $4 = 'name'
// $5 = 'name'

resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: 'name'
}

resource automationJobSchedule 'Microsoft.Automation/automationAccounts/jobSchedules@2019-06-01' = {
  parent: automationAccount
  name: '00000000-0000-0000-0000-000000000000'
  properties: {
    schedule: {
      name: 'name'
    }
    runbook: {
      name: 'name'
    }
  }
}


