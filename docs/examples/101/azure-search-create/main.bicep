@minLength(2)
@maxLength(60)
param name string

@allowed([
  'free'
  'basic'
  'standard'
  'standard2'
  'standard3'
  'storage_optimized_l1'
  'storage_optimized_l2'
])
param sku string = 'standard'

@minValue(1)
@maxValue(12)
param replicaCount int = 1

@allowed([
  1
  2
  3
  4
  6
  12
])
param partitionCount int = 1

@allowed([
  'default'
  'highDensity'
])
param hostingMode string = 'default'

param location string = resourceGroup().location

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
