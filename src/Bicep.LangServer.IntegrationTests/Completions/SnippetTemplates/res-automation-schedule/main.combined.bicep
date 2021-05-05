resource automationAccount 'Microsoft.Automation/automationAccounts@2015-10-31' = {
  name: 'name'
}

resource automationSchedule 'Microsoft.Automation/automationAccounts/schedules@2015-10-31' = {
  parent: automationAccount
  name: 'name'
  properties: {
    description: 'description'
    startTime: 'startTime'
    interval: 'interval'
    frequency: 'OneTime'
  }
}

