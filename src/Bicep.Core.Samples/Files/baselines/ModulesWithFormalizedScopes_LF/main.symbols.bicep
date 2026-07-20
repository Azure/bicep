targetScope = 'subscription'

param location string = 'eastus'
//@[6:14) Parameter location. Type: string. Declaration start char: 0, length: 32

// REP 0015: with the 'formalizedScope' experimental feature enabled, this module's cross-scope
// targeting is emitted as a single duck-typed "@scope" object instead of the legacy
// "subscriptionId" / "resourceGroup" properties.
module storageMod 'modules/mod.bicep' = {
//@[7:17) Module storageMod. Type: module. Declaration start char: 0, length: 135
  name: 'storageMod'
  scope: resourceGroup('my-rg')
  params: {
    location: location
  }
}

module storageMod2 'modules/mod.bicep' = {
//@[7:18) Module storageMod2. Type: module. Declaration start char: 0, length: 178
  name: 'storageMod2'
  scope: location != 'eastus' ? resourceGroup() : resourceGroup('my-rg')
  params: {
    location: location
  }
}

output loc string = storageMod.outputs.loc
//@[7:10) Output loc. Type: string. Declaration start char: 0, length: 42

