// Automation Job Schedule
resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: /*${1:'name'}*/'name'
}

resource /*${2:automationJobSchedule}*/automationJobSchedule 'Microsoft.Automation/automationAccounts/jobSchedules@2019-06-01' = {
  parent: automationAccount
  name: /*${3:'name'}*/'name'
  properties: {
    schedule: {
      name: /*${4:'name'}*/'name'
    }
    runbook: {
      name: /*${5:'name'}*/'name'
    }
  }
}
