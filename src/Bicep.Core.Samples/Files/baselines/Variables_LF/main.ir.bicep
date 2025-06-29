
//@[000:8214) ProgramExpression
// int
@sys.description('an int variable')
//@[000:0050) ├─DeclaredVariableExpression { Name = myInt }
//@[017:0034) | ├─StringLiteralExpression { Value = an int variable }
var myInt = 42
//@[012:0014) | └─IntegerLiteralExpression { Value = 42 }

// string
@sys.description('a string variable')
//@[000:0055) ├─DeclaredVariableExpression { Name = myStr }
//@[017:0036) | ├─StringLiteralExpression { Value = a string variable }
var myStr = 'str'
//@[012:0017) | └─StringLiteralExpression { Value = str }
var curliesWithNoInterp = '}{1}{'
//@[000:0033) ├─DeclaredVariableExpression { Name = curliesWithNoInterp }
//@[026:0033) | └─StringLiteralExpression { Value = }{1}{ }
var interp1 = 'abc${123}def'
//@[000:0028) ├─DeclaredVariableExpression { Name = interp1 }
//@[014:0028) | └─InterpolatedStringExpression
//@[020:0023) |   └─IntegerLiteralExpression { Value = 123 }
var interp2 = '${123}def'
//@[000:0025) ├─DeclaredVariableExpression { Name = interp2 }
//@[014:0025) | └─InterpolatedStringExpression
//@[017:0020) |   └─IntegerLiteralExpression { Value = 123 }
var interp3 = 'abc${123}'
//@[000:0025) ├─DeclaredVariableExpression { Name = interp3 }
//@[014:0025) | └─InterpolatedStringExpression
//@[020:0023) |   └─IntegerLiteralExpression { Value = 123 }
var interp4 = 'abc${123}${456}jk$l${789}p$'
//@[000:0043) ├─DeclaredVariableExpression { Name = interp4 }
//@[014:0043) | └─InterpolatedStringExpression
//@[020:0023) |   ├─IntegerLiteralExpression { Value = 123 }
//@[026:0029) |   ├─IntegerLiteralExpression { Value = 456 }
//@[036:0039) |   └─IntegerLiteralExpression { Value = 789 }
var doubleInterp = 'abc${'def${123}'}_${'${456}${789}'}'
//@[000:0056) ├─DeclaredVariableExpression { Name = doubleInterp }
//@[019:0056) | └─InterpolatedStringExpression
//@[025:0036) |   ├─InterpolatedStringExpression
//@[031:0034) |   | └─IntegerLiteralExpression { Value = 123 }
//@[040:0054) |   └─InterpolatedStringExpression
//@[043:0046) |     ├─IntegerLiteralExpression { Value = 456 }
//@[049:0052) |     └─IntegerLiteralExpression { Value = 789 }
var curliesInInterp = '{${123}{0}${true}}'
//@[000:0042) ├─DeclaredVariableExpression { Name = curliesInInterp }
//@[022:0042) | └─InterpolatedStringExpression
//@[026:0029) |   ├─IntegerLiteralExpression { Value = 123 }
//@[035:0039) |   └─BooleanLiteralExpression { Value = True }

// #completionTest(0) -> declarations

// verify correct bracket escaping
var bracketInTheMiddle = 'a[b]'
//@[000:0031) ├─DeclaredVariableExpression { Name = bracketInTheMiddle }
//@[025:0031) | └─StringLiteralExpression { Value = a[b] }
// #completionTest(25) -> empty
var bracketAtBeginning = '[test'
//@[000:0032) ├─DeclaredVariableExpression { Name = bracketAtBeginning }
//@[025:0032) | └─StringLiteralExpression { Value = [test }
// #completionTest(23) -> symbolsPlusTypes
var enclosingBrackets = '[test]'
//@[000:0032) ├─DeclaredVariableExpression { Name = enclosingBrackets }
//@[024:0032) | └─StringLiteralExpression { Value = [test] }
var emptyJsonArray = '[]'
//@[000:0025) ├─DeclaredVariableExpression { Name = emptyJsonArray }
//@[021:0025) | └─StringLiteralExpression { Value = [] }
var interpolatedBrackets = '[${myInt}]'
//@[000:0039) ├─DeclaredVariableExpression { Name = interpolatedBrackets }
//@[027:0039) | └─InterpolatedStringExpression
//@[031:0036) |   └─VariableReferenceExpression { Variable = myInt }
var nestedBrackets = '[test[]test2]'
//@[000:0036) ├─DeclaredVariableExpression { Name = nestedBrackets }
//@[021:0036) | └─StringLiteralExpression { Value = [test[]test2] }
var nestedInterpolatedBrackets = '[${emptyJsonArray}]'
//@[000:0054) ├─DeclaredVariableExpression { Name = nestedInterpolatedBrackets }
//@[033:0054) | └─InterpolatedStringExpression
//@[037:0051) |   └─VariableReferenceExpression { Variable = emptyJsonArray }
var bracketStringInExpression = concat('[', '\'test\'',']')
//@[000:0059) ├─DeclaredVariableExpression { Name = bracketStringInExpression }
//@[032:0059) | └─FunctionCallExpression { Name = concat }
//@[039:0042) |   ├─StringLiteralExpression { Value = [ }
//@[044:0054) |   ├─StringLiteralExpression { Value = 'test' }
//@[055:0058) |   └─StringLiteralExpression { Value = ] }

// booleans
@sys.description('a bool variable')
//@[000:0054) ├─DeclaredVariableExpression { Name = myTruth }
//@[017:0034) | ├─StringLiteralExpression { Value = a bool variable }
var myTruth = true
//@[014:0018) | └─BooleanLiteralExpression { Value = True }
var myFalsehood = false
//@[000:0023) ├─DeclaredVariableExpression { Name = myFalsehood }
//@[018:0023) | └─BooleanLiteralExpression { Value = False }

var myEmptyObj = { }
//@[000:0020) ├─DeclaredVariableExpression { Name = myEmptyObj }
//@[017:0020) | └─ObjectExpression
var myEmptyArray = [ ]
//@[000:0022) ├─DeclaredVariableExpression { Name = myEmptyArray }
//@[019:0022) | └─ArrayExpression

// object
@sys.description('a object variable')
//@[000:0242) ├─DeclaredVariableExpression { Name = myObj }
//@[017:0036) | ├─StringLiteralExpression { Value = a object variable }
var myObj = {
//@[012:0204) | └─ObjectExpression
  a: 'a'
//@[002:0008) |   ├─ObjectPropertyExpression
//@[002:0003) |   | ├─StringLiteralExpression { Value = a }
//@[005:0008) |   | └─StringLiteralExpression { Value = a }
  b: -12
//@[002:0008) |   ├─ObjectPropertyExpression
//@[002:0003) |   | ├─StringLiteralExpression { Value = b }
//@[005:0008) |   | └─IntegerLiteralExpression { Value = -12 }
  c: true
//@[002:0009) |   ├─ObjectPropertyExpression
//@[002:0003) |   | ├─StringLiteralExpression { Value = c }
//@[005:0009) |   | └─BooleanLiteralExpression { Value = True }
  d: !true
//@[002:0010) |   ├─ObjectPropertyExpression
//@[002:0003) |   | ├─StringLiteralExpression { Value = d }
//@[005:0010) |   | └─UnaryExpression { Operator = Not }
//@[006:0010) |   |   └─BooleanLiteralExpression { Value = True }
  list: [
//@[002:0102) |   ├─ObjectPropertyExpression
//@[002:0006) |   | ├─StringLiteralExpression { Value = list }
//@[008:0102) |   | └─ArrayExpression
    1
//@[004:0005) |   |   ├─IntegerLiteralExpression { Value = 1 }
    2
//@[004:0005) |   |   ├─IntegerLiteralExpression { Value = 2 }
    2+1
//@[004:0007) |   |   ├─BinaryExpression { Operator = Add }
//@[004:0005) |   |   | ├─IntegerLiteralExpression { Value = 2 }
//@[006:0007) |   |   | └─IntegerLiteralExpression { Value = 1 }
    {
//@[004:0053) |   |   ├─ObjectExpression
      test: 144 > 33 && true || 99 <= 199
//@[006:0041) |   |   | └─ObjectPropertyExpression
//@[006:0010) |   |   |   ├─StringLiteralExpression { Value = test }
//@[012:0041) |   |   |   └─BinaryExpression { Operator = LogicalOr }
//@[012:0028) |   |   |     ├─BinaryExpression { Operator = LogicalAnd }
//@[012:0020) |   |   |     | ├─BinaryExpression { Operator = GreaterThan }
//@[012:0015) |   |   |     | | ├─IntegerLiteralExpression { Value = 144 }
//@[018:0020) |   |   |     | | └─IntegerLiteralExpression { Value = 33 }
//@[024:0028) |   |   |     | └─BooleanLiteralExpression { Value = True }
//@[032:0041) |   |   |     └─BinaryExpression { Operator = LessThanOrEqual }
//@[032:0034) |   |   |       ├─IntegerLiteralExpression { Value = 99 }
//@[038:0041) |   |   |       └─IntegerLiteralExpression { Value = 199 }
    }
    'a' =~ 'b'
//@[004:0014) |   |   └─BinaryExpression { Operator = EqualsInsensitive }
//@[004:0007) |   |     ├─StringLiteralExpression { Value = a }
//@[011:0014) |   |     └─StringLiteralExpression { Value = b }
  ]
  obj: {
//@[002:0046) |   └─ObjectPropertyExpression
//@[002:0005) |     ├─StringLiteralExpression { Value = obj }
//@[007:0046) |     └─ObjectExpression
    nested: [
//@[004:0033) |       └─ObjectPropertyExpression
//@[004:0010) |         ├─StringLiteralExpression { Value = nested }
//@[012:0033) |         └─ArrayExpression
      'hello'
//@[006:0013) |           └─StringLiteralExpression { Value = hello }
    ]
  }
}

