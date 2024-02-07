param deployAutomationModules string
param automationAccountName string
param location string

var automationModules = {
  modules: [
    {
      Name: 'foo'
      url: 'https://foo.com'
    }
  ]
}

resource automationAccountName_automationModules_modules 'Microsoft.Automation/automationAccounts/modules@2015-10-31' = [
  for i in range(0, length(automationModules.modules)): if (deployAutomationModules == 'true') {
    name: '${automationAccountName}/${automationModules.modules[i].Name}'
    location: location
    properties: {
      contentLink: {
        uri: automationModules.modules[i].url
      }
    }
    dependsOn: [
      resourceId(
//@[6:107) [BCP034 (Error)] The enclosing array expected an item of type "module[] | (resource | module) | resource[]", but the provided item was of type "string". (CodeDescription: none) |resourceId(\n        'Microsoft.Automation/automationAccounts/',\n        automationAccountName\n      )|
        'Microsoft.Automation/automationAccounts/',
        automationAccountName
      )
    ]
  }
]

