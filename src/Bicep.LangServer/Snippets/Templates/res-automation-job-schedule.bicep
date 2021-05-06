// Automation Job Schedule
resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: ${1:'name'}
}

resource ${2:automationJobSchedule} 'Microsoft.Automation/automationAccounts/jobSchedules@2019-06-01' = {
  parent: automationAccount
  name: ${3:'name'}
  properties: {
    schedule: {
      name: ${4:'name'}
    }
    runbook: {
      name: ${5:'name'}
    }
  }
}
