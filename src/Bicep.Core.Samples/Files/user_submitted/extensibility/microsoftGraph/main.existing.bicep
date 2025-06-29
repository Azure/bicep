extension microsoftGraphBeta

resource resourceApp 'Microsoft.Graph/applications@beta' existing = {
  uniqueName: 'resourceApp'
}

resource group 'Microsoft.Graph/applications@beta' existing = {
  uniqueName: 'myGroup'
}
