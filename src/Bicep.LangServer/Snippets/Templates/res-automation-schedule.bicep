// Automation Schedule
resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: /*${1:'name'}*/'name'
}

resource /*${2:automationSchedule}*/automationSchedule 'Microsoft.Automation/automationAccounts/schedules@2019-06-01' = {
  parent: automationAccount
  name: /*${3:'name'}*/'name'
  properties: {
    description: /*${4:'description'}*/'description'
    startTime: /*${5:'startTime'}*/'startTime'
    interval: /*${6:'interval'}*/'interval'
    frequency: /*'${7|OneTime,Day,Hour,Week,Month|}'*/'OneTime'
  }
}
