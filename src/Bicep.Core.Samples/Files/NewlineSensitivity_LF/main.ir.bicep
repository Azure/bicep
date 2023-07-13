@allowed(['abc', 'def', 'ghi'])
//@[00:1053) ProgramExpression
//@[00:0048) ├─DeclaredParameterExpression { Name = foo }
param foo string
//@[10:0016) | └─AmbientTypeReferenceExpression { Name = string }

var singleLineFunction = concat('abc', 'def')
//@[00:0045) ├─DeclaredVariableExpression { Name = singleLineFunction }
//@[25:0045) | └─FunctionCallExpression { Name = concat }
//@[32:0037) |   ├─StringLiteralExpression { Value = abc }
//@[39:0044) |   └─StringLiteralExpression { Value = def }

var multiLineFunction = concat(
//@[00:0050) ├─DeclaredVariableExpression { Name = multiLineFunction }
//@[24:0050) | └─FunctionCallExpression { Name = concat }
  'abc',
//@[02:0007) |   ├─StringLiteralExpression { Value = abc }
  'def'
//@[02:0007) |   └─StringLiteralExpression { Value = def }
)

var multiLineFunctionUnusualFormatting = concat(
//@[00:0101) ├─DeclaredVariableExpression { Name = multiLineFunctionUnusualFormatting }
//@[41:0101) | └─FunctionCallExpression { Name = concat }
              'abc',          any(['hello']),
//@[14:0019) |   ├─StringLiteralExpression { Value = abc }
//@[34:0043) |   ├─ArrayExpression
//@[35:0042) |   | └─StringLiteralExpression { Value = hello }
'def')
//@[00:0005) |   └─StringLiteralExpression { Value = def }

var nestedTest = concat(
//@[00:0108) ├─DeclaredVariableExpression { Name = nestedTest }
//@[17:0108) | └─FunctionCallExpression { Name = concat }
concat(
//@[00:0074) |   ├─FunctionCallExpression { Name = concat }
concat(
//@[00:0057) |   | ├─FunctionCallExpression { Name = concat }
concat(
//@[00:0039) |   | | ├─FunctionCallExpression { Name = concat }
concat(
//@[00:0023) |   | | | ├─FunctionCallExpression { Name = concat }
'level',
//@[00:0007) |   | | | | ├─StringLiteralExpression { Value = level }
'one'),
//@[00:0005) |   | | | | └─StringLiteralExpression { Value = one }
'two'),
//@[00:0005) |   | | | └─StringLiteralExpression { Value = two }
'three'),
//@[00:0007) |   | | └─StringLiteralExpression { Value = three }
'four'),
//@[00:0006) |   | └─StringLiteralExpression { Value = four }
'five')
//@[00:0006) |   └─StringLiteralExpression { Value = five }

var singleLineArray = ['abc', 'def']
//@[00:0036) ├─DeclaredVariableExpression { Name = singleLineArray }
//@[22:0036) | └─ArrayExpression
//@[23:0028) |   ├─StringLiteralExpression { Value = abc }
//@[30:0035) |   └─StringLiteralExpression { Value = def }
var singleLineArrayTrailingCommas = ['abc', 'def',]
//@[00:0051) ├─DeclaredVariableExpression { Name = singleLineArrayTrailingCommas }
//@[36:0051) | └─ArrayExpression
//@[37:0042) |   ├─StringLiteralExpression { Value = abc }
//@[44:0049) |   └─StringLiteralExpression { Value = def }

var multiLineArray = [
//@[00:0040) ├─DeclaredVariableExpression { Name = multiLineArray }
//@[21:0040) | └─ArrayExpression
  'abc'
//@[02:0007) |   ├─StringLiteralExpression { Value = abc }
  'def'
//@[02:0007) |   └─StringLiteralExpression { Value = def }
]

var mixedArray = ['abc', 'def'
//@[00:0050) ├─DeclaredVariableExpression { Name = mixedArray }
//@[17:0050) | └─ArrayExpression
//@[18:0023) |   ├─StringLiteralExpression { Value = abc }
//@[25:0030) |   ├─StringLiteralExpression { Value = def }
'ghi', 'jkl'
//@[00:0005) |   ├─StringLiteralExpression { Value = ghi }
//@[07:0012) |   ├─StringLiteralExpression { Value = jkl }
'lmn']
//@[00:0005) |   └─StringLiteralExpression { Value = lmn }

