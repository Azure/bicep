# Use recent API version

**Code**: use-recent-api-version

**Description**: The API version for each resource should use a recent version. A non-preview (GA) version is considered valid as long as it's less than 2 years old or is the latest available. A preview version (alpha, beta, preview, privatepreview and rc) is considered valid only if it's the latest available and there is no later non-preview version.

The following example fails this test.

```bicep
resource dnsZone 'Microsoft.Network/dnsZones@2017-10-01' = {
  name: 'name'
  location: 'global'
}
```