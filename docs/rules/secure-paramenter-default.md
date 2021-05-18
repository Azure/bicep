# Secure paramenter default

**Code**: secure-paramenter-default

**Description**: Don't provide a hard-coded default value for a secure parameter in your template. An empty string is fine for the default value.

You use the types SecureString or SecureObject on parameters that contain sensitive values, like passwords. When a parameter uses a secure type, the value of the parameter isn't logged or stored in the deployment history. This action prevents a malicious user from discovering the sensitive value.

However, when you provide a default value, that value is discoverable by anyone who can access the template or the deployment history.

The following example fails this test:

```bicep
@secure()
param adminPassword string = 'HardcodedPassword'
```

The following example passes this test.

```bicep
@secure()
param adminPassword string
```
