targetScope = 'ev2mockup'

// https://ev2docs.azure.net/features/service-artifacts-parameterization/sys-variables.html
var config = externalInput('ev2.variable', '$config')

metadata service ServiceMetadata = {
  serviceIdentifier: 'b0cea33a-3bb9-4349-9f8c-6bdb92ed6999'
  displayName: 'My Sample Service'
  // TODO what does this identifier represent?
  serviceGroup: 'Microsoft.MySampleService.MySampleServiceGroup'
  environment: 'Prod'
  tenantId: config.aad.tenants.Microsoft.id
}

metadata rollout RolloutMetadata = {
  // TODO why do we also need a name for the rollout?
  name: 'MyWebservice 1.0'
  buildSource: {
    parameters: {
      version: loadTextContent('./version.txt')
    }
  }
  configuration: {
    serviceScope: {
      // TODO: How should this interact with users who are generating config?
      // TODO: any possibilities to improve on how people are doing config?
      spec: loadJsonContent('./config/prod.json')
    }
  }
  notification: {
    email: {
      to: [
        'antmarti@microsoft.com'
      ]
    }
  }
  rolloutType: 'Major'
}

// TODO - what is "validation" in the context of Bicep
step preValidate 'validate/pre.bicepparam' = {
}

// TODO define how this'll work with rollout parameters + extensions
step deploy 'deploy/main.bicepparam' = {
  // TODO: How to represent that this is per-scope?
  // scope: subscription(config)
  dependsOn: [
    preValidate
  ]
}

// TODO - what is "validation" in the context of Bicep
step postValidate 'validate/post.bicepparam' = {
  dependsOn: [
    deploy
  ]
}

// TODO: How is stagemap/progression defined?
