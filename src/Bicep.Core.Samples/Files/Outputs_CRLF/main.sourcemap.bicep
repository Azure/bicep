
@sys.description('string output description')
//@[line01->line024]         "description": "string output description"
output myStr string = 'hello'
//@[line02->line020]     "myStr": {
//@[line02->line021]       "type": "string",
//@[line02->line022]       "value": "hello",
//@[line02->line023]       "metadata": {
//@[line02->line025]       }
//@[line02->line026]     },

@sys.description('int output description')
//@[line04->line031]         "description": "int output description"
output myInt int = 7
//@[line05->line027]     "myInt": {
//@[line05->line028]       "type": "int",
//@[line05->line029]       "value": 7,
//@[line05->line030]       "metadata": {
//@[line05->line032]       }
//@[line05->line033]     },
output myOtherInt int = 20 / 13 + 80 % -4
//@[line06->line034]     "myOtherInt": {
//@[line06->line035]       "type": "int",
//@[line06->line036]       "value": "[add(div(20, 13), mod(80, -4))]"
//@[line06->line037]     },

@sys.description('bool output description')
//@[line08->line042]         "description": "bool output description"
output myBool bool = !false
//@[line09->line038]     "myBool": {
//@[line09->line039]       "type": "bool",
//@[line09->line040]       "value": "[not(false())]",
//@[line09->line041]       "metadata": {
//@[line09->line043]       }
//@[line09->line044]     },
output myOtherBool bool = true
//@[line10->line045]     "myOtherBool": {
//@[line10->line046]       "type": "bool",
//@[line10->line047]       "value": true
//@[line10->line048]     },

@sys.description('object array description')
//@[line12->line053]         "description": "object array description"
output suchEmpty array = [
//@[line13->line049]     "suchEmpty": {
//@[line13->line050]       "type": "array",
//@[line13->line051]       "value": [],
//@[line13->line052]       "metadata": {
//@[line13->line054]       }
//@[line13->line055]     },
]

output suchEmpty2 object = {
//@[line16->line056]     "suchEmpty2": {
//@[line16->line057]       "type": "object",
//@[line16->line058]       "value": {}
//@[line16->line059]     },
}

@sys.description('object output description')
//@[line19->line081]         "description": "object output description"
output obj object = {
//@[line20->line060]     "obj": {
//@[line20->line061]       "type": "object",
//@[line20->line062]       "value": {
//@[line20->line079]       },
//@[line20->line080]       "metadata": {
//@[line20->line082]       }
//@[line20->line083]     },
  a: 'a'
//@[line21->line063]         "a": "a",
  b: 12
//@[line22->line064]         "b": 12,
  c: true
//@[line23->line065]         "c": true,
  d: null
//@[line24->line066]         "d": null,
  list: [
//@[line25->line067]         "list": [
//@[line25->line073]         ],
    1
//@[line26->line068]           1,
    2
//@[line27->line069]           2,
    3
//@[line28->line070]           3,
    null
//@[line29->line071]           null,
    {
//@[line30->line072]           {}
    }
  ]
  obj: {
//@[line33->line074]         "obj": {
//@[line33->line078]         }
    nested: [
//@[line34->line075]           "nested": [
//@[line34->line077]           ]
      'hello'
//@[line35->line076]             "hello"
    ]
  }
}

output myArr array = [
//@[line40->line084]     "myArr": {
//@[line40->line085]       "type": "array",
//@[line40->line086]       "value": [
//@[line40->line090]       ]
//@[line40->line091]     },
  'pirates'
//@[line41->line087]         "pirates",
  'say'
//@[line42->line088]         "say",
   false ? 'arr2' : 'arr'
//@[line43->line089]         "[if(false(), 'arr2', 'arr')]"
]

output rgLocation string = resourceGroup().location
//@[line46->line092]     "rgLocation": {
//@[line46->line093]       "type": "string",
//@[line46->line094]       "value": "[resourceGroup().location]"
//@[line46->line095]     },

output isWestUs bool = resourceGroup().location != 'westus' ? false : true
//@[line48->line096]     "isWestUs": {
//@[line48->line097]       "type": "bool",
//@[line48->line098]       "value": "[if(not(equals(resourceGroup().location, 'westus')), false(), true())]"
//@[line48->line099]     },

output expressionBasedIndexer string = {
//@[line50->line100]     "expressionBasedIndexer": {
//@[line50->line101]       "type": "string",
//@[line50->line102]       "value": "[createObject('eastus', createObject('foo', true()), 'westus', createObject('foo', false()))[resourceGroup().location].foo]"
//@[line50->line103]     },
  eastus: {
    foo: true
  }
  westus: {
    foo: false
  }
}[resourceGroup().location].foo

var secondaryKeyIntermediateVar = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01').secondaryKey

output primaryKey string = listKeys(resourceId('Mock.RP/type', 'nigel'), '2020-01-01').primaryKey
//@[line61->line104]     "primaryKey": {
//@[line61->line105]       "type": "string",
//@[line61->line106]       "value": "[listKeys(resourceId('Mock.RP/type', 'nigel'), '2020-01-01').primaryKey]"
//@[line61->line107]     },
output secondaryKey string = secondaryKeyIntermediateVar
//@[line62->line108]     "secondaryKey": {
//@[line62->line109]       "type": "string",
//@[line62->line110]       "value": "[listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01').secondaryKey]"
//@[line62->line111]     },

var varWithOverlappingOutput = 'hello'
//@[line64->line016]     "varWithOverlappingOutput": "hello"
param paramWithOverlappingOutput string
//@[line65->line011]     "paramWithOverlappingOutput": {
//@[line65->line012]       "type": "string"
//@[line65->line013]     }

output varWithOverlappingOutput string = varWithOverlappingOutput
//@[line67->line112]     "varWithOverlappingOutput": {
//@[line67->line113]       "type": "string",
//@[line67->line114]       "value": "[variables('varWithOverlappingOutput')]"
//@[line67->line115]     },
output paramWithOverlappingOutput string = paramWithOverlappingOutput
//@[line68->line116]     "paramWithOverlappingOutput": {
//@[line68->line117]       "type": "string",
//@[line68->line118]       "value": "[parameters('paramWithOverlappingOutput')]"
//@[line68->line119]     },

// top-level output loops are supported
output generatedArray array = [for i in range(0,10): i]
//@[line71->line120]     "generatedArray": {
//@[line71->line121]       "type": "array",
//@[line71->line122]       "copy": {
//@[line71->line123]         "count": "[length(range(0, 10))]",
//@[line71->line124]         "input": "[range(0, 10)[copyIndex()]]"
//@[line71->line125]       }
//@[line71->line126]     }