@sys.description('a object with interp')
//@[000:0157) ├─DeclaredVariableExpression { Name = objWithInterp }
//@[017:0039) | ├─StringLiteralExpression { Value = a object with interp }
var objWithInterp = {
//@[020:0116) | └─ObjectExpression
  '${myStr}': 1
//@[002:0015) |   ├─ObjectPropertyExpression
//@[002:0012) |   | ├─InterpolatedStringExpression
//@[005:0010) |   | | └─VariableReferenceExpression { Variable = myStr }
//@[014:0015) |   | └─IntegerLiteralExpression { Value = 1 }
  'abc${myStr}def': 2
//@[002:0021) |   ├─ObjectPropertyExpression
//@[002:0018) |   | ├─InterpolatedStringExpression
//@[008:0013) |   | | └─VariableReferenceExpression { Variable = myStr }
//@[020:0021) |   | └─IntegerLiteralExpression { Value = 2 }
  '${interp1}abc${interp2}': '${interp1}abc${interp2}'
//@[002:0054) |   └─ObjectPropertyExpression
//@[002:0027) |     ├─InterpolatedStringExpression
//@[005:0012) |     | ├─VariableReferenceExpression { Variable = interp1 }
//@[018:0025) |     | └─VariableReferenceExpression { Variable = interp2 }
//@[029:0054) |     └─InterpolatedStringExpression
//@[032:0039) |       ├─VariableReferenceExpression { Variable = interp1 }
//@[045:0052) |       └─VariableReferenceExpression { Variable = interp2 }
}

// array
var myArr = [
//@[000:0043) ├─DeclaredVariableExpression { Name = myArr }
//@[012:0043) | └─ArrayExpression
  'pirates'
//@[002:0011) |   ├─StringLiteralExpression { Value = pirates }
  'say'
//@[002:0007) |   ├─StringLiteralExpression { Value = say }
  'arr'
//@[002:0007) |   └─StringLiteralExpression { Value = arr }
]

// array with objects
var myArrWithObjects = [
//@[000:0138) ├─DeclaredVariableExpression { Name = myArrWithObjects }
//@[023:0138) | └─ArrayExpression
  {
//@[002:0040) |   ├─ObjectExpression
    name: 'one'
//@[004:0015) |   | ├─ObjectPropertyExpression
//@[004:0008) |   | | ├─StringLiteralExpression { Value = name }
//@[010:0015) |   | | └─StringLiteralExpression { Value = one }
    enable: true
//@[004:0016) |   | └─ObjectPropertyExpression
//@[004:0010) |   |   ├─StringLiteralExpression { Value = enable }
//@[012:0016) |   |   └─BooleanLiteralExpression { Value = True }
  }
  {
//@[002:0070) |   └─ObjectExpression
    name: 'two'
//@[004:0015) |     ├─ObjectPropertyExpression
//@[004:0008) |     | ├─StringLiteralExpression { Value = name }
//@[010:0015) |     | └─StringLiteralExpression { Value = two }
    enable: false && false || 'two' !~ 'three'
//@[004:0046) |     └─ObjectPropertyExpression
//@[004:0010) |       ├─StringLiteralExpression { Value = enable }
//@[012:0046) |       └─BinaryExpression { Operator = LogicalOr }
//@[012:0026) |         ├─BinaryExpression { Operator = LogicalAnd }
//@[012:0017) |         | ├─BooleanLiteralExpression { Value = False }
//@[021:0026) |         | └─BooleanLiteralExpression { Value = False }
//@[030:0046) |         └─BinaryExpression { Operator = NotEqualsInsensitive }
//@[030:0035) |           ├─StringLiteralExpression { Value = two }
//@[039:0046) |           └─StringLiteralExpression { Value = three }
  }
]

var expressionIndexOnAny = any({
//@[000:0064) ├─DeclaredVariableExpression { Name = expressionIndexOnAny }
//@[027:0064) | └─ArrayAccessExpression
//@[031:0034) |   └─ObjectExpression
})[az.resourceGroup().location]
//@[003:0030) |   ├─PropertyAccessExpression { PropertyName = location }
//@[003:0021) |   | └─FunctionCallExpression { Name = resourceGroup }

var anyIndexOnAny = any(true)[any(false)]
//@[000:0041) ├─DeclaredVariableExpression { Name = anyIndexOnAny }
//@[020:0041) | └─ArrayAccessExpression
//@[034:0039) |   ├─BooleanLiteralExpression { Value = False }
//@[024:0028) |   └─BooleanLiteralExpression { Value = True }

var deploymentName = deployment().name
//@[000:0038) ├─DeclaredVariableExpression { Name = deploymentName }
//@[021:0038) | └─PropertyAccessExpression { PropertyName = name }
//@[021:0033) |   └─FunctionCallExpression { Name = deployment }
var templateContentVersion = deployment().properties.template.contentVersion
//@[000:0076) ├─DeclaredVariableExpression { Name = templateContentVersion }
//@[029:0076) | └─PropertyAccessExpression { PropertyName = contentVersion }
//@[029:0061) |   └─PropertyAccessExpression { PropertyName = template }
//@[029:0052) |     └─PropertyAccessExpression { PropertyName = properties }
//@[029:0041) |       └─FunctionCallExpression { Name = deployment }
var templateLinkUri = deployment().properties.templateLink.uri
//@[000:0062) ├─DeclaredVariableExpression { Name = templateLinkUri }
//@[022:0062) | └─PropertyAccessExpression { PropertyName = uri }
//@[022:0058) |   └─PropertyAccessExpression { PropertyName = templateLink }
//@[022:0045) |     └─PropertyAccessExpression { PropertyName = properties }
//@[022:0034) |       └─FunctionCallExpression { Name = deployment }
var templateLinkId = deployment().properties.templateLink.id
//@[000:0060) ├─DeclaredVariableExpression { Name = templateLinkId }
//@[021:0060) | └─PropertyAccessExpression { PropertyName = id }
//@[021:0057) |   └─PropertyAccessExpression { PropertyName = templateLink }
//@[021:0044) |     └─PropertyAccessExpression { PropertyName = properties }
//@[021:0033) |       └─FunctionCallExpression { Name = deployment }

