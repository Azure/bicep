/* 
  This is a block comment.
*/

// parameters without default value
param myString string
//@[line005->line011]     "myString": {
//@[line005->line012]       "type": "string"
//@[line005->line013]     },
param myInt int
//@[line006->line014]     "myInt": {
//@[line006->line015]       "type": "int"
//@[line006->line016]     },
param myBool bool
//@[line007->line017]     "myBool": {
//@[line007->line018]       "type": "bool"
//@[line007->line019]     },

// parameters with default value
param myString2 string = 'string value'
//@[line010->line020]     "myString2": {
//@[line010->line021]       "type": "string",
//@[line010->line022]       "defaultValue": "string value"
//@[line010->line023]     },
param myInt2 int = 42
//@[line011->line024]     "myInt2": {
//@[line011->line025]       "type": "int",
//@[line011->line026]       "defaultValue": 42
//@[line011->line027]     },
param myTruth bool = true
//@[line012->line028]     "myTruth": {
//@[line012->line029]       "type": "bool",
//@[line012->line030]       "defaultValue": true
//@[line012->line031]     },
param myFalsehood bool = false
//@[line013->line032]     "myFalsehood": {
//@[line013->line033]       "type": "bool",
//@[line013->line034]       "defaultValue": false
//@[line013->line035]     },
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[line014->line036]     "myEscapedString": {
//@[line014->line037]       "type": "string",
//@[line014->line038]       "defaultValue": "First line\r\nSecond\ttabbed\tline"
//@[line014->line039]     },

// object default value
param foo object = {
//@[line017->line040]     "foo": {
//@[line017->line041]       "type": "object",
//@[line017->line042]       "defaultValue": {
//@[line017->line062]       }
//@[line017->line063]     },
  enabled: true
//@[line018->line043]         "enabled": true,
  name: 'this is my object'
//@[line019->line044]         "name": "this is my object",
  priority: 3
//@[line020->line045]         "priority": 3,
  info: {
//@[line021->line046]         "info": {
//@[line021->line048]         },
    a: 'b'
//@[line022->line047]           "a": "b"
  }
  empty: {
//@[line024->line049]         "empty": {},
  }
  array: [
//@[line026->line050]         "array": [
//@[line026->line061]         ]
    'string item'
//@[line027->line051]           "string item",
    12
//@[line028->line052]           12,
    true
//@[line029->line053]           true,
    [
//@[line030->line054]           [
//@[line030->line057]           ],
      'inner'
//@[line031->line055]             "inner",
      false
//@[line032->line056]             false
    ]
    {
//@[line034->line058]           {
//@[line034->line060]           }
      a: 'b'
//@[line035->line059]             "a": "b"
    }
  ]
}

// array default value
param myArrayParam array = [
//@[line041->line064]     "myArrayParam": {
//@[line041->line065]       "type": "array",
//@[line041->line066]       "defaultValue": [
//@[line041->line070]       ]
//@[line041->line071]     },
  'a'
//@[line042->line067]         "a",
  'b'
//@[line043->line068]         "b",
  'c'
//@[line044->line069]         "c"
]

// secure string
@secure()
param password string
//@[line049->line072]     "password": {
//@[line049->line073]       "type": "securestring"
//@[line049->line074]     },

// secure object
@secure()
param secretObject object
//@[line053->line075]     "secretObject": {
//@[line053->line076]       "type": "secureObject"
//@[line053->line077]     },

// enum parameter
@allowed([
//@[line056->line080]       "allowedValues": [
//@[line056->line083]       ]
  'Standard_LRS'
//@[line057->line081]         "Standard_LRS",
  'Standard_GRS'
//@[line058->line082]         "Standard_GRS"
])
param storageSku string
//@[line060->line078]     "storageSku": {
//@[line060->line079]       "type": "string",
//@[line060->line084]     },

// length constraint on a string
@minLength(3)
//@[line063->line088]       "minLength": 3
@maxLength(24)
//@[line064->line087]       "maxLength": 24,
param storageName string
//@[line065->line085]     "storageName": {
//@[line065->line086]       "type": "string",
//@[line065->line089]     },

// length constraint on an array
@minLength(3)
//@[line068->line093]       "minLength": 3
@maxLength(24)
//@[line069->line092]       "maxLength": 24,
param someArray array
//@[line070->line090]     "someArray": {
//@[line070->line091]       "type": "array",
//@[line070->line094]     },

// empty metadata
@metadata({})
//@[line073->line097]       "metadata": {}
param emptyMetadata string
//@[line074->line095]     "emptyMetadata": {
//@[line074->line096]       "type": "string",
//@[line074->line098]     },

// description
@metadata({
//@[line077->line101]       "metadata": {
//@[line077->line103]       }
  description: 'my description'
//@[line078->line102]         "description": "my description"
})
param description string
//@[line080->line099]     "description": {
//@[line080->line100]       "type": "string",
//@[line080->line104]     },

@sys.description('my description')
//@[line082->line108]         "description": "my description"
param description2 string
//@[line083->line105]     "description2": {
//@[line083->line106]       "type": "string",
//@[line083->line107]       "metadata": {
//@[line083->line109]       }
//@[line083->line110]     },

// random extra metadata
@metadata({
//@[line086->line113]       "metadata": {
//@[line086->line121]       }
  description: 'my description'
//@[line087->line114]         "description": "my description",
  a: 1
//@[line088->line115]         "a": 1,
  b: true
//@[line089->line116]         "b": true,
  c: [
//@[line090->line117]         "c": [],
  ]
  d: {
//@[line092->line118]         "d": {
//@[line092->line120]         }
    test: 'abc'
//@[line093->line119]           "test": "abc"
  }
})
param additionalMetadata string
//@[line096->line111]     "additionalMetadata": {
//@[line096->line112]       "type": "string",
//@[line096->line122]     },

// all modifiers together
@secure()
@minLength(3)
//@[line100->line134]       "minLength": 3
@maxLength(24)
//@[line101->line133]       "maxLength": 24,
@allowed([
//@[line102->line128]       "allowedValues": [
//@[line102->line132]       ],
  'one'
//@[line103->line129]         "one",
  'two'
//@[line104->line130]         "two",
  'three'
//@[line105->line131]         "three"
])
@metadata({
//@[line107->line125]       "metadata": {
//@[line107->line127]       },
  description: 'Name of the storage account'
//@[line108->line126]         "description": "Name of the storage account"
})
param someParameter string
//@[line110->line123]     "someParameter": {
//@[line110->line124]       "type": "securestring",
//@[line110->line135]     },

param defaultExpression bool = 18 != (true || false)
//@[line112->line136]     "defaultExpression": {
//@[line112->line137]       "type": "bool",
//@[line112->line138]       "defaultValue": "[not(equals(18, or(true(), false())))]"
//@[line112->line139]     },

@allowed([
//@[line114->line142]       "allowedValues": [
//@[line114->line145]       ]
  'abc'
//@[line115->line143]         "abc",
  'def'
//@[line116->line144]         "def"
])
param stringLiteral string
//@[line118->line140]     "stringLiteral": {
//@[line118->line141]       "type": "string",
//@[line118->line146]     },

@allowed([
//@[line120->line150]       "allowedValues": [
//@[line120->line154]       ]
  'abc'
//@[line121->line151]         "abc",
  'def'
//@[line122->line152]         "def",
  'ghi'
//@[line123->line153]         "ghi"
])
param stringLiteralWithAllowedValuesSuperset string = stringLiteral
//@[line125->line147]     "stringLiteralWithAllowedValuesSuperset": {
//@[line125->line148]       "type": "string",
//@[line125->line149]       "defaultValue": "[parameters('stringLiteral')]",
//@[line125->line155]     },

@secure()
@minLength(2)
//@[line128->line163]       "minLength": 2
  @maxLength(10)
//@[line129->line162]       "maxLength": 10,
@allowed([
//@[line130->line158]       "allowedValues": [
//@[line130->line161]       ],
  'Apple'
//@[line131->line159]         "Apple",
  'Banana'
//@[line132->line160]         "Banana"
])
param decoratedString string
//@[line134->line156]     "decoratedString": {
//@[line134->line157]       "type": "securestring",
//@[line134->line164]     },

@minValue(200)
//@[line136->line168]       "minValue": 200
param decoratedInt int = 123
//@[line137->line165]     "decoratedInt": {
//@[line137->line166]       "type": "int",
//@[line137->line167]       "defaultValue": 123,
//@[line137->line169]     },

// negative integer literals are allowed as decorator values
@minValue(-10)
//@[line140->line173]       "minValue": -10
@maxValue(-3)
//@[line141->line172]       "maxValue": -3,
param negativeValues int
//@[line142->line170]     "negativeValues": {
//@[line142->line171]       "type": "int",
//@[line142->line174]     },

@sys.description('A boolean.')
//@[line144->line179]         "description": "A boolean.",
@metadata({
    description: 'I will be overrode.'
    foo: 'something'
//@[line147->line180]         "foo": "something",
    bar: [
//@[line148->line181]         "bar": [
//@[line148->line185]         ]
        {          }
//@[line149->line182]           {},
        true
//@[line150->line183]           true,
        123
//@[line151->line184]           123
    ]
})
param decoratedBool bool = (true && false) != true
//@[line154->line175]     "decoratedBool": {
//@[line154->line176]       "type": "bool",
//@[line154->line177]       "defaultValue": "[not(equals(and(true(), false()), true()))]",
//@[line154->line178]       "metadata": {
//@[line154->line186]       }
//@[line154->line187]     },

@secure()
param decoratedObject object = {
//@[line157->line188]     "decoratedObject": {
//@[line157->line189]       "type": "secureObject",
//@[line157->line190]       "defaultValue": {
//@[line157->line210]       }
//@[line157->line211]     },
  enabled: true
//@[line158->line191]         "enabled": true,
  name: 'this is my object'
//@[line159->line192]         "name": "this is my object",
  priority: 3
//@[line160->line193]         "priority": 3,
  info: {
//@[line161->line194]         "info": {
//@[line161->line196]         },
    a: 'b'
//@[line162->line195]           "a": "b"
  }
  empty: {
//@[line164->line197]         "empty": {},
  }
  array: [
//@[line166->line198]         "array": [
//@[line166->line209]         ]
    'string item'
//@[line167->line199]           "string item",
    12
//@[line168->line200]           12,
    true
//@[line169->line201]           true,
    [
//@[line170->line202]           [
//@[line170->line205]           ],
      'inner'
//@[line171->line203]             "inner",
      false
//@[line172->line204]             false
    ]
    {
//@[line174->line206]           {
//@[line174->line208]           }
      a: 'b'
//@[line175->line207]             "a": "b"
    }
  ]
}

@sys.metadata({
    description: 'An array.'
//@[line181->line219]         "description": "An array."
})
@sys.maxLength(20)
//@[line183->line221]       "maxLength": 20
@sys.description('I will be overrode.')
param decoratedArray array = [
//@[line185->line212]     "decoratedArray": {
//@[line185->line213]       "type": "array",
//@[line185->line214]       "defaultValue": [
//@[line185->line217]       ],
//@[line185->line218]       "metadata": {
//@[line185->line220]       },
//@[line185->line222]     }
    utcNow()
//@[line186->line215]         "[utcNow()]",
    newGuid()
//@[line187->line216]         "[newGuid()]"
]

