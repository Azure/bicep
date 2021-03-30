// test
/* test 2 */
targetScope = 'resourceGroup'

resource avcsdd 'Microsoft.Cache/redis@2020-06-01' = { // line comment
  name: 'def' /* block
  comment */
  location: 'somewhere'
  properties: {
    sku: {
      capacity: 123
      family: 'C'
      name: 'Basic'
    }
  }
}

var secretsObject = {
  secrets: [
    'abc'
    'def'
  ]
}

var parent = 'abc'

resource secrets0 'Microsoft.KeyVault/vaults/secrets@2018-02-14' = {
  name: '${parent}/child'
  properties: {
    attributes:  {
      enabled: true
    }
  }
}
resource secrets1 'Microsoft.KeyVault/vaults/secrets@2018-02-14' = if (secrets0.id == '') {
  name: '${parent}/child1'
  properties: {
    /* inline comment */
  }
}

resource secrets2 'Microsoft.KeyVault/vaults/secrets@2018-02-14' = [for secret in secretsObject.secrets: {
  name: 'asdfsd/forloop'
  properties: {}
}]

resource secrets3 'Microsoft.KeyVault/vaults/secrets@2018-02-14' = [for secret in secretsObject.secrets: {
  name: 'jk${true}asdf${23}.\${SDF${secretsObject['secrets'][1]}'
  properties: {
  }
}]

resource secrets4 'Microsoft.KeyVault/vaults/secrets@2018-02-14' = [for secret in secretsObject.secrets: if (true) {
  'name': 'test'
  properties:{
    
  }
}]

var multi = ''''''
var multi2 = '''
      hello!
'''

var func = resourceGroup().location
var func2 = reference('Microsoft.KeyVault/vaults/secrets', func)
var func3 = union({
  'abc': resourceGroup().id
}, {
  'def': 'test'
})

@allowed([
  'hello!'
  'hi!'
])
@secure()
param secureParam string = 'hello!'