var portalEndpoint = environment().portal
//@[000:0041) ├─DeclaredVariableExpression { Name = portalEndpoint }
//@[021:0041) | └─PropertyAccessExpression { PropertyName = portal }
//@[021:0034) |   └─FunctionCallExpression { Name = environment }
var loginEndpoint = environment().authentication.loginEndpoint
//@[000:0062) ├─DeclaredVariableExpression { Name = loginEndpoint }
//@[020:0062) | └─PropertyAccessExpression { PropertyName = loginEndpoint }
//@[020:0048) |   └─PropertyAccessExpression { PropertyName = authentication }
//@[020:0033) |     └─FunctionCallExpression { Name = environment }

var namedPropertyIndexer = {
//@[000:0048) ├─DeclaredVariableExpression { Name = namedPropertyIndexer }
//@[027:0048) | └─PropertyAccessExpression { PropertyName = foo }
//@[027:0041) |   └─ObjectExpression
  foo: 's'
//@[002:0010) |     └─ObjectPropertyExpression
//@[002:0005) |       ├─StringLiteralExpression { Value = foo }
//@[007:0010) |       └─StringLiteralExpression { Value = s }
}['foo']

var intIndexer = [
//@[000:0029) ├─DeclaredVariableExpression { Name = intIndexer }
//@[017:0029) | └─ArrayAccessExpression
//@[017:0026) |   └─ArrayExpression
  's'
//@[002:0005) |     └─StringLiteralExpression { Value = s }
][0]
//@[002:0003) |   ├─IntegerLiteralExpression { Value = 0 }

var functionOnIndexer1 = concat([
//@[000:0050) ├─DeclaredVariableExpression { Name = functionOnIndexer1 }
//@[025:0050) | └─FunctionCallExpression { Name = concat }
//@[032:0044) |   ├─ArrayAccessExpression
//@[032:0041) |   | └─ArrayExpression
  's'
//@[002:0005) |   |   └─StringLiteralExpression { Value = s }
][0], 's')
//@[002:0003) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[006:0009) |   └─StringLiteralExpression { Value = s }

var singleQuote = '\''
//@[000:0022) ├─DeclaredVariableExpression { Name = singleQuote }
//@[018:0022) | └─StringLiteralExpression { Value = ' }
var myPropertyName = '${singleQuote}foo${singleQuote}'
//@[000:0054) ├─DeclaredVariableExpression { Name = myPropertyName }
//@[021:0054) | └─InterpolatedStringExpression
//@[024:0035) |   ├─VariableReferenceExpression { Variable = singleQuote }
//@[041:0052) |   └─VariableReferenceExpression { Variable = singleQuote }

var unusedIntermediate = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01')
var unusedIntermediateRef = unusedIntermediate.secondaryKey

// previously this was not possible to emit correctly
var previousEmitLimit = [
//@[000:0299) ├─DeclaredVariableExpression { Name = previousEmitLimit }
//@[024:0299) | └─ArrayExpression
  concat('s')
//@[002:0013) |   ├─FunctionCallExpression { Name = concat }
//@[009:0012) |   | └─StringLiteralExpression { Value = s }
  '${4}'
//@[002:0008) |   ├─InterpolatedStringExpression
//@[005:0006) |   | └─IntegerLiteralExpression { Value = 4 }
  {
//@[002:0248) |   └─ObjectExpression
    a: {
//@[004:0240) |     └─ObjectPropertyExpression
//@[004:0005) |       ├─StringLiteralExpression { Value = a }
//@[007:0240) |       └─ObjectExpression
      b: base64('s')
//@[006:0020) |         ├─ObjectPropertyExpression
//@[006:0007) |         | ├─StringLiteralExpression { Value = b }
//@[009:0020) |         | └─FunctionCallExpression { Name = base64 }
//@[016:0019) |         |   └─StringLiteralExpression { Value = s }
      c: concat([
//@[006:0082) |         ├─ObjectPropertyExpression
//@[006:0007) |         | ├─StringLiteralExpression { Value = c }
//@[009:0082) |         | └─FunctionCallExpression { Name = concat }
//@[016:0040) |         |   ├─ArrayExpression
        12 + 3
//@[008:0014) |         |   | └─BinaryExpression { Operator = Add }
//@[008:0010) |         |   |   ├─IntegerLiteralExpression { Value = 12 }
//@[013:0014) |         |   |   └─IntegerLiteralExpression { Value = 3 }
      ], [
//@[009:0048) |         |   └─ArrayExpression
        !true
//@[008:0013) |         |     ├─UnaryExpression { Operator = Not }
//@[009:0013) |         |     | └─BooleanLiteralExpression { Value = True }
        'hello'
//@[008:0015) |         |     └─StringLiteralExpression { Value = hello }
      ])
      d: az.resourceGroup().location
//@[006:0036) |         ├─ObjectPropertyExpression
//@[006:0007) |         | ├─StringLiteralExpression { Value = d }
//@[009:0036) |         | └─PropertyAccessExpression { PropertyName = location }
//@[009:0027) |         |   └─FunctionCallExpression { Name = resourceGroup }
      e: concat([
//@[006:0039) |         ├─ObjectPropertyExpression
//@[006:0007) |         | ├─StringLiteralExpression { Value = e }
//@[009:0039) |         | └─FunctionCallExpression { Name = concat }
//@[016:0038) |         |   └─ArrayExpression
        true
//@[008:0012) |         |     └─BooleanLiteralExpression { Value = True }
      ])
      f: concat([
//@[006:0044) |         └─ObjectPropertyExpression
//@[006:0007) |           ├─StringLiteralExpression { Value = f }
//@[009:0044) |           └─FunctionCallExpression { Name = concat }
//@[016:0043) |             └─ArrayExpression
        's' == 12
//@[008:0017) |               └─BinaryExpression { Operator = Equals }
//@[008:0011) |                 ├─StringLiteralExpression { Value = s }
//@[015:0017) |                 └─IntegerLiteralExpression { Value = 12 }
      ])
    }
  }
]

// previously this was not possible to emit correctly
var previousEmitLimit2 = [
//@[000:0327) ├─DeclaredVariableExpression { Name = previousEmitLimit2 }
//@[025:0327) | └─ArrayExpression
  concat('s')
//@[002:0013) |   ├─FunctionCallExpression { Name = concat }
//@[009:0012) |   | └─StringLiteralExpression { Value = s }
  '${4}'
//@[002:0008) |   ├─InterpolatedStringExpression
//@[005:0006) |   | └─IntegerLiteralExpression { Value = 4 }
  {
//@[002:0275) |   └─ObjectExpression
    a: {
//@[004:0267) |     └─ObjectPropertyExpression
//@[004:0005) |       ├─StringLiteralExpression { Value = a }
//@[007:0267) |       └─ObjectExpression
      b: base64('s')
//@[006:0020) |         ├─ObjectPropertyExpression
//@[006:0007) |         | ├─StringLiteralExpression { Value = b }
//@[009:0020) |         | └─FunctionCallExpression { Name = base64 }
//@[016:0019) |         |   └─StringLiteralExpression { Value = s }
      c: union({
//@[006:0090) |         ├─ObjectPropertyExpression
//@[006:0007) |         | ├─StringLiteralExpression { Value = c }
//@[009:0090) |         | └─FunctionCallExpression { Name = union }
//@[015:0042) |         |   ├─ObjectExpression
        a: 12 + 3
//@[008:0017) |         |   | └─ObjectPropertyExpression
//@[008:0009) |         |   |   ├─StringLiteralExpression { Value = a }
//@[011:0017) |         |   |   └─BinaryExpression { Operator = Add }
//@[011:0013) |         |   |     ├─IntegerLiteralExpression { Value = 12 }
//@[016:0017) |         |   |     └─IntegerLiteralExpression { Value = 3 }
      }, {
//@[009:0054) |         |   └─ObjectExpression
        b: !true
//@[008:0016) |         |     ├─ObjectPropertyExpression
//@[008:0009) |         |     | ├─StringLiteralExpression { Value = b }
//@[011:0016) |         |     | └─UnaryExpression { Operator = Not }
//@[012:0016) |         |     |   └─BooleanLiteralExpression { Value = True }
        c: 'hello'
//@[008:0018) |         |     └─ObjectPropertyExpression
//@[008:0009) |         |       ├─StringLiteralExpression { Value = c }
//@[011:0018) |         |       └─StringLiteralExpression { Value = hello }
      })
      d: az.resourceGroup().location
//@[006:0036) |         ├─ObjectPropertyExpression
//@[006:0007) |         | ├─StringLiteralExpression { Value = d }
//@[009:0036) |         | └─PropertyAccessExpression { PropertyName = location }
//@[009:0027) |         |   └─FunctionCallExpression { Name = resourceGroup }
      e: union({
//@[006:0045) |         ├─ObjectPropertyExpression
//@[006:0007) |         | ├─StringLiteralExpression { Value = e }
//@[009:0045) |         | └─FunctionCallExpression { Name = union }
//@[015:0040) |         |   ├─ObjectExpression
        x: true
//@[008:0015) |         |   | └─ObjectPropertyExpression
//@[008:0009) |         |   |   ├─StringLiteralExpression { Value = x }
//@[011:0015) |         |   |   └─BooleanLiteralExpression { Value = True }
      }, {})
//@[009:0011) |         |   └─ObjectExpression
      f: intersection({
//@[006:0057) |         └─ObjectPropertyExpression
//@[006:0007) |           ├─StringLiteralExpression { Value = f }
//@[009:0057) |           └─FunctionCallExpression { Name = intersection }
//@[022:0052) |             ├─ObjectExpression
        q: 's' == 12
//@[008:0020) |             | └─ObjectPropertyExpression
//@[008:0009) |             |   ├─StringLiteralExpression { Value = q }
//@[011:0020) |             |   └─BinaryExpression { Operator = Equals }
//@[011:0014) |             |     ├─StringLiteralExpression { Value = s }
//@[018:0020) |             |     └─IntegerLiteralExpression { Value = 12 }
      }, {})
//@[009:0011) |             └─ObjectExpression
    }
  }
]

