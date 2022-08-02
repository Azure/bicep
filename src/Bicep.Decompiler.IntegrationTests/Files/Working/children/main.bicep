@description('Location for all resources.')
param location string = resourceGroup().location
param childPrefix string = newGuid()

var fooName = 'Foo!'

resource fooName_bar 'Foo.Rp/bar@2019-06-01' = {
//@[21:44) [BCP081 (Warning)] Resource type "Foo.Rp/bar@2019-06-01" does not have types available. (CodeDescription: none) |'Foo.Rp/bar@2019-06-01'|
  name: '${fooName}bar'
  location: location
  properties: {
    foo: 'bar'
  }
}

resource fooName_bar_steve 'Foo.Rp/bar/child1@2019-06-01' = {
//@[27:57) [BCP081 (Warning)] Resource type "Foo.Rp/bar/child1@2019-06-01" does not have types available. (CodeDescription: none) |'Foo.Rp/bar/child1@2019-06-01'|
  parent: fooName_bar
  name: 'steve'
  properties: {
    foo: 'bar'
  }
}

resource fooName_bar_steve_louise 'Foo.Rp/bar/child1/child2@2019-06-01' = {
//@[34:71) [BCP081 (Warning)] Resource type "Foo.Rp/bar/child1/child2@2019-06-01" does not have types available. (CodeDescription: none) |'Foo.Rp/bar/child1/child2@2019-06-01'|
  parent: fooName_bar_steve
  name: 'louise'
  properties: {
    foo: 'bar'
  }
}

resource fooName_bar_childPrefix 'Foo.Rp/bar/child1@2019-06-01' = {
//@[33:63) [BCP081 (Warning)] Resource type "Foo.Rp/bar/child1@2019-06-01" does not have types available. (CodeDescription: none) |'Foo.Rp/bar/child1@2019-06-01'|
  parent: fooName_bar
  name: childPrefix
//@[08:19) [use-stable-resource-identifiers (Warning)] Resource identifiers should be reproducible outside of their initial deployment context. Resource fooName_bar_childPrefix's 'name' identifier is potentially nondeterministic due to its use of the 'newGuid' function (fooName_bar_childPrefix.name -> childPrefix (default value) -> newGuid()). (CodeDescription: bicep core(https://aka.ms/bicep/linter/use-stable-resource-identifiers)) |childPrefix|
  location: location
  properties: {
    foo: 'bar'
  }
}
