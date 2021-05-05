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

