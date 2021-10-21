TODO asdff

# TTK: Location-Should-Not-Be-Hardcoded:

1. A location parameter, if it exists, must:
   - Be named exactly 'location' (case-insensitive)
   - **and** be of type string
1. A location parameter, if it exists, must have one of the following default values:
   - None
   - **or** the expression `resourceGroup().location`
   - **or** the expression `deployment().location`  TODO: asdff this doesn't compile
   **or** c) the string `'global'`
1. In a nested or linked template, the `location` parameter, if it exists, must have no default value
**SUGGESTED CHANGE:**
1. When instantiating a module, if it has a `location` parameter a value must be provided (instead of being left to a default)

## ISSUES: allow different names for "location"?

# TTK: Resources should have location:

1. Each resource's location property, if specified, must either:
  - be any expression (usually this should be the parameter `location`, but that is not required)
  - be the string `'global'`
  



# Location should not be hardcoded

**Code**: location-should-not-be-hardcoded

**Description**: asdff

To set a resource's location, your templates should have a parameter named location with the type set to string. TODO: asdff In the main template, main.bicep, this parameter can default to the resource group location. In modules, the location parameter shouldn't have a default location.

Template users may have limited access to regions where they can create resources. A hard-coded resource location might block users from creating a resource. The expression `resourceGroup().location` could block users if the resource group was created in a region the user can't access. Users who are blocked are unable to use the template.

By providing a location parameter that defaults to the resource group location, users can use the default value when convenient but also specify a different location when needed.

The following example **fails** because the resource's location is set to `resourceGroup().location`.
```bicep
resource storageaccount1 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: 'storageaccount1'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Premium_LRS'
  }
}
```

The next example uses a location parameter but **fails** because the parameter defaults to a hard-coded location.
```bicep
param location string = 'westus'

resource storageaccount1 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: 'storageaccount1'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Premium_LRS'
  }
}
```

The following example passes when the template is used as the main template TODO asdff. Create a parameter that defaults to the resource group location but allows users to provide a different value.
```bicep
@description('Location for the resources.')
param location string = resourceGroup().location

resource storageaccount1 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: 'storageaccount1'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Premium_LRS'
  }
}
```

## Note TODO asdf

If the preceding example is used as a linked template, the test fails. When used as a linked template, remove the default value.