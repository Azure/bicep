# Interpolation preferred

Interpolation should be used instead of concat function.

The following example fails this test because the concat function is used.

```bicep
param suffix string = '001'
var vnetName = concat('vnet-', parameters('suffix'))

resource vnet 'Microsoft.Network/virtualNetworks@2018-10-01' = {
  name: vnetName
  ...
}
```

The following example passes this test.

```bicep
param suffix string = '001'
var vnetName = 'vnet-${suffix}'

resource vnet 'Microsoft.Network/virtualNetworks@2018-10-01' = {
  name: vnetName
  ...
}
```
