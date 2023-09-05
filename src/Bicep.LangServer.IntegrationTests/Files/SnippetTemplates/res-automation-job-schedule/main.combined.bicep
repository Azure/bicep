// $1 = 'name'
// $2 = automationJobSchedule
// $3 = 'name'
// $4 = 'name'
// $5 = 'name'

resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: 'name'
}

resource automationJobSchedule 'Microsoft.Automation/automationAccounts/jobSchedules@2019-06-01' = {
  parent: automationAccount
  name: 'name'
//@[8:14) [BCP333 (Warning)] The provided value (whose length will always be less than or equal to 4) is too short to assign to a target for which the minimum allowable length is 36. (CodeDescription: none) |'name'|
  properties: {
    schedule: {
      name: 'name'
    }
    runbook: {
      name: 'name'
    }
  }
}


