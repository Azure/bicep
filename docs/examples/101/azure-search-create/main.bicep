param name string {
  minLength: 2
  maxLength: 60
}

param sku string {
  default: 'standard'
  allowed: [
    'free'
    'basic'
    'standard'
    'standard2'
    'standard3'
    'storage_optimized_l1'
    'storage_optimized_l2'
  ]
}

param replicaCount int {
  default: 1
  minValue: 1
  maxValue: 12
}

param partitionCount int {
  default: 1
  allowed: [
    1
    2
    3
    4
    6
    12
  ]
}

param hostingMode string {
  default: 'default'
  allowed: [
    'default'
    'highDensity'
  ]
}

param location string {
  default: resourceGroup().location
}

resource search 'Microsoft.Search/searchServices@2020-08-01' = {
  name: name
  location: location
  sku: {
    name: sku
  }
  properties: {
    replicaCount: replicaCount
    partitionCount: partitionCount
    hostingMode: hostingMode
  }
}