// previously this was not possible to emit correctly
var previousEmitLimit3 = {
//@[000:0140) ├─DeclaredVariableExpression { Name = previousEmitLimit3 }
//@[025:0140) | └─ObjectExpression
  a: {
//@[002:0111) |   └─ObjectPropertyExpression
//@[002:0003) |     ├─StringLiteralExpression { Value = a }
//@[005:0111) |     └─ObjectExpression
    b: {
//@[004:0056) |       ├─ObjectPropertyExpression
//@[004:0005) |       | ├─StringLiteralExpression { Value = b }
//@[007:0056) |       | └─BinaryExpression { Operator = Equals }
//@[007:0051) |       |   ├─ObjectExpression
      a: az.resourceGroup().location
//@[006:0036) |       |   | └─ObjectPropertyExpression
//@[006:0007) |       |   |   ├─StringLiteralExpression { Value = a }
//@[009:0036) |       |   |   └─PropertyAccessExpression { PropertyName = location }
//@[009:0027) |       |   |     └─FunctionCallExpression { Name = resourceGroup }
    } == 2
//@[009:0010) |       |   └─IntegerLiteralExpression { Value = 2 }
    c: concat([
//@[004:0043) |       └─ObjectPropertyExpression
//@[004:0005) |         ├─StringLiteralExpression { Value = c }
//@[007:0043) |         └─FunctionCallExpression { Name = concat }
//@[014:0022) |           ├─ArrayExpression

    ], [
//@[007:0025) |           └─ArrayExpression
      true
//@[006:0010) |             └─BooleanLiteralExpression { Value = True }
    ])
  }
}

// #completionTest(0) -> declarations

var myVar = 'hello'
//@[000:0019) ├─DeclaredVariableExpression { Name = myVar }
//@[012:0019) | └─StringLiteralExpression { Value = hello }
var myVar2 = any({
//@[000:0040) ├─DeclaredVariableExpression { Name = myVar2 }
//@[017:0039) | └─ObjectExpression
  something: myVar
//@[002:0018) |   └─ObjectPropertyExpression
//@[002:0011) |     ├─StringLiteralExpression { Value = something }
//@[013:0018) |     └─VariableReferenceExpression { Variable = myVar }
})
var myVar3 = any(any({
//@[000:0045) ├─DeclaredVariableExpression { Name = myVar3 }
//@[021:0043) | └─ObjectExpression
  something: myVar
//@[002:0018) |   └─ObjectPropertyExpression
//@[002:0011) |     ├─StringLiteralExpression { Value = something }
//@[013:0018) |     └─VariableReferenceExpression { Variable = myVar }
}))
var myVar4 = length(any(concat('s','a')))
//@[000:0041) ├─DeclaredVariableExpression { Name = myVar4 }
//@[013:0041) | └─FunctionCallExpression { Name = length }
//@[024:0039) |   └─FunctionCallExpression { Name = concat }
//@[031:0034) |     ├─StringLiteralExpression { Value = s }
//@[035:0038) |     └─StringLiteralExpression { Value = a }

