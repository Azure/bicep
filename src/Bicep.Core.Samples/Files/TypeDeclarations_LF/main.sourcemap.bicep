@description('The foo type')
//@[line00->line066]         "description": "The foo type"
@sealed()
type foo = {
//@[line02->line013]     "foo": {
//@[line02->line014]       "type": "object",
//@[line02->line015]       "required": [
//@[line02->line016]         "stringProp",
//@[line02->line017]         "objectProp",
//@[line02->line018]         "typeRefProp",
//@[line02->line019]         "literalProp"
//@[line02->line020]       ],
//@[line02->line021]       "properties": {
//@[line02->line022]         "stringProp": {
//@[line02->line023]           "type": "string",
//@[line02->line024]           "metadata": {
//@[line02->line026]           },
//@[line02->line029]         },
//@[line02->line030]         "objectProp": {
//@[line02->line031]           "type": "object",
//@[line02->line032]           "required": [
//@[line02->line033]             "intProp"
//@[line02->line034]           ],
//@[line02->line035]           "properties": {
//@[line02->line036]             "intProp": {
//@[line02->line037]               "type": "int",
//@[line02->line039]             },
//@[line02->line040]             "intArrayArrayProp": {
//@[line02->line041]               "type": "array",
//@[line02->line042]               "items": {
//@[line02->line043]                 "type": "array",
//@[line02->line044]                 "items": {
//@[line02->line045]                   "type": "int"
//@[line02->line046]                 }
//@[line02->line047]               }
//@[line02->line048]             }
//@[line02->line049]           }
//@[line02->line050]         },
//@[line02->line051]         "typeRefProp": {
//@[line02->line052]           "$ref": "#/definitions/bar"
//@[line02->line053]         },
//@[line02->line054]         "literalProp": {
//@[line02->line055]           "type": "string",
//@[line02->line056]           "allowedValues": [
//@[line02->line058]           ]
//@[line02->line059]         },
//@[line02->line060]         "recursion": {
//@[line02->line061]           "$ref": "#/definitions/foo"
//@[line02->line062]         }
//@[line02->line063]       },
//@[line02->line064]       "sealed": "true",
//@[line02->line065]       "metadata": {
//@[line02->line067]       }
//@[line02->line068]     },
  @minLength(3)
//@[line03->line028]           "minLength": 3
  @maxLength(10)
//@[line04->line027]           "maxLength": 10,
  @description('A string property')
//@[line05->line025]             "description": "A string property"
  stringProp: string

  objectProp: {
    @minValue(1)
//@[line09->line038]               "minValue": 1
    intProp: int

    intArrayArrayProp?: int [] []
  }

  typeRefProp: bar

  literalProp: 'literal'
//@[line17->line057]             "literal"

  recursion?: foo
}

@minLength(3)
//@[line22->line111]       "minLength": 3
@description('An array of array of arrays of arrays of ints')
//@[line23->line109]         "description": "An array of array of arrays of arrays of ints"
@metadata({
  examples: [
//@[line25->line084]         "examples": [
//@[line25->line108]         ],
    [[[[1]]], [[[2]]], [[[3]]]]
//@[line26->line085]           [
//@[line26->line086]             [
//@[line26->line087]               [
//@[line26->line088]                 [
//@[line26->line089]                   1
//@[line26->line090]                 ]
//@[line26->line091]               ]
//@[line26->line092]             ],
//@[line26->line093]             [
//@[line26->line094]               [
//@[line26->line095]                 [
//@[line26->line096]                   2
//@[line26->line097]                 ]
//@[line26->line098]               ]
//@[line26->line099]             ],
//@[line26->line100]             [
//@[line26->line101]               [
//@[line26->line102]                 [
//@[line26->line103]                   3
//@[line26->line104]                 ]
//@[line26->line105]               ]
//@[line26->line106]             ]
//@[line26->line107]           ]
  ]
})
type bar = int[][][][]
//@[line29->line069]     "bar": {
//@[line29->line070]       "type": "array",
//@[line29->line071]       "items": {
//@[line29->line072]         "type": "array",
//@[line29->line073]         "items": {
//@[line29->line074]           "type": "array",
//@[line29->line075]           "items": {
//@[line29->line076]             "type": "array",
//@[line29->line077]             "items": {
//@[line29->line078]               "type": "int"
//@[line29->line079]             }
//@[line29->line080]           }
//@[line29->line081]         }
//@[line29->line082]       },
//@[line29->line083]       "metadata": {
//@[line29->line110]       },
//@[line29->line112]     },

