# AdminUserName should not be a literal

**Code**: adminUsername-should-not-be-literal

**Description**: When setting an adminUserName property, don't use a literal value or an expression which evaluates to a literal value.
Create a parameter for the username and use an expression to reference the parameter's value.

The following examples fail this test.

```bicep
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name'
  location: location
  properties: {
    osProfile: {
      adminUsername: 'adminUsername'
    }
  }
}
```

```bicep
var defaultAdmin = 'administrator'

resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name'
  location: location
  properties: {
    osProfile: {
      adminUsername: defaultAdmin
    }

```

The following example passes this test.

```bicep
@secure()
param adminUsername string
param location string

resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name'
  location: location
  properties: {
    osProfile: {
      adminUsername: adminUsername
    }
  }
}
```