// verify that unqualified banned function identifiers can be used as declaration identifiers
var variables = true
//@[000:0020) ├─DeclaredVariableExpression { Name = variables }
//@[016:0020) | └─BooleanLiteralExpression { Value = True }
param parameters bool = true
//@[000:0028) ├─DeclaredParameterExpression { Name = parameters }
//@[017:0021) | ├─AmbientTypeReferenceExpression { Name = bool }
//@[024:0028) | └─BooleanLiteralExpression { Value = True }
var if = true
//@[000:0013) ├─DeclaredVariableExpression { Name = if }
//@[009:0013) | └─BooleanLiteralExpression { Value = True }
var createArray = true
//@[000:0022) ├─DeclaredVariableExpression { Name = createArray }
//@[018:0022) | └─BooleanLiteralExpression { Value = True }
var createObject = true
//@[000:0023) ├─DeclaredVariableExpression { Name = createObject }
//@[019:0023) | └─BooleanLiteralExpression { Value = True }
var add = true
//@[000:0014) ├─DeclaredVariableExpression { Name = add }
//@[010:0014) | └─BooleanLiteralExpression { Value = True }
var sub = true
//@[000:0014) ├─DeclaredVariableExpression { Name = sub }
//@[010:0014) | └─BooleanLiteralExpression { Value = True }
var mul = true
//@[000:0014) ├─DeclaredVariableExpression { Name = mul }
//@[010:0014) | └─BooleanLiteralExpression { Value = True }
var div = true
//@[000:0014) ├─DeclaredVariableExpression { Name = div }
//@[010:0014) | └─BooleanLiteralExpression { Value = True }
param mod bool = true
//@[000:0021) ├─DeclaredParameterExpression { Name = mod }
//@[010:0014) | ├─AmbientTypeReferenceExpression { Name = bool }
//@[017:0021) | └─BooleanLiteralExpression { Value = True }
var less = true
//@[000:0015) ├─DeclaredVariableExpression { Name = less }
//@[011:0015) | └─BooleanLiteralExpression { Value = True }
var lessOrEquals = true
//@[000:0023) ├─DeclaredVariableExpression { Name = lessOrEquals }
//@[019:0023) | └─BooleanLiteralExpression { Value = True }
var greater = true
//@[000:0018) ├─DeclaredVariableExpression { Name = greater }
//@[014:0018) | └─BooleanLiteralExpression { Value = True }
var greaterOrEquals = true
//@[000:0026) ├─DeclaredVariableExpression { Name = greaterOrEquals }
//@[022:0026) | └─BooleanLiteralExpression { Value = True }
param equals bool = true
//@[000:0024) ├─DeclaredParameterExpression { Name = equals }
//@[013:0017) | ├─AmbientTypeReferenceExpression { Name = bool }
//@[020:0024) | └─BooleanLiteralExpression { Value = True }
var not = true
//@[000:0014) ├─DeclaredVariableExpression { Name = not }
//@[010:0014) | └─BooleanLiteralExpression { Value = True }
var and = true
//@[000:0014) ├─DeclaredVariableExpression { Name = and }
//@[010:0014) | └─BooleanLiteralExpression { Value = True }
var or = true
//@[000:0013) ├─DeclaredVariableExpression { Name = or }
//@[009:0013) | └─BooleanLiteralExpression { Value = True }
var I_WANT_IT_ALL = variables && parameters && if && createArray && createObject && add && sub && mul && div && mod && less && lessOrEquals && greater && greaterOrEquals && equals && not && and && or
//@[000:0199) ├─DeclaredVariableExpression { Name = I_WANT_IT_ALL }
//@[020:0199) | └─BinaryExpression { Operator = LogicalAnd }
//@[020:0193) |   ├─BinaryExpression { Operator = LogicalAnd }
//@[020:0186) |   | ├─BinaryExpression { Operator = LogicalAnd }
//@[020:0179) |   | | ├─BinaryExpression { Operator = LogicalAnd }
//@[020:0169) |   | | | ├─BinaryExpression { Operator = LogicalAnd }
//@[020:0150) |   | | | | ├─BinaryExpression { Operator = LogicalAnd }
//@[020:0139) |   | | | | | ├─BinaryExpression { Operator = LogicalAnd }
//@[020:0123) |   | | | | | | ├─BinaryExpression { Operator = LogicalAnd }
//@[020:0115) |   | | | | | | | ├─BinaryExpression { Operator = LogicalAnd }
//@[020:0108) |   | | | | | | | | ├─BinaryExpression { Operator = LogicalAnd }
//@[020:0101) |   | | | | | | | | | ├─BinaryExpression { Operator = LogicalAnd }
//@[020:0094) |   | | | | | | | | | | ├─BinaryExpression { Operator = LogicalAnd }
//@[020:0087) |   | | | | | | | | | | | ├─BinaryExpression { Operator = LogicalAnd }
//@[020:0080) |   | | | | | | | | | | | | ├─BinaryExpression { Operator = LogicalAnd }
//@[020:0064) |   | | | | | | | | | | | | | ├─BinaryExpression { Operator = LogicalAnd }
//@[020:0049) |   | | | | | | | | | | | | | | ├─BinaryExpression { Operator = LogicalAnd }
//@[020:0043) |   | | | | | | | | | | | | | | | ├─BinaryExpression { Operator = LogicalAnd }
//@[020:0029) |   | | | | | | | | | | | | | | | | ├─VariableReferenceExpression { Variable = variables }
//@[033:0043) |   | | | | | | | | | | | | | | | | └─ParametersReferenceExpression { Parameter = parameters }
//@[047:0049) |   | | | | | | | | | | | | | | | └─VariableReferenceExpression { Variable = if }
//@[053:0064) |   | | | | | | | | | | | | | | └─VariableReferenceExpression { Variable = createArray }
//@[068:0080) |   | | | | | | | | | | | | | └─VariableReferenceExpression { Variable = createObject }
//@[084:0087) |   | | | | | | | | | | | | └─VariableReferenceExpression { Variable = add }
//@[091:0094) |   | | | | | | | | | | | └─VariableReferenceExpression { Variable = sub }
//@[098:0101) |   | | | | | | | | | | └─VariableReferenceExpression { Variable = mul }
//@[105:0108) |   | | | | | | | | | └─VariableReferenceExpression { Variable = div }
//@[112:0115) |   | | | | | | | | └─ParametersReferenceExpression { Parameter = mod }
//@[119:0123) |   | | | | | | | └─VariableReferenceExpression { Variable = less }
//@[127:0139) |   | | | | | | └─VariableReferenceExpression { Variable = lessOrEquals }
//@[143:0150) |   | | | | | └─VariableReferenceExpression { Variable = greater }
//@[154:0169) |   | | | | └─VariableReferenceExpression { Variable = greaterOrEquals }
//@[173:0179) |   | | | └─ParametersReferenceExpression { Parameter = equals }
//@[183:0186) |   | | └─VariableReferenceExpression { Variable = not }
//@[190:0193) |   | └─VariableReferenceExpression { Variable = and }
//@[197:0199) |   └─VariableReferenceExpression { Variable = or }

// identifiers can have underscores
var _ = 3
//@[000:0009) ├─DeclaredVariableExpression { Name = _ }
//@[008:0009) | └─IntegerLiteralExpression { Value = 3 }
var __ = 10 * _
//@[000:0015) ├─DeclaredVariableExpression { Name = __ }
//@[009:0015) | └─BinaryExpression { Operator = Multiply }
//@[009:0011) |   ├─IntegerLiteralExpression { Value = 10 }
//@[014:0015) |   └─VariableReferenceExpression { Variable = _ }
var _0a_1b = true
//@[000:0017) ├─DeclaredVariableExpression { Name = _0a_1b }
//@[013:0017) | └─BooleanLiteralExpression { Value = True }
var _1_ = _0a_1b || (__ + _ % 2 == 0)
//@[000:0037) ├─DeclaredVariableExpression { Name = _1_ }
//@[010:0037) | └─BinaryExpression { Operator = LogicalOr }
//@[010:0016) |   ├─VariableReferenceExpression { Variable = _0a_1b }
//@[021:0036) |   └─BinaryExpression { Operator = Equals }
//@[021:0031) |     ├─BinaryExpression { Operator = Add }
//@[021:0023) |     | ├─VariableReferenceExpression { Variable = __ }
//@[026:0031) |     | └─BinaryExpression { Operator = Modulo }
//@[026:0027) |     |   ├─VariableReferenceExpression { Variable = _ }
//@[030:0031) |     |   └─IntegerLiteralExpression { Value = 2 }
//@[035:0036) |     └─IntegerLiteralExpression { Value = 0 }

// fully qualified access
var resourceGroup = 'something'
//@[000:0031) ├─DeclaredVariableExpression { Name = resourceGroup }
//@[020:0031) | └─StringLiteralExpression { Value = something }
var resourceGroupName = az.resourceGroup().name
//@[000:0047) ├─DeclaredVariableExpression { Name = resourceGroupName }
//@[024:0047) | └─PropertyAccessExpression { PropertyName = name }
//@[024:0042) |   └─FunctionCallExpression { Name = resourceGroup }
var resourceGroupObject = az.resourceGroup()
//@[000:0044) ├─DeclaredVariableExpression { Name = resourceGroupObject }
//@[026:0044) | └─FunctionCallExpression { Name = resourceGroup }
var propertyAccessFromObject = resourceGroupObject.name
//@[000:0055) ├─DeclaredVariableExpression { Name = propertyAccessFromObject }
//@[031:0055) | └─PropertyAccessExpression { PropertyName = name }
//@[031:0050) |   └─VariableReferenceExpression { Variable = resourceGroupObject }
var isTrue = sys.max(1, 2) == 3
//@[000:0031) ├─DeclaredVariableExpression { Name = isTrue }
//@[013:0031) | └─BinaryExpression { Operator = Equals }
//@[013:0026) |   ├─FunctionCallExpression { Name = max }
//@[021:0022) |   | ├─IntegerLiteralExpression { Value = 1 }
//@[024:0025) |   | └─IntegerLiteralExpression { Value = 2 }
//@[030:0031) |   └─IntegerLiteralExpression { Value = 3 }
var isFalse = !isTrue
//@[000:0021) ├─DeclaredVariableExpression { Name = isFalse }
//@[014:0021) | └─UnaryExpression { Operator = Not }
//@[015:0021) |   └─VariableReferenceExpression { Variable = isTrue }
var someText = isTrue ? sys.concat('a', sys.concat('b', 'c')) : 'someText'
//@[000:0074) ├─DeclaredVariableExpression { Name = someText }
//@[015:0074) | └─TernaryExpression
//@[015:0021) |   ├─VariableReferenceExpression { Variable = isTrue }
//@[024:0061) |   ├─FunctionCallExpression { Name = concat }
//@[035:0038) |   | ├─StringLiteralExpression { Value = a }
//@[040:0060) |   | └─FunctionCallExpression { Name = concat }
//@[051:0054) |   |   ├─StringLiteralExpression { Value = b }
//@[056:0059) |   |   └─StringLiteralExpression { Value = c }
//@[064:0074) |   └─StringLiteralExpression { Value = someText }

