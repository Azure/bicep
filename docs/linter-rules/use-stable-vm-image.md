# Use stable vm image

**Code**: use-stable-vm-image

**Description**: Virtual machines shouldn't use preview images. This rule checks the following properties under "imageReference" and fails if any of them contain the string "preview": 
* offer
* sku
* version

The following example fails this test.

```bicep
resource vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: 'virtualMachineName'
  location: resourceGroup().location
  properties: {
    storageProfile: {
      imageReference: {
        offer: 'WindowsServer-preview'
        sku: '2019-Datacenter-preview'
        version: 'preview'
      }
    }
  }
}
```

The following example passes this test.

```bicep
resource vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: 'virtualMachineName'
  location: resourceGroup().location
  properties: {
    storageProfile: {
      imageReference: {
        offer: 'WindowsServer'
        sku: '2019-Datacenter'
        version: 'latest'
      }
    }
  }
}
```