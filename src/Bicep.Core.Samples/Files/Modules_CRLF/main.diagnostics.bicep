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
