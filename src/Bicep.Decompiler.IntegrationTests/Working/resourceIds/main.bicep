@description('Location for all resources.')
param location string = resourceGroup().location

var fooName = 'Foo!'

resource fooName_bar 'Foo.Rp/bar@2019-06-01' = {
//@[21:44) [BCP081 (Warning)] Resource type "Foo.Rp/bar@2019-06-01" does not have types available. (CodeDescription: none) |'Foo.Rp/bar@2019-06-01'|
  name: '${fooName}bar'
  location: location
  properties: {
    foo: 'bar'
  }
}

resource fooName_baz 'Foo.Rp/bar@2019-06-01' = {
//@[21:44) [BCP081 (Warning)] Resource type "Foo.Rp/bar@2019-06-01" does not have types available. (CodeDescription: none) |'Foo.Rp/bar@2019-06-01'|
  name: '${fooName}baz'
  location: location
  dependsOn: [
    fooName_bar
  ]
}

resource fooName_blah 'Foo.Rp/bar@2019-06-01' = {
//@[22:45) [BCP081 (Warning)] Resource type "Foo.Rp/bar@2019-06-01" does not have types available. (CodeDescription: none) |'Foo.Rp/bar@2019-06-01'|
  name: '${fooName}blah'
  location: location
  dependsOn: [
    fooName_bar
  ]
}

resource fooName_blah2 'Foo.Rp/bar@2019-06-01' = {
//@[23:46) [BCP081 (Warning)] Resource type "Foo.Rp/bar@2019-06-01" does not have types available. (CodeDescription: none) |'Foo.Rp/bar@2019-06-01'|
  name: '${fooName}blah2'
  location: location
  properties: {
    foobar: fooName_bar.properties.foo
    foobarFull: reference('${fooName}bar', '2019-06-01', 'Full').properties.foo
    foobarLocation: reference('${fooName}bar', '2019-06-01', 'Full').location
    foobarResId: fooName_bar.properties.foo
    foobarResIdFull: reference(fooName_bar.id, '2019-06-01', 'Full').properties.foo
    foobarResIdLocation: reference(fooName_bar.id, '2019-06-01', 'Full').location
  }
  dependsOn: [
    'Foo.Rp/bar${fooName}bar'
//@[4:29) [BCP034 (Error)] The enclosing array expected an item of type "module[] | (resource | module) | resource[]", but the provided item was of type "string". (CodeDescription: none) |'Foo.Rp/bar${fooName}bar'|
  ]
}
