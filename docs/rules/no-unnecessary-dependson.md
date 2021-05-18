# Remove unnecessary dependsOn

**Code**: no-unnecessary-dependson

**Description**: An implicit dependency will be created when one resource declaration references the identifier of another resource declaration in an expression. It is not necessary to explicitly add dependsOn property.

The following example fails this test because the dependsOn property is not necessary.

```bicep
resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
  name: 'myZone'
  location: 'global'
}

resource otherResource 'Microsoft.Example/examples@2020-06-01' = {
  name: 'exampleResource'
  dependsOn: [
    dnsZone
  ]
  properties: {
    // get read-only DNS zone property
    nameServers: dnsZone.properties.nameServers
  }
}
```

The following example passes this test.

```bicep
resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
  name: 'myZone'
  location: 'global'
}

resource otherResource 'Microsoft.Example/examples@2020-06-01' = {
  name: 'exampleResource'
  properties: {
    // get read-only DNS zone property
    nameServers: dnsZone.properties.nameServers
  }
}
```

A nested resource also has an implicit dependency on its containing resource.

```bicep
resource myParent 'My.Rp/parentType@2020-01-01' = {
  name: 'myParent'
  location: 'West US'

  // depends on 'myParent' implicitly
  resource myChild 'childType' = {
    name: 'myChild'
  }
}
```
