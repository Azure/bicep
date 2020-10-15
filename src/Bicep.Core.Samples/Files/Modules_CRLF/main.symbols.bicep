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

module modB './child/moduleb.bicep' = {
//@[7:11) Module modB. Type: modB. Declaration start char: 0, length: 65
  location: 'West US'
}
