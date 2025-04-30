import { standardSdp } from 'br:mcr.microsoft.com/bicep/sdp'

var regions = [
  'westus'
  'westeurope'
]

var config = {
  regions: regions
  globalDns: {
    name: 'globalDns'
  }
  regionalDns: toObject(regions, region => region, region => {
    name: '${region}-svc'
    permittedIps: externalInput('$config(ev2.ipAddresses)')
  })
  clusterResources: toObject(regions, region => region, region => {
    name: '${region}-clusterResources'
  })
  icmAutomation: {
    name: 'icmAutomation'
  }
  copilot: toObject(regions, region => region, region => {
    name: '${region}-copilot'
  })
}

var copilotRegions = [
  'westus'
  'westeurope'
]

stack globalDns 'globalDns.bicep' = {
  version: '0.1.0'
}

stack regionalDns 'regionalDns.bicep' = [for region in config.regions : {
  version: '0.2.0'
  config: config.regionalDns[region]
  requires: {
    globalDns: '>=0.1.0'
  }
}]

stack clusterResources 'clusterResources.bicep' = [for region in config.regions: {
  version: '0.2.0'
  config: config.clusterResources[region]
  requires: {
    regionalDns: '>=0.2.0'
  }
  rules: [
    standardSdp
  ]
}]

// standalone app
stack icmAutomation 'icmAutomation.bicep' = {
  config: config.icmAutomation
  version: '0.2.0'
}

stack copilotHandler 'copilot.bicep' = [for region in config.regions if (contains(copilotRegions, region)): {
  version: '0.2.0'
  config: config.copilot[region]
  requires: {
    icmAutomation: '>=0.2.0'
  }
  rules: [
    standardSdp
  ]
}]
