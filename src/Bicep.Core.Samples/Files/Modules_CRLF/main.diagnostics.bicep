module modATest './modulea.bicep' = {
  stringParamB: 'hello!'
  objParam: {
    a: 'b'
  }
  arrayParam: [
    {
      a: 'b'
    }
    'abc'
  ]
}

module modB './child/moduleb.bicep' = {
  location: 'West US'
}
