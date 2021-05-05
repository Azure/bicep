// Automation Schedule
resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: ${1:'name'}
}

resource ${2:automationSchedule} 'Microsoft.Automation/automationAccounts/schedules@2019-06-01' = {
  parent: automationAccount
  name: ${3:'name'}
  properties: {
    description: ${4:'description'}
    startTime: ${5:'startTime'}
    interval: ${6:'interval'}
    frequency: '${7|OneTime,Day,Hour,Week,Month|}'
  }
}