// Bicep functions that cannot be converted into ARM functions
var scopesWithoutArmRepresentation = {
//@[000:0195) ├─DeclaredVariableExpression { Name = scopesWithoutArmRepresentation }
//@[037:0195) | └─ObjectExpression
  subscription: subscription('10b57a01-6350-4ce2-972a-6a13642f00bf')
//@[002:0068) |   ├─ObjectPropertyExpression
//@[002:0014) |   | ├─StringLiteralExpression { Value = subscription }
//@[016:0068) |   | └─ObjectExpression
  resourceGroup: az.resourceGroup('10b57a01-6350-4ce2-972a-6a13642f00bf', 'myRgName')
//@[002:0085) |   └─ObjectPropertyExpression
//@[002:0015) |     ├─StringLiteralExpression { Value = resourceGroup }
//@[017:0085) |     └─ObjectExpression
}

var scopesWithArmRepresentation = {
//@[000:0123) ├─DeclaredVariableExpression { Name = scopesWithArmRepresentation }
//@[034:0123) | └─ObjectExpression
  tenant: tenant()
//@[002:0018) |   ├─ObjectPropertyExpression
//@[002:0008) |   | ├─StringLiteralExpression { Value = tenant }
//@[010:0018) |   | └─FunctionCallExpression { Name = tenant }
  subscription: subscription()
//@[002:0030) |   ├─ObjectPropertyExpression
//@[002:0014) |   | ├─StringLiteralExpression { Value = subscription }
//@[016:0030) |   | └─FunctionCallExpression { Name = subscription }
  resourceGroup: az.resourceGroup()
//@[002:0035) |   └─ObjectPropertyExpression
//@[002:0015) |     ├─StringLiteralExpression { Value = resourceGroup }
//@[017:0035) |     └─FunctionCallExpression { Name = resourceGroup }
}

// Issue #1332
var issue1332_propname = 'ptest'
//@[000:0032) ├─DeclaredVariableExpression { Name = issue1332_propname }
//@[025:0032) | └─StringLiteralExpression { Value = ptest }
var issue1332 = true ? {
//@[000:0086) ├─DeclaredVariableExpression { Name = issue1332 }
//@[016:0086) | └─TernaryExpression
//@[016:0020) |   ├─BooleanLiteralExpression { Value = True }
//@[023:0081) |   ├─ObjectExpression
    prop1: {
//@[004:0054) |   | └─ObjectPropertyExpression
//@[004:0009) |   |   ├─StringLiteralExpression { Value = prop1 }
//@[011:0054) |   |   └─ObjectExpression
        '${issue1332_propname}': {}
//@[008:0035) |   |     └─ObjectPropertyExpression
//@[008:0031) |   |       ├─InterpolatedStringExpression
//@[011:0029) |   |       | └─VariableReferenceExpression { Variable = issue1332_propname }
//@[033:0035) |   |       └─ObjectExpression
    }
} : {}
//@[004:0006) |   └─ObjectExpression

// Issue #486
var myBigInt = 2199023255552
//@[000:0028) ├─DeclaredVariableExpression { Name = myBigInt }
//@[015:0028) | └─IntegerLiteralExpression { Value = 2199023255552 }
var myIntExpression = 5 * 5
//@[000:0027) ├─DeclaredVariableExpression { Name = myIntExpression }
//@[022:0027) | └─BinaryExpression { Operator = Multiply }
//@[022:0023) |   ├─IntegerLiteralExpression { Value = 5 }
//@[026:0027) |   └─IntegerLiteralExpression { Value = 5 }
var myBigIntExpression = 2199023255552 * 2
//@[000:0042) ├─DeclaredVariableExpression { Name = myBigIntExpression }
//@[025:0042) | └─BinaryExpression { Operator = Multiply }
//@[025:0038) |   ├─IntegerLiteralExpression { Value = 2199023255552 }
//@[041:0042) |   └─IntegerLiteralExpression { Value = 2 }
var myBigIntExpression2 = 2199023255552 * 2199023255552
//@[000:0055) ├─DeclaredVariableExpression { Name = myBigIntExpression2 }
//@[026:0055) | └─BinaryExpression { Operator = Multiply }
//@[026:0039) |   ├─IntegerLiteralExpression { Value = 2199023255552 }
//@[042:0055) |   └─IntegerLiteralExpression { Value = 2199023255552 }

// variable loops
var incrementingNumbers = [for i in range(0,10) : i]
//@[000:0052) ├─DeclaredVariableExpression { Name = incrementingNumbers }
//@[026:0052) | └─ForLoopExpression
//@[036:0047) |   ├─FunctionCallExpression { Name = range }
//@[042:0043) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[044:0046) |   | └─IntegerLiteralExpression { Value = 10 }
//@[050:0051) |   └─ArrayAccessExpression
//@[050:0051) |     ├─CopyIndexExpression
//@[036:0047) |     └─FunctionCallExpression { Name = range }
//@[042:0043) |       ├─IntegerLiteralExpression { Value = 0 }
//@[044:0046) |       └─IntegerLiteralExpression { Value = 10 }
var printToSingleLine1 = [
//@[000:0057) ├─DeclaredVariableExpression { Name = printToSingleLine1 }
//@[025:0057) | └─ForLoopExpression
    for i in range(0,20) : i
//@[013:0024) |   ├─FunctionCallExpression { Name = range }
//@[019:0020) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[021:0023) |   | └─IntegerLiteralExpression { Value = 20 }
//@[027:0028) |   └─ArrayAccessExpression
//@[027:0028) |     ├─CopyIndexExpression
//@[013:0024) |     └─FunctionCallExpression { Name = range }
//@[019:0020) |       ├─IntegerLiteralExpression { Value = 0 }
//@[021:0023) |       └─IntegerLiteralExpression { Value = 20 }
]
var printToSingleLine2 = [
//@[000:0080) ├─DeclaredVariableExpression { Name = printToSingleLine2 }
//@[025:0080) | └─ForLoopExpression
    /* harmless comment */ for i in range(0,20) : i
//@[036:0047) |   ├─FunctionCallExpression { Name = range }
//@[042:0043) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[044:0046) |   | └─IntegerLiteralExpression { Value = 20 }
//@[050:0051) |   └─ArrayAccessExpression
//@[050:0051) |     ├─CopyIndexExpression
//@[036:0047) |     └─FunctionCallExpression { Name = range }
//@[042:0043) |       ├─IntegerLiteralExpression { Value = 0 }
//@[044:0046) |       └─IntegerLiteralExpression { Value = 20 }
]
var printToSingleLine3 = [
//@[000:0080) ├─DeclaredVariableExpression { Name = printToSingleLine3 }
//@[025:0080) | └─ForLoopExpression
    for i in range(0,20) : i /* harmless comment */
//@[013:0024) |   ├─FunctionCallExpression { Name = range }
//@[019:0020) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[021:0023) |   | └─IntegerLiteralExpression { Value = 20 }
//@[027:0028) |   └─ArrayAccessExpression
//@[027:0028) |     ├─CopyIndexExpression
//@[013:0024) |     └─FunctionCallExpression { Name = range }
//@[019:0020) |       ├─IntegerLiteralExpression { Value = 0 }
//@[021:0023) |       └─IntegerLiteralExpression { Value = 20 }
]
var forceLineBreaks1 = [
//@[000:0084) ├─DeclaredVariableExpression { Name = forceLineBreaks1 }
//@[023:0084) | └─ForLoopExpression
    // force line breaks
    for i in range(0,    30) : i
//@[013:0028) |   ├─FunctionCallExpression { Name = range }
//@[019:0020) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[025:0027) |   | └─IntegerLiteralExpression { Value = 30 }
//@[031:0032) |   └─ArrayAccessExpression
//@[031:0032) |     ├─CopyIndexExpression
//@[013:0028) |     └─FunctionCallExpression { Name = range }
//@[019:0020) |       ├─IntegerLiteralExpression { Value = 0 }
//@[025:0027) |       └─IntegerLiteralExpression { Value = 30 }
]
var forceLineBreaks2 = [
//@[000:0084) ├─DeclaredVariableExpression { Name = forceLineBreaks2 }
//@[023:0084) | └─ForLoopExpression
    for i in range(0,    30) : i
//@[013:0028) |   ├─FunctionCallExpression { Name = range }
//@[019:0020) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[025:0027) |   | └─IntegerLiteralExpression { Value = 30 }
//@[031:0032) |   └─ArrayAccessExpression
//@[031:0032) |     ├─CopyIndexExpression
//@[013:0028) |     └─FunctionCallExpression { Name = range }
//@[019:0020) |       ├─IntegerLiteralExpression { Value = 0 }
//@[025:0027) |       └─IntegerLiteralExpression { Value = 30 }
    // force line breaks
]
var forceLineBreaks3 = [
//@[000:0115) ├─DeclaredVariableExpression { Name = forceLineBreaks3 }
//@[023:0115) | └─ForLoopExpression
    /* force line breaks */
    for i in range(0,    30) : i
//@[013:0028) |   ├─FunctionCallExpression { Name = range }
//@[019:0020) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[025:0027) |   | └─IntegerLiteralExpression { Value = 30 }
//@[031:0032) |   └─ArrayAccessExpression
//@[031:0032) |     ├─CopyIndexExpression
//@[013:0028) |     └─FunctionCallExpression { Name = range }
//@[019:0020) |       ├─IntegerLiteralExpression { Value = 0 }
//@[025:0027) |       └─IntegerLiteralExpression { Value = 30 }
    /* force line breaks */
]

