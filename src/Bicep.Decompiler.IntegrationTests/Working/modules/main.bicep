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

module moduleWithDodgyUri '?' /*TODO: replace with correct path to [concat(parameters('location'), 'abc', variables('module2Url'))]*/ = {
//@[26:29) [BCP085 (Error)] The specified module path contains one ore more invalid path characters. The following are not permitted: """, "*", ":", "<", ">", "?", "\", "|". |'?'|
  name: 'moduleWithDodgyUri'
  params: {
    location: location
    objectParam: objectVar
    arrayParam: arrayVar
  }
}

module moduleWithRg 'nested/module1.bicep' = {
  name: 'moduleWithRg'
  scope: resourceGroup('test${module1Url}')
  params: {}
//@[2:8) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam", "location", "objectParam". |params|
}

module moduleWithRgAndSub 'nested/module1.bicep' = {
  name: 'moduleWithRgAndSub'
  scope: resourceGroup('${module1Url}test', 'test${module1Url}')
  params: {}
//@[2:8) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam", "location", "objectParam". |params|
}

module moduleWithSub 'nested/module1.bicep' = {
  name: 'moduleWithSub'
  scope: subscription('${module1Url}test')
//@[9:42) [BCP036 (Error)] The property "scope" expected a value of type "resourceGroup" but the provided value is of type "subscription". |subscription('${module1Url}test')|
  params: {}
//@[2:8) [BCP035 (Error)] The specified "object" declaration is missing the following required properties: "arrayParam", "location", "objectParam". |params|
}
