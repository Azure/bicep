using 'main.bicep'

param config = {
  global: {
    serviceTreeId: '67fa35d2-495a-4f32-aee6-9dac46f7ecce'
    environment: 'test'
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
