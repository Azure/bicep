# Convert any* ARM Template into a Bicep file

**assumes that the ARM template only uses syntax that is supported in Bicep*

Since Bicep is a transparent abstraction of ARM templates, any resource that can be deployed via an ARM template can be authored in Bicep. However, not all ARM template capabilities are supported in Bicep in the 0.1 release. The following statements must be true:

* Template does *not* use the `copy` function for creating multiple resources, multiple variables, or multiple outputs
* Template does *not* conditionally deploy resources with the `condition` property
* Template does not deploy across scopes (though this can be hacked together by using the `Microsoft.Resources/deployments` resource and using the `templateLink` or `template` property to inser the full ARM template)


## Convert parameters, variables, and outputs

Parameter, variable and output declarations are relatively simple to convert. Let's look at three ARM Template code samples, converted into Bicep syntax:

### Convert a parameter

To convert a simple parameter (usually with only a default value):

ARM Template:
```json
...
    "parameters": {
        "name": {
            "type": "string",
            "defaultValue": "myName"
        }
    },
...
```

Bicep:
```
parameter name string = 'myName'
```

For a more complex parameter with modifiers such as `allowedValues`:

ARM Template:
```json
...
    "parameters": {
        "name": {
            "type": "string",
            "defaultValue": "myName",
            "allowedValues": [
                "myName",
                "myOtherName"
            ],
            "minLength": 3,
            "maxLength": 24
        }
    },
...
```

Bicep:

```
parameter name string {
  default: 'myName'
  allowedValues: [
    'myName'
    'myOtherName'
  ]
  minLength: 3
  maxLength: 24
}
```

### Convert a variable

Translation of a simple string variable:

ARM Template:

```
"variables": {
    "location": "eastus"
}
```

Bicep:

```
variable location = 'eastus'
```

### Convert an output

Translation of a simple string output

ARM Template:

```
"outputs": {
    "myOutput": {
        "type": "string",
        "value": "my output value"
    }
}
```

Bicep:

```
output myOutput string = 'my output value'
```

## Convert resources

Let's take a basic resource declared in ARM, and convert it to Bicep. Notice that all properties required in the ARM template are still required in bicep:

ARM Template:

```
// todo
```

Bicep: 

```
// todo
```