var loopInput = [
//@[000:0035) ├─DeclaredVariableExpression { Name = loopInput }
//@[016:0035) | └─ArrayExpression
  'one'
//@[002:0007) |   ├─StringLiteralExpression { Value = one }
  'two'
//@[002:0007) |   └─StringLiteralExpression { Value = two }
]
var arrayOfStringsViaLoop = [for (name, i) in loopInput: 'prefix-${i}-${name}']
//@[000:0079) ├─DeclaredVariableExpression { Name = arrayOfStringsViaLoop }
//@[028:0079) | └─ForLoopExpression
//@[046:0055) |   ├─VariableReferenceExpression { Variable = loopInput }
//@[057:0078) |   └─InterpolatedStringExpression
//@[067:0068) |     ├─CopyIndexExpression
//@[072:0076) |     └─ArrayAccessExpression
//@[072:0076) |       ├─CopyIndexExpression
//@[046:0055) |       └─VariableReferenceExpression { Variable = loopInput }
var arrayOfObjectsViaLoop = [for (name, i) in loopInput: {
//@[000:0123) ├─DeclaredVariableExpression { Name = arrayOfObjectsViaLoop }
//@[028:0123) | └─ForLoopExpression
//@[046:0055) |   ├─VariableReferenceExpression { Variable = loopInput }
//@[057:0122) |   └─ObjectExpression
//@[046:0055) |     |   └─VariableReferenceExpression { Variable = loopInput }
//@[046:0055) |           └─VariableReferenceExpression { Variable = loopInput }
  index: i
//@[002:0010) |     ├─ObjectPropertyExpression
//@[002:0007) |     | ├─StringLiteralExpression { Value = index }
//@[009:0010) |     | └─CopyIndexExpression
  name: name
//@[002:0012) |     ├─ObjectPropertyExpression
//@[002:0006) |     | ├─StringLiteralExpression { Value = name }
//@[008:0012) |     | └─ArrayAccessExpression
//@[008:0012) |     |   ├─CopyIndexExpression
  value: 'prefix-${i}-${name}-suffix'
//@[002:0037) |     └─ObjectPropertyExpression
//@[002:0007) |       ├─StringLiteralExpression { Value = value }
//@[009:0037) |       └─InterpolatedStringExpression
//@[019:0020) |         ├─CopyIndexExpression
//@[024:0028) |         └─ArrayAccessExpression
//@[024:0028) |           ├─CopyIndexExpression
}]
var arrayOfArraysViaLoop = [for (name, i) in loopInput: [
//@[000:0102) ├─DeclaredVariableExpression { Name = arrayOfArraysViaLoop }
//@[027:0102) | └─ForLoopExpression
//@[045:0054) |   ├─VariableReferenceExpression { Variable = loopInput }
//@[056:0101) |   └─ArrayExpression
//@[045:0054) |     | └─VariableReferenceExpression { Variable = loopInput }
//@[045:0054) |         └─VariableReferenceExpression { Variable = loopInput }
  i
//@[002:0003) |     ├─CopyIndexExpression
  name
//@[002:0006) |     ├─ArrayAccessExpression
//@[002:0006) |     | ├─CopyIndexExpression
  'prefix-${i}-${name}-suffix'
//@[002:0030) |     └─InterpolatedStringExpression
//@[012:0013) |       ├─CopyIndexExpression
//@[017:0021) |       └─ArrayAccessExpression
//@[017:0021) |         ├─CopyIndexExpression
]]
var arrayOfBooleans = [for (name, i) in loopInput: i % 2 == 0]
//@[000:0062) ├─DeclaredVariableExpression { Name = arrayOfBooleans }
//@[022:0062) | └─ForLoopExpression
//@[040:0049) |   ├─VariableReferenceExpression { Variable = loopInput }
//@[051:0061) |   └─BinaryExpression { Operator = Equals }
//@[051:0056) |     ├─BinaryExpression { Operator = Modulo }
//@[051:0052) |     | ├─CopyIndexExpression
//@[055:0056) |     | └─IntegerLiteralExpression { Value = 2 }
//@[060:0061) |     └─IntegerLiteralExpression { Value = 0 }
var arrayOfHardCodedNumbers = [for i in range(0,10): 3]
//@[000:0055) ├─DeclaredVariableExpression { Name = arrayOfHardCodedNumbers }
//@[030:0055) | └─ForLoopExpression
//@[040:0051) |   ├─FunctionCallExpression { Name = range }
//@[046:0047) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[048:0050) |   | └─IntegerLiteralExpression { Value = 10 }
//@[053:0054) |   └─IntegerLiteralExpression { Value = 3 }
var arrayOfHardCodedBools = [for i in range(0,10): false]
//@[000:0057) ├─DeclaredVariableExpression { Name = arrayOfHardCodedBools }
//@[028:0057) | └─ForLoopExpression
//@[038:0049) |   ├─FunctionCallExpression { Name = range }
//@[044:0045) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[046:0048) |   | └─IntegerLiteralExpression { Value = 10 }
//@[051:0056) |   └─BooleanLiteralExpression { Value = False }
var arrayOfHardCodedStrings = [for i in range(0,3): 'hi']
//@[000:0057) ├─DeclaredVariableExpression { Name = arrayOfHardCodedStrings }
//@[030:0057) | └─ForLoopExpression
//@[040:0050) |   ├─FunctionCallExpression { Name = range }
//@[046:0047) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[048:0049) |   | └─IntegerLiteralExpression { Value = 3 }
//@[052:0056) |   └─StringLiteralExpression { Value = hi }
var arrayOfNonRuntimeFunctionCalls = [for i in range(0,3): concat('hi', i)]
//@[000:0075) ├─DeclaredVariableExpression { Name = arrayOfNonRuntimeFunctionCalls }
//@[037:0075) | └─ForLoopExpression
//@[047:0057) |   ├─FunctionCallExpression { Name = range }
//@[053:0054) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[055:0056) |   | └─IntegerLiteralExpression { Value = 3 }
//@[059:0074) |   └─FunctionCallExpression { Name = concat }
//@[066:0070) |     ├─StringLiteralExpression { Value = hi }
//@[072:0073) |     └─ArrayAccessExpression
//@[072:0073) |       ├─CopyIndexExpression
//@[047:0057) |       └─FunctionCallExpression { Name = range }
//@[053:0054) |         ├─IntegerLiteralExpression { Value = 0 }
//@[055:0056) |         └─IntegerLiteralExpression { Value = 3 }

var multilineString = '''
//@[000:0036) ├─DeclaredVariableExpression { Name = multilineString }
//@[022:0036) | └─StringLiteralExpression { Value = HELLO!\n }
HELLO!
'''

var multilineEmpty = ''''''
//@[000:0027) ├─DeclaredVariableExpression { Name = multilineEmpty }
//@[021:0027) | └─StringLiteralExpression { Value =  }
var multilineEmptyNewline = '''
//@[000:0035) ├─DeclaredVariableExpression { Name = multilineEmptyNewline }
//@[028:0035) | └─StringLiteralExpression { Value =  }
'''

