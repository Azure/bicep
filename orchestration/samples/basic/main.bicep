targetScope = 'orchestrator'

import { GlobalConfigType } from './types.bicep'

// TODO implement this properly
func lookupSubscription(region string) string => 'a1bfa635-f2bf-42f1-86b5-848c674fc321'

param config GlobalConfigType
param mode 'hotfix' | 'standard'

var prefix = '${config.global.infraPrefix}-${config.global.environment}'

stack cluster './cluster/main.bicepparam' = [for (region, i) in config.regions: {
  region: region
  deploy: 'onChange'
  inputs: {
    subscriptionId: lookupSubscription(region)
    resourceGroup: '${prefix}-cluster-${region}'
    name: '${prefix}-cluster-${region}'
    config: {
      managedEnvironmentName: '${prefix}-managed-env-${region}'
    }
  }
}]

stack clusterApp './clusterApp/main.bicepparam' = [for (region, i) in config.regions: {
  region: region
  deploy: 'always'
  inputs: {
    subscriptionId: lookupSubscription(region)
    resourceGroup: '${prefix}-cluster-${region}'
    name: '${prefix}-cluster-app-${region}'
    config: {
      managedEnvironmentName: '${prefix}-managed-env-${region}'
    }
  }
  requires: [
    cluster[i]
  ]
}]

stack global './global/main.bicepparam' = {
  region: config.global.infraRegion
  deploy: 'onChange'
  inputs: {
    subscriptionId: lookupSubscription('global')
    resourceGroup: '${prefix}-global'
    name: '${prefix}-global'
    config: {
      afdName: 'hello-world-afd'
      apps: [for (region, i) in config.regions: {
        subscriptionId: lookupSubscription(region)
        resourceGroup: '${prefix}-cluster-${region}'
        name: 'hello-world'
      }]
    }
  }
  requires: [
    // TODO support a collection here
    clusterApp[0]
    clusterApp[1]
  ]
}

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
