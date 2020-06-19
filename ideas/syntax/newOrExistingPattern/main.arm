// ...

resource storage 'Microsoft.Storage/storageAccounts@2017-06-01' when (storageNewOrExisting == 'new') = {
    name: storageAccountName
    location: location
    kind: 'Storage'
    sku: {
        name: storageAccountType
    }
} 
else = reference(storageAccountName) // reference to preexisting "external" resource

resource vm 'microsoft.compute/virtualMachines@2017-03-30' {
    name: vmName
    location: location
    properties: {
        // ...
        diagnosticsProfile: {
            bootDiagnostics: {
                enabled: true
                storageUri: storage.primaryEndpoints.blob
            }
        }
    }
}