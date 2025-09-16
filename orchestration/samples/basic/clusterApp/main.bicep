@description('The name of the managed environment')
param managedEnvironmentName string

resource appEnv 'Microsoft.App/managedEnvironments@2025-02-02-preview' existing = {
  name: managedEnvironmentName
}

module containerApp 'br/public:avm/res/app/container-app:0.18.1' = {
  params: {
    name: 'hello-world'
    containers: [
      {
        image: 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'
        name: 'simple-hello-world-container'
        resources: {
          cpu: json('0.25')
          memory: '0.5Gi'
        }
      }
    ]
    environmentResourceId: appEnv.id
  }
}
