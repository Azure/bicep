using 'main.bicep'

param config = {
  global: {
    serviceTreeId: '...'
  }
  regional: {
    eastus: {
      shortName: 'eus'
    }
    westus: {
      shortName: 'wus'
    }
  }
  regions: ['eastus', 'westus']
  stageMappings: [
    { name: 'stage0', regions: ['eastus'] }
    { name: 'stage1', regions: ['westus'] }
  ]
}

param mode = 'hotfix'
