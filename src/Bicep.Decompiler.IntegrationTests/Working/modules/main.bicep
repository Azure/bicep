param location string = resourceGroup().location
param baseUrl string {
  metadata: {
    description: 'Base URL for the reference templates and scripts'
  }
  default: 'https://my.base/url'
}

var armBaseUrl = baseUrl
var module1Url = '${armBaseUrl}/nested/module1.json'
var module2Url = '${armBaseUrl}/nested/module2.json'
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
    location: location
    objectParam: objectVar
    arrayParam: arrayVar
  }
}

module module2Deploy 'nested/module2.bicep' = {
  name: 'module2Deploy'
  params: {
    location: location
    objectParam: objectVar
    arrayParam: arrayVar
  }
}

module moduleWithDodgyUri '<failed to parse [concat(parameters(\'location\'), \'abc\', variables(\'module2Url\'))]>' = {
//@[26:116) [BCP085 (Error)] The specified module path contains one ore more invalid path characters. The following are not permitted: """, "*", ":", "<", ">", "?", "\", "|". |'<failed to parse [concat(parameters(\'location\'), \'abc\', variables(\'module2Url\'))]>'|
  name: 'moduleWithDodgyUri'
  params: {
    location: location
    objectParam: objectVar
    arrayParam: arrayVar
  }
}
