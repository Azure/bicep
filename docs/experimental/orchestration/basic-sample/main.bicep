targetScope = 'orchestrator'

import { GlobalConfigType } from './types.bicep'

param config GlobalConfigType

component global './global.bicepparam' = {

}

component regional './regional.bicepparam' = [for region in config.regions: {
  
}]
