@description('Location for all resources.')
param location string = resourceGroup().location

var fooName = 'Foo!'

resource fooName_bar 'Foo.Rp/bar@2019-06-01' = {
//@[21:44) [BCP081 (Warning)] Resource type "Foo.Rp/bar@2019-06-01" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed. (CodeDescription: none) |'Foo.Rp/bar@2019-06-01'|
  name: '${fooName}bar'
  location: location
  properties: {
    foo: 'bar'
  }
}

resource fooName_baz 'Foo.Rp/bar@2019-06-01' = {
//@[21:44) [BCP081 (Warning)] Resource type "Foo.Rp/bar@2019-06-01" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed. (CodeDescription: none) |'Foo.Rp/bar@2019-06-01'|
  name: '${fooName}baz'
  location: location
  dependsOn: [
    fooName_bar
  ]
}

resource fooName_blah 'Foo.Rp/bar@2019-06-01' = {
//@[22:45) [BCP081 (Warning)] Resource type "Foo.Rp/bar@2019-06-01" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed. (CodeDescription: none) |'Foo.Rp/bar@2019-06-01'|
  name: '${fooName}blah'
  location: location
  dependsOn: [
    fooName_bar
  ]
}

resource fooName_blah2 'Foo.Rp/bar@2019-06-01' = {
//@[23:46) [BCP081 (Warning)] Resource type "Foo.Rp/bar@2019-06-01" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed. (CodeDescription: none) |'Foo.Rp/bar@2019-06-01'|
  name: '${fooName}blah2'
  location: location
  properties: {
    foobar: fooName_bar.properties.foo
    foobarFull: reference('${fooName}bar', '2019-06-01', 'Full').properties.foo
    foobarLocation: reference('${fooName}bar', '2019-06-01', 'Full').location
    foobarResId: fooName_bar.properties.foo
    foobarResIdFull: reference(fooName_bar.id, '2019-06-01', 'Full').properties.foo
//@[21:68) [use-resource-symbol-reference (Warning)] Use a resource reference instead of invoking function "reference". This simplifies the syntax and allows Bicep to better understand your deployment dependency graph. (CodeDescription: bicep core(https://aka.ms/bicep/linter/use-resource-symbol-reference)) |reference(fooName_bar.id, '2019-06-01', 'Full')|
    foobarResIdLocation: reference(fooName_bar.id, '2019-06-01', 'Full').location
//@[25:72) [use-resource-symbol-reference (Warning)] Use a resource reference instead of invoking function "reference". This simplifies the syntax and allows Bicep to better understand your deployment dependency graph. (CodeDescription: bicep core(https://aka.ms/bicep/linter/use-resource-symbol-reference)) |reference(fooName_bar.id, '2019-06-01', 'Full')|
  }
  dependsOn: [
    'Foo.Rp/bar${fooName}bar'
//@[04:29) [BCP034 (Error)] The enclosing array expected an item of type "module[] | (resource | module) | resource[]", but the provided item was of type "'Foo.Rp/barFoo!bar'". (CodeDescription: none) |'Foo.Rp/bar${fooName}bar'|
  ]
}

