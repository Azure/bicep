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
}
