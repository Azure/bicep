# Secure parameter default

**Code**: secure-parameter-default

**Description**: Don't provide a hard-coded default value for a secure parameter in your template, unless it is empty or an expression containing a call to newGuid().

You use the [@secure() decorator](../spec/parameters.md) on parameters that contain sensitive values, like passwords. When a parameter uses a secure decorator, the value of the parameter isn't logged or stored in the deployment history. This action prevents a malicious user from discovering the sensitive value.

However, when you provide a default value, that value is discoverable by anyone who can access the template or the deployment history.

The following example fails this test:

```bicep
@secure()
param adminPassword string = 'HardcodedPassword'
```

The following examples pass this test:

```bicep
@secure()
param adminPassword string
```

```bicep
@secure()
param adminPassword string = ''
```

```bicep
@secure()
param adminPassword string = newGuid()
```
