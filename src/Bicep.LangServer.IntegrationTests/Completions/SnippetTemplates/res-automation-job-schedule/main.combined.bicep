resource automationAccount 'Microsoft.Automation/automationAccounts@2015-10-31' = {
  name: 'name'
}

resource automationJobSchedule 'Microsoft.Automation/automationAccounts/jobSchedules@2015-10-31' = {
  parent: automationAccount
  name: 'name'
  properties: {
    schedule: {
      name: 'name'
//@[6:10) [BCP073 (Warning)] The property "name" is read-only. Expressions cannot be assigned to read-only properties. |name|
    }
    runbook: {
      name: 'name'
    }
  }
}

