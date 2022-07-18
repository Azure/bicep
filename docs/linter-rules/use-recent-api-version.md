# Use recent api version asdfg

**Code**: use-recent-api-version

**Description**: The API version for each resource should use a recent version. A non-preview(GA) version is considered valid as long as it's less than 2 years old or latest. A preview(alpha, beta, preview, privatepreview and rc) version is considered valid only if it's latest and there is no later non-preview version.

The following example fails this test.

```bicep
resource dnsZone 'Microsoft.Network/dnsZones@2017-10-01' = {
  name: 'name'
  location: 'global'
}
```

The following example passes this test.

```bicep
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'name'
  location: 'global'
}
```
