metadata description = 'Exercises restricted network access.'

module test '../../../main.bicep' = {
  name: 'test'
  params: {
    resourceGroupName: 'restricted-rg'
    secret: 'example'
    networkAccess: {
      kind: 'restricted'
      allowedCidrs: [
        '10.0.0.0/24'
      ]
    }
  }
}
