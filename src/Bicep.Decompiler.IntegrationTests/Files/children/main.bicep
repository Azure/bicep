param location string {
  metadata: {
    description: 'Location for all resources.'
  }
  default: resourceGroup().location
}
param childPrefix string = newGuid()

var fooName = 'Foo!'

resource fooName_bar 'Foo.Rp/bar@2019-06-01' = {
//@[21:44) [BCP081 (Warning)] Resource type "Foo.Rp/bar@2019-06-01" does not have types available. |'Foo.Rp/bar@2019-06-01'|
  name: '${fooName}bar'
  location: location
  properties: {
    foo: 'bar'
  }
}

resource fooName_bar_steve 'Foo.Rp/bar/child1@2019-06-01' = {
//@[27:57) [BCP081 (Warning)] Resource type "Foo.Rp/bar/child1@2019-06-01" does not have types available. |'Foo.Rp/bar/child1@2019-06-01'|
  name: '${fooName}bar${'/'}steve'
  properties: {
    foo: 'bar'
  }
  dependsOn: [
    fooName_bar
  ]
}

resource fooName_bar_steve_louise 'Foo.Rp/bar/child1/child2@2019-06-01' = {
//@[34:71) [BCP081 (Warning)] Resource type "Foo.Rp/bar/child1/child2@2019-06-01" does not have types available. |'Foo.Rp/bar/child1/child2@2019-06-01'|
  name: '${'${fooName}bar'}/${'steve'}/${'louise'}'
  properties: {
    foo: 'bar'
  }
  dependsOn: [
    resourceId('Foo.Rp/bar/child1', '${fooName}bar', 'steve')
//@[4:61) [BCP034 (Error)] The enclosing array expected an item of type "resource | module", but the provided item was of type "string". |resourceId('Foo.Rp/bar/child1', '${fooName}bar', 'steve')|
  ]
}

resource fooName_bar_childPrefix 'Foo.Rp/bar/child1@2019-06-01' = {
//@[33:63) [BCP081 (Warning)] Resource type "Foo.Rp/bar/child1@2019-06-01" does not have types available. |'Foo.Rp/bar/child1@2019-06-01'|
  name: '${fooName}bar${'/'}${childPrefix}'
  location: location
  properties: {
    foo: 'bar'
  }
  dependsOn: [
    fooName_bar
  ]
}
