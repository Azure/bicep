targetScope = 'resourceGroup'

resource resourceDup1 'Mock.Rp/mockResource@2020-01-01' = {
  name: 'resourceDup'
}
resource resourceDup2 'Mock.Rp/mockResource@2020-01-01' = {
  name: 'resourceDup'
}
resource resourceDup3 'Mock.Rp/mockResource@2019-01-01' = {
  name: 'resourceDup'
}

resource resourceValid 'Mock.Rp/mockResource@2020-01-01' = {
  name: 'resourceValid'
}

resource extResourceDup1 'Mock.Rp/mockExtResource@2020-01-01' = {
  name: 'extResourceDup'
  scope: resourceValid
}

resource extResourceDup2 'Mock.Rp/mockExtResource@2019-01-01' = {
  name: 'extResourceDup'
  scope: resourceValid
}
