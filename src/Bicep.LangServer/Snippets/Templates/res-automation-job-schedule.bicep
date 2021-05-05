// Automation Job Schedule
resource automationAccount 'Microsoft.Automation/automationAccounts@2015-10-31' = {
  name: ${1:'name'}
}

resource ${2:automationJobSchedule} 'Microsoft.Automation/automationAccounts/jobSchedules@2015-10-31' = {
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
