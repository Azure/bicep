module modATest './modulea.bicep' = {
//@[7:15) Module modATest. Type: modATest. Declaration start char: 0, length: 159
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
