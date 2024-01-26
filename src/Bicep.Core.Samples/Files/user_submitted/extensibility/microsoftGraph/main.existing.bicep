provider 'microsoftGraph@1.0.0'

resource resourceApp 'Microsoft.Graph/applications@beta' existing = {
  uniqueName: 'resourceApp'
}

resource group 'Microsoft.Graph/applications@beta' existing = {
  uniqueName: 'myGroup'
}
