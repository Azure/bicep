targetScope = 'orchestrator'

import { GlobalConfigType } from './types.bicep'

param config GlobalConfigType

stack global './global.bicepparam' = {
  region: 'global'
}

stack cluster './cluster.bicepparam' = [for (region, i) in config.regions: {
  region: region
  requires: [
    global
  ]
}]

stack drpApp './drpApp.bicepparam' = [for (region, i) in config.regions: {
  region: region
  requires: [
    cluster[i]
  ]
}]

rule stages 'Batching' = {
  groups: [for (mapping, i) in config.stageMappings: {
    order: i
    name: mapping.name
    selector: (stack) => contains(mapping.regions, stack.region)
  }]
  minimumWaitTimeBetweenBatches: 'PT24H'
}

rule approvals 'Approval' = {
  // how does this work?
}
