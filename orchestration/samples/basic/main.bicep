targetScope = 'orchestrator'

import { GlobalConfigType } from './types.bicep'

param config GlobalConfigType
param mode 'hotfix' | 'standard'

stack global './global/main.bicepparam' = {
  region: 'global'
  deploy: 'always' // always | onChange | onUnhealthy
}

stack cluster './cluster/main.bicepparam' = [for (region, i) in config.regions: {
  region: region
  requires: [
    global
  ]
}]

stack clusterApp './clusterApp/main.bicepparam' = [for (region, i) in config.regions: {
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
  minimumWaitTimeBetweenBatches: mode == 'hotfix' ? 'PT1H' : 'PT24H'
}

rule approvals 'Approval' = {
  // how does this work?
}
