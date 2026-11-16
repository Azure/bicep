param allowPublicAccess bool

module storage 'storage.bicep' = {
  name: 'storage'
  params: {
    allowPublicAccess: allowPublicAccess
  }
}

output storageId string = storage.outputs.storageId
output tenantId string = tenant().tenantId
output deploymentName string = deployment().name
