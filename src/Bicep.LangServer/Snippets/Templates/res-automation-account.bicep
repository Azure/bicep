﻿// Automation Account
resource ${1:automationAccount} 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    sku: {
      name: ${3|'Free','Basic'|}
    }
  }
}
