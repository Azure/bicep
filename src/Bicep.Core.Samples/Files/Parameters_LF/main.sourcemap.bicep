/*
  This is a block comment.
*/

// parameters without default value
@sys.description('''
//@[line005->line014]         "description": "this is my multi line\ndescription for my myString\n"
this is my multi line
description for my myString
''')
param myString string
//@[line009->line011]     "myString": {
//@[line009->line012]       "type": "string",
//@[line009->line013]       "metadata": {
//@[line009->line015]       }
//@[line009->line016]     },
param myInt int
//@[line010->line017]     "myInt": {
//@[line010->line018]       "type": "int"
//@[line010->line019]     },
param myBool bool
//@[line011->line020]     "myBool": {
//@[line011->line021]       "type": "bool"
//@[line011->line022]     },

// parameters with default value
@sys.description('this is myString2')
//@[line014->line027]         "description": "this is myString2"
@metadata({
  description: 'overwrite but still valid'
})
param myString2 string = 'string value'
//@[line018->line023]     "myString2": {
//@[line018->line024]       "type": "string",
//@[line018->line025]       "defaultValue": "string value",
//@[line018->line026]       "metadata": {
//@[line018->line028]       }
//@[line018->line029]     },
param myInt2 int = 42
//@[line019->line030]     "myInt2": {
//@[line019->line031]       "type": "int",
//@[line019->line032]       "defaultValue": 42
//@[line019->line033]     },
param myTruth bool = true
//@[line020->line034]     "myTruth": {
//@[line020->line035]       "type": "bool",
//@[line020->line036]       "defaultValue": true
//@[line020->line037]     },
param myFalsehood bool = false
//@[line021->line038]     "myFalsehood": {
//@[line021->line039]       "type": "bool",
//@[line021->line040]       "defaultValue": false
//@[line021->line041]     },
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[line022->line042]     "myEscapedString": {
//@[line022->line043]       "type": "string",
//@[line022->line044]       "defaultValue": "First line\r\nSecond\ttabbed\tline"
//@[line022->line045]     },

// object default value
@sys.description('this is foo')
//@[line025->line070]         "description": "this is foo",
@metadata({
  description: 'overwrite but still valid'
  another: 'just for fun'
//@[line028->line071]         "another": "just for fun"
})
param foo object = {
//@[line030->line046]     "foo": {
//@[line030->line047]       "type": "object",
//@[line030->line048]       "defaultValue": {
//@[line030->line068]       },
//@[line030->line069]       "metadata": {
//@[line030->line072]       }
//@[line030->line073]     },
  enabled: true
//@[line031->line049]         "enabled": true,
  name: 'this is my object'
//@[line032->line050]         "name": "this is my object",
  priority: 3
//@[line033->line051]         "priority": 3,
  info: {
//@[line034->line052]         "info": {
//@[line034->line054]         },
    a: 'b'
//@[line035->line053]           "a": "b"
  }
  empty: {
//@[line037->line055]         "empty": {},
  }
  array: [
//@[line039->line056]         "array": [
//@[line039->line067]         ]
    'string item'
//@[line040->line057]           "string item",
    12
//@[line041->line058]           12,
    true
//@[line042->line059]           true,
    [
//@[line043->line060]           [
//@[line043->line063]           ],
      'inner'
//@[line044->line061]             "inner",
      false
//@[line045->line062]             false
    ]
    {
//@[line047->line064]           {
//@[line047->line066]           }
      a: 'b'
//@[line048->line065]             "a": "b"
    }
  ]
}

// array default value
param myArrayParam array = [
//@[line054->line074]     "myArrayParam": {
//@[line054->line075]       "type": "array",
//@[line054->line076]       "defaultValue": [
//@[line054->line080]       ]
//@[line054->line081]     },
  'a'
//@[line055->line077]         "a",
  'b'
//@[line056->line078]         "b",
  'c'
//@[line057->line079]         "c"
]

// secure string
@secure()
param password string
//@[line062->line082]     "password": {
//@[line062->line083]       "type": "securestring"
//@[line062->line084]     },

// secure object
@secure()
param secretObject object
//@[line066->line085]     "secretObject": {
//@[line066->line086]       "type": "secureObject"
//@[line066->line087]     },

// enum parameter
@allowed([
//@[line069->line090]       "allowedValues": [
//@[line069->line093]       ]
  'Standard_LRS'
//@[line070->line091]         "Standard_LRS",
  'Standard_GRS'
//@[line071->line092]         "Standard_GRS"
])
param storageSku string
//@[line073->line088]     "storageSku": {
//@[line073->line089]       "type": "string",
//@[line073->line094]     },

@allowed([
//@[line075->line097]       "allowedValues": [
//@[line075->line101]       ]
  1
//@[line076->line098]         1,
  2
//@[line077->line099]         2,
  3
//@[line078->line100]         3
])
param intEnum int
//@[line080->line095]     "intEnum": {
//@[line080->line096]       "type": "int",
//@[line080->line102]     },

// length constraint on a string
@minLength(3)
//@[line083->line106]       "minLength": 3
@maxLength(24)
//@[line084->line105]       "maxLength": 24,
param storageName string
//@[line085->line103]     "storageName": {
//@[line085->line104]       "type": "string",
//@[line085->line107]     },

// length constraint on an array
@minLength(3)
//@[line088->line111]       "minLength": 3
@maxLength(24)
//@[line089->line110]       "maxLength": 24,
param someArray array
//@[line090->line108]     "someArray": {
//@[line090->line109]       "type": "array",
//@[line090->line112]     },

// empty metadata
@metadata({})
//@[line093->line115]       "metadata": {}
param emptyMetadata string
//@[line094->line113]     "emptyMetadata": {
//@[line094->line114]       "type": "string",
//@[line094->line116]     },

// description
@metadata({
//@[line097->line119]       "metadata": {
//@[line097->line121]       }
  description: 'my description'
//@[line098->line120]         "description": "my description"
})
param description string
//@[line100->line117]     "description": {
//@[line100->line118]       "type": "string",
//@[line100->line122]     },

@sys.description('my description')
//@[line102->line126]         "description": "my description"
param description2 string
//@[line103->line123]     "description2": {
//@[line103->line124]       "type": "string",
//@[line103->line125]       "metadata": {
//@[line103->line127]       }
//@[line103->line128]     },

// random extra metadata
@metadata({
//@[line106->line131]       "metadata": {
//@[line106->line139]       }
  description: 'my description'
//@[line107->line132]         "description": "my description",
  a: 1
//@[line108->line133]         "a": 1,
  b: true
//@[line109->line134]         "b": true,
  c: [
//@[line110->line135]         "c": [],
  ]
  d: {
//@[line112->line136]         "d": {
//@[line112->line138]         }
    test: 'abc'
//@[line113->line137]           "test": "abc"
  }
})
param additionalMetadata string
//@[line116->line129]     "additionalMetadata": {
//@[line116->line130]       "type": "string",
//@[line116->line140]     },

// all modifiers together
@secure()
@minLength(3)
//@[line120->line152]       "minLength": 3
@maxLength(24)
//@[line121->line151]       "maxLength": 24,
@allowed([
//@[line122->line146]       "allowedValues": [
//@[line122->line150]       ],
  'one'
//@[line123->line147]         "one",
  'two'
//@[line124->line148]         "two",
  'three'
//@[line125->line149]         "three"
])
@metadata({
//@[line127->line143]       "metadata": {
//@[line127->line145]       },
  description: 'Name of the storage account'
//@[line128->line144]         "description": "Name of the storage account"
})
param someParameter string
//@[line130->line141]     "someParameter": {
//@[line130->line142]       "type": "securestring",
//@[line130->line153]     },

param defaultExpression bool = 18 != (true || false)
//@[line132->line154]     "defaultExpression": {
//@[line132->line155]       "type": "bool",
//@[line132->line156]       "defaultValue": "[not(equals(18, or(true(), false())))]"
//@[line132->line157]     },

@allowed([
//@[line134->line160]       "allowedValues": [
//@[line134->line163]       ]
  'abc'
//@[line135->line161]         "abc",
  'def'
//@[line136->line162]         "def"
])
param stringLiteral string
//@[line138->line158]     "stringLiteral": {
//@[line138->line159]       "type": "string",
//@[line138->line164]     },

@allowed([
//@[line140->line168]       "allowedValues": [
//@[line140->line172]       ]
  'abc'
//@[line141->line169]         "abc",
  'def'
//@[line142->line170]         "def",
  'ghi'
//@[line143->line171]         "ghi"
])
param stringLiteralWithAllowedValuesSuperset string = stringLiteral
//@[line145->line165]     "stringLiteralWithAllowedValuesSuperset": {
//@[line145->line166]       "type": "string",
//@[line145->line167]       "defaultValue": "[parameters('stringLiteral')]",
//@[line145->line173]     },

@secure()
@minLength(2)
//@[line148->line181]       "minLength": 2
  @maxLength(10)
//@[line149->line180]       "maxLength": 10,
@allowed([
//@[line150->line176]       "allowedValues": [
//@[line150->line179]       ],
  'Apple'
//@[line151->line177]         "Apple",
  'Banana'
//@[line152->line178]         "Banana"
])
param decoratedString string
//@[line154->line174]     "decoratedString": {
//@[line154->line175]       "type": "securestring",
//@[line154->line182]     },

@minValue(200)
//@[line156->line186]       "minValue": 200
param decoratedInt int = 123
//@[line157->line183]     "decoratedInt": {
//@[line157->line184]       "type": "int",
//@[line157->line185]       "defaultValue": 123,
//@[line157->line187]     },

// negative integer literals are allowed as decorator values
@minValue(-10)
//@[line160->line191]       "minValue": -10
@maxValue(-3)
//@[line161->line190]       "maxValue": -3,
param negativeValues int
//@[line162->line188]     "negativeValues": {
//@[line162->line189]       "type": "int",
//@[line162->line192]     },

@sys.description('A boolean.')
//@[line164->line197]         "description": "A boolean.",
@metadata({
    description: 'I will be overrode.'
    foo: 'something'
//@[line167->line198]         "foo": "something",
    bar: [
//@[line168->line199]         "bar": [
//@[line168->line203]         ]
        {          }
//@[line169->line200]           {},
        true
//@[line170->line201]           true,
        123
//@[line171->line202]           123
    ]
})
param decoratedBool bool = /* comment1 */ /* comment2*/      /* comment3 */ /* comment4 */ (true && false) != true
//@[line174->line193]     "decoratedBool": {
//@[line174->line194]       "type": "bool",
//@[line174->line195]       "defaultValue": "[not(equals(and(true(), false()), true()))]",
//@[line174->line196]       "metadata": {
//@[line174->line204]       }
//@[line174->line205]     },

@secure()
param decoratedObject object = {
//@[line177->line206]     "decoratedObject": {
//@[line177->line207]       "type": "secureObject",
//@[line177->line208]       "defaultValue": {
//@[line177->line228]       }
//@[line177->line229]     },
  enabled: true
//@[line178->line209]         "enabled": true,
  name: 'this is my object'
//@[line179->line210]         "name": "this is my object",
  priority: 3
//@[line180->line211]         "priority": 3,
  info: {
//@[line181->line212]         "info": {
//@[line181->line214]         },
    a: 'b'
//@[line182->line213]           "a": "b"
  }
  empty: {
//@[line184->line215]         "empty": {},
  }
  array: [
//@[line186->line216]         "array": [
//@[line186->line227]         ]
    'string item'
//@[line187->line217]           "string item",
    12
//@[line188->line218]           12,
    true
//@[line189->line219]           true,
    [
//@[line190->line220]           [
//@[line190->line223]           ],
      'inner'
//@[line191->line221]             "inner",
      false
//@[line192->line222]             false
    ]
    {
//@[line194->line224]           {
//@[line194->line226]           }
      a: 'b'
//@[line195->line225]             "a": "b"
    }
  ]
}

@sys.metadata({
    description: 'An array.'
//@[line201->line237]         "description": "An array."
})
@sys.maxLength(20)
//@[line203->line239]       "maxLength": 20
@sys.description('I will be overrode.')
param decoratedArray array = [
//@[line205->line230]     "decoratedArray": {
//@[line205->line231]       "type": "array",
//@[line205->line232]       "defaultValue": [
//@[line205->line235]       ],
//@[line205->line236]       "metadata": {
//@[line205->line238]       },
//@[line205->line240]     }
    utcNow()
//@[line206->line233]         "[utcNow()]",
    newGuid()
//@[line207->line234]         "[newGuid()]"
]

