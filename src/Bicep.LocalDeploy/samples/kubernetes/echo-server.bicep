@secure()
param kubeConfig string

param servicePort int = 8080

provider 'kubernetes@1.0.0' with {
  kubeConfig: kubeConfig
  namespace: 'default'
}

var build = {
  name: 'echo-server'
  version: '0.8.12'
  image: 'ealen/echo-server:0.8.12'
  containerPort: 80
}

@description('Configure the Echo Server deployment')
resource buildDeploy 'apps/Deployment@v1' = {
  metadata: {
    name: build.name
  }
  spec: {
    selector: {
      matchLabels: {
        app: build.name
        version: build.version
      }
    }
    replicas: 1
    template: {
      metadata: {
        labels: {
          app: build.name
          version: build.version
        }
      }
      spec: {
        containers: [
          {
            name: build.name
            image: build.image
            ports: [
              {
                containerPort: build.containerPort
              }
            ]
          }
        ]
      }
    }
  }
}

@description('Configure the Echo Server service')
resource buildService 'core/Service@v1' = {
  metadata: {
    name: build.name
  }
  spec: {
    type: 'LoadBalancer'
    ports: [
      {
        port: servicePort
#disable-next-line BCP036
        targetPort: build.containerPort
      }
    ]
    selector: {
      app: build.name
    }
  }
}

output dnsLabel string = build.name
