// $1 = 'name'
// $2 = automationSchedule
// $3 = 'name'
// $4 = 'description'
// $5 = 'startTime'
// $6 = 'interval'
// $7 = OneTime

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
// Insert snippet here