type aUnion = 'snap'|'crackle'|'pop'
//@[line31->line113]     "aUnion": {
//@[line31->line114]       "type": "string",
//@[line31->line115]       "allowedValues": [
//@[line31->line116]         "crackle",
//@[line31->line117]         "pop",
//@[line31->line118]         "snap"
//@[line31->line119]       ]
//@[line31->line120]     },

type expandedUnion = aUnion|'fizz'|'buzz'|'pop'
//@[line33->line121]     "expandedUnion": {
//@[line33->line122]       "type": "string",
//@[line33->line123]       "allowedValues": [
//@[line33->line124]         "buzz",
//@[line33->line125]         "crackle",
//@[line33->line126]         "fizz",
//@[line33->line127]         "pop",
//@[line33->line128]         "snap"
//@[line33->line129]       ]
//@[line33->line130]     },

type mixedArray = ('heffalump'|'woozle'|{ shape: '*', size: '*'}|10|-10|true|!true|null)[]
//@[line35->line131]     "mixedArray": {
//@[line35->line132]       "type": "array",
//@[line35->line133]       "allowedValues": [
//@[line35->line134]         "heffalump",
//@[line35->line135]         "woozle",
//@[line35->line136]         -10,
//@[line35->line137]         10,
//@[line35->line138]         false,
//@[line35->line139]         null,
//@[line35->line140]         true,
//@[line35->line141]         {
//@[line35->line142]           "shape": "*",
//@[line35->line143]           "size": "*"
//@[line35->line144]         }
//@[line35->line145]       ]
//@[line35->line146]     },

type bool = string
//@[line37->line147]     "bool": {
//@[line37->line148]       "type": "string"
//@[line37->line149]     }

param inlineObjectParam {
//@[line39->line152]     "inlineObjectParam": {
//@[line39->line153]       "type": "object",
//@[line39->line154]       "required": [
//@[line39->line155]         "foo",
//@[line39->line156]         "bar",
//@[line39->line157]         "baz"
//@[line39->line158]       ],
//@[line39->line159]       "properties": {
//@[line39->line160]         "foo": {
//@[line39->line161]           "type": "string"
//@[line39->line162]         },
//@[line39->line163]         "bar": {
//@[line39->line164]           "type": "int",
//@[line39->line165]           "allowedValues": [
//@[line39->line166]             100,
//@[line39->line167]             200,
//@[line39->line168]             300,
//@[line39->line169]             400,
//@[line39->line170]             500
//@[line39->line171]           ]
//@[line39->line172]         },
//@[line39->line173]         "baz": {
//@[line39->line174]           "type": "bool"
//@[line39->line175]         }
//@[line39->line176]       },
//@[line39->line182]     },
  foo: string
  bar: 100|200|300|400|500
  baz: sys.bool
} = {
//@[line43->line177]       "defaultValue": {
//@[line43->line181]       }
  foo: 'foo'
//@[line44->line178]         "foo": "foo",
  bar: 300
//@[line45->line179]         "bar": 300,
  baz: false
//@[line46->line180]         "baz": false
}

param unionParam {property: 'ping'}|{property: 'pong'} = {property: 'pong'}
//@[line49->line183]     "unionParam": {
//@[line49->line184]       "type": "object",
//@[line49->line185]       "allowedValues": [
//@[line49->line186]         {
//@[line49->line187]           "property": "ping"
//@[line49->line188]         },
//@[line49->line189]         {
//@[line49->line190]           "property": "pong"
//@[line49->line191]         }
//@[line49->line192]       ],
//@[line49->line193]       "defaultValue": {
//@[line49->line194]         "property": "pong"
//@[line49->line195]       }
//@[line49->line196]     },

param paramUsingType mixedArray
//@[line51->line197]     "paramUsingType": {
//@[line51->line198]       "$ref": "#/definitions/mixedArray"
//@[line51->line199]     }

