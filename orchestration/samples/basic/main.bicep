targetScope = 'orchestrator'

import { GlobalConfigType } from './types.bicep'

// TODO implement this properly
func lookupSubscription(region string) string => 'a1bfa635-f2bf-42f1-86b5-848c674fc321'

param config GlobalConfigType
param mode 'hotfix' | 'standard'

stack global './global/main.bicepparam' = {
  region: 'global'
  deploy: 'onChange'
  inputs: {
    subscriptionId: lookupSubscription('global')
    resourceGroup: 'global-shared'
    name: 'global-shared'
  }
}

stack cluster './cluster/main.bicepparam' = [for (region, i) in config.regions: {
  region: region
  deploy: 'onChange'
  inputs: {
    subscriptionId: lookupSubscription(region)
    resourceGroup: 'cluster-${region}'
    name: 'cluster-${region}'
  }
  requires: [
    global
  ]
}]

stack clusterApp './clusterApp/main.bicepparam' = [for (region, i) in config.regions: {
  region: region
  deploy: 'always'
  inputs: {
    subscriptionId: lookupSubscription(region)
    resourceGroup: 'cluster-${region}'
    name: 'cluster-app-${region}'
  }
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
