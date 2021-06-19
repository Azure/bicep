# Budgets
This module will deploy a cost Budget at the subscription level with notifications.

## Usage

### Example 1 - Budget with notifications
``` bicep
targetScope = 'subscription'

param deploymentName string = 'budget${utcNow()}'

module budget './main.bicep' = {
  name: deploymentName
  params: {
    name: 'MyBudget'
    amount: 1000
    resetPeriod: 'Monthly'
    startDate: '2021-07-01'
    notifications: {
      notification1: {
        enabled: true
        operator: 'GreaterThan'
        threshold: 50
        contactEmails: [
          'john.smith@microsoft.com'
        ]
        contactRoles: [
          'Owner'
        ]
        contactGroups: []
      }
      notification2: {
        enabled: true
        operator: 'GreaterThan'
        threshold: 90
        contactEmails: [
          'john.smith@microsoft.com'
        ]
        contactRoles: [
          'Owner'
        ]
        contactGroups: []
      }
    }
  }
}
```

### Example 2 - Budget with end date and without notifications
``` bicep
targetScope = 'subscription'

param deploymentName string = 'budget${utcNow()}'

module budget './main.bicep' = {
  name: deploymentName
  params: {
    name: 'MyBudget'
    amount: 1000
    resetPeriod: 'Monthly'
    startDate: '2021-07-01'
    endDate: '2021-12-01'
  }
}
```