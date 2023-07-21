param app1Name string
param app2Name string
param app3Name string

resource app1 'Microsoft.AppPlatform/Spring/apps@2020-07-01' = {
  name: app1Name
  properties: {
    public: true
    activeDeploymentName: 'default'
  }
}

resource app2 'Microsoft.AppPlatform/Spring/apps@2020-07-01' = {
  name: app2Name
  properties: {
    public: false
    activeDeploymentName: 'default'
  }
}

resource app3 'Microsoft.AppPlatform/Spring/apps@2020-07-01' = {
  name: app3Name
  properties: {
    public: false
    activeDeploymentName: 'default'
  }
}
