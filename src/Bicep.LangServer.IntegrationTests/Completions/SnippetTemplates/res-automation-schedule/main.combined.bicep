resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: 'name'
}

resource automationSchedule 'Microsoft.Automation/automationAccounts/schedules@2019-06-01' = {
  parent: automationAccount
  name: 'name'
  properties: {
    description: 'description'
    startTime: 'startTime'
    interval: 'interval'
    frequency: 'OneTime'
  }
}

