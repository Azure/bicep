# Simplify interpolation

**Code**: simplify-interpolation

**Description**: It isn't necessary to use interpolation to reference a parameter or variable.

The following example fails this test.

```bicep
param AutomationAccountName string

resource AutomationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
  name: '${AutomationAccountName}'
  ...
}
```

The following example passes this test.

```bicep
param AutomationAccountName string

resource AutomationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
  name: AutomationAccountName
  ...
}
```