// evaluates to '\'abc\''
var multilineExtraQuotes = ''''abc''''
//@[000:0038) ├─DeclaredVariableExpression { Name = multilineExtraQuotes }
//@[027:0038) | └─StringLiteralExpression { Value = 'abc' }

// evaluates to '\'\nabc\n\''
var multilineExtraQuotesNewlines = ''''
//@[000:0048) ├─DeclaredVariableExpression { Name = multilineExtraQuotesNewlines }
//@[035:0048) | └─StringLiteralExpression { Value = '\nabc\n' }
abc
''''

var multilineSingleLine = '''hello!'''
//@[000:0038) ├─DeclaredVariableExpression { Name = multilineSingleLine }
//@[026:0038) | └─StringLiteralExpression { Value = hello! }

var multilineFormatted = format('''
//@[000:0073) ├─DeclaredVariableExpression { Name = multilineFormatted }
//@[025:0073) | └─FunctionCallExpression { Name = format }
//@[032:0061) |   ├─StringLiteralExpression { Value = Hello,\nmy\nname is\n{0}\n }
Hello,
my
name is
{0}
''', 'Anthony')
//@[005:0014) |   └─StringLiteralExpression { Value = Anthony }

var multilineJavaScript = '''
//@[000:0586) ├─DeclaredVariableExpression { Name = multilineJavaScript }
//@[026:0586) | └─StringLiteralExpression { Value = // NOT RECOMMENDED PATTERN\nconst fs = require('fs');\n\nmodule.exports = function (context) {\n    fs.readFile('./hello.txt', (err, data) => {\n        if (err) {\n            context.log.error('ERROR', err);\n            // BUG #1: This will result in an uncaught exception that crashes the entire process\n            throw err;\n        }\n        context.log(`Data from file: ${data}`);\n        // context.done() should be called here\n    });\n    // BUG #2: Data is not guaranteed to be read before the Azure Function's invocation ends\n    context.done();\n}\n }
// NOT RECOMMENDED PATTERN
const fs = require('fs');

module.exports = function (context) {
    fs.readFile('./hello.txt', (err, data) => {
        if (err) {
            context.log.error('ERROR', err);
            // BUG #1: This will result in an uncaught exception that crashes the entire process
            throw err;
        }
        context.log(`Data from file: ${data}`);
        // context.done() should be called here
    });
    // BUG #2: Data is not guaranteed to be read before the Azure Function's invocation ends
    context.done();
}
'''

var providersTest = providers('Microsoft.Resources').namespace
//@[000:0062) ├─DeclaredVariableExpression { Name = providersTest }
//@[020:0062) | └─PropertyAccessExpression { PropertyName = namespace }
//@[020:0052) |   └─FunctionCallExpression { Name = providers }
//@[030:0051) |     └─StringLiteralExpression { Value = Microsoft.Resources }
var providersTest2 = providers('Microsoft.Resources', 'deployments').locations
//@[000:0078) ├─DeclaredVariableExpression { Name = providersTest2 }
//@[021:0078) | └─PropertyAccessExpression { PropertyName = locations }
//@[021:0068) |   └─FunctionCallExpression { Name = providers }
//@[031:0052) |     ├─StringLiteralExpression { Value = Microsoft.Resources }
//@[054:0067) |     └─StringLiteralExpression { Value = deployments }

var copyBlockInObject = {
//@[000:0120) ├─DeclaredVariableExpression { Name = copyBlockInObject }
//@[024:0120) | └─ObjectExpression
  copy: [
//@[002:0092) |   └─ObjectPropertyExpression
//@[002:0006) |     ├─StringLiteralExpression { Value = copy }
//@[008:0092) |     └─ArrayExpression
    {
//@[004:0078) |       └─ObjectExpression
      name: 'blah'
//@[006:0018) |         ├─ObjectPropertyExpression
//@[006:0010) |         | ├─StringLiteralExpression { Value = name }
//@[012:0018) |         | └─StringLiteralExpression { Value = blah }
      count: '[notAFunction()]'
//@[006:0031) |         ├─ObjectPropertyExpression
//@[006:0011) |         | ├─StringLiteralExpression { Value = count }
//@[013:0031) |         | └─StringLiteralExpression { Value = [notAFunction()] }
      input: {}
//@[006:0015) |         └─ObjectPropertyExpression
//@[006:0011) |           ├─StringLiteralExpression { Value = input }
//@[013:0015) |           └─ObjectExpression
    }
  ]
}

var joinedString = join(['I', 'love', 'Bicep!'], ' ')
//@[000:0053) ├─DeclaredVariableExpression { Name = joinedString }
//@[019:0053) | └─FunctionCallExpression { Name = join }
//@[024:0047) |   ├─ArrayExpression
//@[025:0028) |   | ├─StringLiteralExpression { Value = I }
//@[030:0036) |   | ├─StringLiteralExpression { Value = love }
//@[038:0046) |   | └─StringLiteralExpression { Value = Bicep! }
//@[049:0052) |   └─StringLiteralExpression { Value =   }

var prefix = take('food', 3)
//@[000:0028) ├─DeclaredVariableExpression { Name = prefix }
//@[013:0028) | └─FunctionCallExpression { Name = take }
//@[018:0024) |   ├─StringLiteralExpression { Value = food }
//@[026:0027) |   └─IntegerLiteralExpression { Value = 3 }
var isPrefixed = startsWith('food', 'foo')
//@[000:0042) ├─DeclaredVariableExpression { Name = isPrefixed }
//@[017:0042) | └─FunctionCallExpression { Name = startsWith }
//@[028:0034) |   ├─StringLiteralExpression { Value = food }
//@[036:0041) |   └─StringLiteralExpression { Value = foo }

var spread = {
//@[000:0044) ├─DeclaredVariableExpression { Name = spread }
//@[013:0044) | └─FunctionCallExpression { Name = shallowMerge }
//@[013:0044) |   └─ArrayExpression
//@[013:0044) |     ├─ObjectExpression
  foo: 'abc'
//@[002:0012) |     | └─ObjectPropertyExpression
//@[002:0005) |     |   ├─StringLiteralExpression { Value = foo }
//@[007:0012) |     |   └─StringLiteralExpression { Value = abc }
  ...issue1332
//@[005:0014) |     └─VariableReferenceExpression { Variable = issue1332 }
}

var test = {
//@[000:0039) ├─DeclaredVariableExpression { Name = test }
//@[011:0039) | └─FunctionCallExpression { Name = shallowMerge }
//@[011:0039) |   └─ArrayExpression
//@[011:0039) |     └─ObjectExpression
  ...spread
//@[005:0011) |     ├─VariableReferenceExpression { Variable = spread }
  bar: 'def'
//@[002:0012) |       └─ObjectPropertyExpression
//@[002:0005) |         ├─StringLiteralExpression { Value = bar }
//@[007:0012) |         └─StringLiteralExpression { Value = def }
}

var arraySpread = [...arrayOfBooleans, ...arrayOfHardCodedNumbers, ...arrayOfHardCodedStrings]
//@[000:0094) ├─DeclaredVariableExpression { Name = arraySpread }
//@[018:0094) | └─FunctionCallExpression { Name = flatten }
//@[018:0094) |   └─ArrayExpression
//@[022:0037) |     ├─VariableReferenceExpression { Variable = arrayOfBooleans }
//@[042:0065) |     ├─VariableReferenceExpression { Variable = arrayOfHardCodedNumbers }
//@[070:0093) |     └─VariableReferenceExpression { Variable = arrayOfHardCodedStrings }


var nameof1 = nameof(arraySpread)
//@[000:0033) ├─DeclaredVariableExpression { Name = nameof1 }
//@[021:0032) | └─StringLiteralExpression { Value = arraySpread }
var nameof2 = nameof(spread.foo)
//@[000:0032) ├─DeclaredVariableExpression { Name = nameof2 }
//@[021:0031) | └─StringLiteralExpression { Value = foo }
var nameof3 = nameof(myObj.obj.nested)
//@[000:0038) └─DeclaredVariableExpression { Name = nameof3 }
//@[021:0037)   └─StringLiteralExpression { Value = nested }

