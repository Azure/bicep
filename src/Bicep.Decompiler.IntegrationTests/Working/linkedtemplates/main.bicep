param location string = resourceGroup().location

@description('Base URL for the reference templates and scripts')
param baseUrl string = 'https://my.base/url'

var armBaseUrl = baseUrl
var objectVar = {
  val1: 'a${location}b'
}
var arrayVar = [
  'abc'
  location
]

module module1Deploy 'nested/module1.bicep' = {
  name: 'module1Deploy'
  params: {
    stringParam: location
    objectParam: objectVar
    arrayParam: arrayVar
  }
}

module module2Deploy 'nested/module2.bicep' = {
  name: 'module2Deploy'
  params: {
    stringParam: location
    objectParam: objectVar
    arrayParam: arrayVar
  }
}
