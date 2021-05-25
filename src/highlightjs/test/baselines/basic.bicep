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
  'name': 'test/child'
  properties:{
  }
}]

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2020-08-01' existing = {
  name: 'myVnet'
}

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

var emojis = '💪😊😈🍕☕'
var ninjaCat = '🐱‍👤'

/* block */

/*
朝辞白帝彩云间
千里江陵一日还
两岸猿声啼不住
轻舟已过万重山
*/

// greek letters in comment: Π π Φ φ plus emoji 😎
var variousAlphabets = {
  'α': 'α'
  'Ωω': [
    'Θμ'
  ]
  'ążźćłóę': 'Cześć!'
  'áéóúñü': '¡Hola!'

  '二头肌': '二头肌'
}

output concatUnicodeStrings string = concat('Θμ', '二头肌', 'α')
output interpolateUnicodeStrings string = 'Θμ二${emojis}头肌${ninjaCat}α'

// all of these should produce the same string
var surrogate_char      = '𐐷'
var surrogate_codepoint = '\u{10437}'
var surrogate_pairs     = '\u{D801}\u{DC37}'

// ascii escapes
var hello = '❆ Hello\u{20}World\u{21} ❁'
