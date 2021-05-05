// Automation Schedule
resource automationAccount 'Microsoft.Automation/automationAccounts@2015-10-31' = {
  name: ${1:'name'}
}

resource ${2:automationSchedule} 'Microsoft.Automation/automationAccounts/schedules@2015-10-31' = {
  parent: automationAccount
  name: ${3:'name'}
  properties: {
    description: ${4:'description'}
    startTime: ${5:'startTime'}
    interval: ${6:'interval'}
    frequency: '${7|OneTime,Day,Hour,Week,Month|}'
  }
}
