targetScope = 'subscription'

param location string = 'eastus'

// REP 0015: with the 'formalizedScope' experimental feature enabled, this module's cross-scope
// targeting is emitted as a single duck-typed "@scope" object instead of the legacy
// "subscriptionId" / "resourceGroup" properties.
module storageMod 'modules/mod.bicep' = {
  name: 'storageMod'
  scope: resourceGroup('my-rg')
  params: {
    location: location
  }
}

module storageMod2 'modules/mod.bicep' = {
  name: 'storageMod2'
  scope: location != 'eastus' ? resourceGroup() : resourceGroup('my-rg')
//@[45:47) [BCP071 (Error)] Expected 1 to 2 arguments, but got 0. (bicep https://aka.ms/bicep/core-diagnostics#BCP071) |()|
  params: {
    location: location
  }
}

output loc string = storageMod.outputs.loc

