param location string

resource myResource 'Mock.Rp/mockResource@2020-01-01' = {
  name: 'mockResource'
  location: location
}

output myResourceId string = myResource.id