var singleLineObject = { abc: 'def', ghi: 'jkl'}
//@[00:0048) ├─DeclaredVariableExpression { Name = singleLineObject }
//@[23:0048) | └─ObjectExpression
//@[25:0035) |   ├─ObjectPropertyExpression
//@[25:0028) |   | ├─StringLiteralExpression { Value = abc }
//@[30:0035) |   | └─StringLiteralExpression { Value = def }
//@[37:0047) |   └─ObjectPropertyExpression
//@[37:0040) |     ├─StringLiteralExpression { Value = ghi }
//@[42:0047) |     └─StringLiteralExpression { Value = jkl }
var singleLineObjectTrailingCommas = { abc: 'def', ghi: 'jkl',}
//@[00:0063) ├─DeclaredVariableExpression { Name = singleLineObjectTrailingCommas }
//@[37:0063) | └─ObjectExpression
//@[39:0049) |   ├─ObjectPropertyExpression
//@[39:0042) |   | ├─StringLiteralExpression { Value = abc }
//@[44:0049) |   | └─StringLiteralExpression { Value = def }
//@[51:0061) |   └─ObjectPropertyExpression
//@[51:0054) |     ├─StringLiteralExpression { Value = ghi }
//@[56:0061) |     └─StringLiteralExpression { Value = jkl }
var multiLineObject = {
//@[00:0051) ├─DeclaredVariableExpression { Name = multiLineObject }
//@[22:0051) | └─ObjectExpression
  abc: 'def'
//@[02:0012) |   ├─ObjectPropertyExpression
//@[02:0005) |   | ├─StringLiteralExpression { Value = abc }
//@[07:0012) |   | └─StringLiteralExpression { Value = def }
  ghi: 'jkl'
//@[02:0012) |   └─ObjectPropertyExpression
//@[02:0005) |     ├─StringLiteralExpression { Value = ghi }
//@[07:0012) |     └─StringLiteralExpression { Value = jkl }
}
var mixedObject = { abc: 'abc', def: 'def'
//@[00:0078) ├─DeclaredVariableExpression { Name = mixedObject }
//@[18:0078) | └─ObjectExpression
//@[20:0030) |   ├─ObjectPropertyExpression
//@[20:0023) |   | ├─StringLiteralExpression { Value = abc }
//@[25:0030) |   | └─StringLiteralExpression { Value = abc }
//@[32:0042) |   ├─ObjectPropertyExpression
//@[32:0035) |   | ├─StringLiteralExpression { Value = def }
//@[37:0042) |   | └─StringLiteralExpression { Value = def }
ghi: 'ghi', jkl: 'jkl'
//@[00:0010) |   ├─ObjectPropertyExpression
//@[00:0003) |   | ├─StringLiteralExpression { Value = ghi }
//@[05:0010) |   | └─StringLiteralExpression { Value = ghi }
//@[12:0022) |   ├─ObjectPropertyExpression
//@[12:0015) |   | ├─StringLiteralExpression { Value = jkl }
//@[17:0022) |   | └─StringLiteralExpression { Value = jkl }
lmn: 'lmn' }
//@[00:0010) |   └─ObjectPropertyExpression
//@[00:0003) |     ├─StringLiteralExpression { Value = lmn }
//@[05:0010) |     └─StringLiteralExpression { Value = lmn }

var nestedMixed = {
//@[00:0087) ├─DeclaredVariableExpression { Name = nestedMixed }
//@[18:0087) | └─ObjectExpression
  abc: { 'def': 'ghi', abc: 'def', foo: [
//@[02:0065) |   └─ObjectPropertyExpression
//@[02:0005) |     ├─StringLiteralExpression { Value = abc }
//@[07:0065) |     └─ObjectExpression
//@[09:0021) |       ├─ObjectPropertyExpression
//@[09:0014) |       | ├─StringLiteralExpression { Value = def }
//@[16:0021) |       | └─StringLiteralExpression { Value = ghi }
//@[23:0033) |       ├─ObjectPropertyExpression
//@[23:0026) |       | ├─StringLiteralExpression { Value = abc }
//@[28:0033) |       | └─StringLiteralExpression { Value = def }
//@[35:0063) |       └─ObjectPropertyExpression
//@[35:0038) |         ├─StringLiteralExpression { Value = foo }
//@[40:0063) |         └─ArrayExpression
    'bar', 'blah'
//@[04:0009) |           ├─StringLiteralExpression { Value = bar }
//@[11:0017) |           └─StringLiteralExpression { Value = blah }
  ] }
}

var brokenFormatting = [      /*foo */ 'bar'   /*
//@[00:0172) └─DeclaredVariableExpression { Name = brokenFormatting }
//@[23:0172)   └─ArrayExpression
//@[39:0044)     ├─StringLiteralExpression { Value = bar }

hello

*/,        'asdfdsf',             12324,       /*   asdf*/ '',     '''
//@[11:0020)     ├─StringLiteralExpression { Value = asdfdsf }
//@[34:0039)     ├─IntegerLiteralExpression { Value = 12324 }
//@[59:0061)     ├─StringLiteralExpression { Value =  }
//@[67:0076)     ├─StringLiteralExpression { Value = \n\n }


'''
123,      233535
//@[00:0003)     ├─IntegerLiteralExpression { Value = 123 }
//@[10:0016)     ├─IntegerLiteralExpression { Value = 233535 }
true
//@[00:0004)     └─BooleanLiteralExpression { Value = True }
              ]

