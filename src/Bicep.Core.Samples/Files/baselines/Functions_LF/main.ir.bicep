func buildUrl(https bool, hostname string, path string) string => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@[000:1233) ProgramExpression
//@[000:0141) ├─DeclaredFunctionExpression { Name = buildUrl }
//@[013:0141) | └─LambdaExpression
//@[020:0024) |   ├─AmbientTypeReferenceExpression { Name = bool }
//@[035:0041) |   ├─AmbientTypeReferenceExpression { Name = string }
//@[048:0054) |   ├─AmbientTypeReferenceExpression { Name = string }
//@[066:0141) |   ├─InterpolatedStringExpression
//@[069:0093) |   | ├─TernaryExpression
//@[069:0074) |   | | ├─LambdaVariableReferenceExpression { Variable = https }
//@[077:0084) |   | | ├─StringLiteralExpression { Value = https }
//@[087:0093) |   | | └─StringLiteralExpression { Value = http }
//@[099:0107) |   | ├─LambdaVariableReferenceExpression { Variable = hostname }
//@[110:0139) |   | └─TernaryExpression
//@[110:0121) |   |   ├─FunctionCallExpression { Name = empty }
//@[116:0120) |   |   | └─LambdaVariableReferenceExpression { Variable = path }
//@[124:0126) |   |   ├─StringLiteralExpression { Value =  }
//@[129:0139) |   |   └─InterpolatedStringExpression
//@[133:0137) |   |     └─LambdaVariableReferenceExpression { Variable = path }
//@[056:0062) |   └─AmbientTypeReferenceExpression { Name = string }
//@[000:0000) |   ├─ObjectExpression [UNPARENTED]
//@[000:0000) |   ├─ObjectExpression [UNPARENTED]

output foo string = buildUrl(true, 'google.com', 'search')
//@[000:0058) ├─DeclaredOutputExpression { Name = foo }
//@[011:0017) | ├─AmbientTypeReferenceExpression { Name = string }
//@[020:0058) | └─UserDefinedFunctionCallExpression { Name = buildUrl }
//@[029:0033) |   ├─BooleanLiteralExpression { Value = True }
//@[035:0047) |   ├─StringLiteralExpression { Value = google.com }
//@[049:0057) |   └─StringLiteralExpression { Value = search }

func sayHello(name string) string => 'Hi ${name}!'
//@[000:0050) ├─DeclaredFunctionExpression { Name = sayHello }
//@[013:0050) | └─LambdaExpression
//@[019:0025) |   ├─AmbientTypeReferenceExpression { Name = string }
//@[037:0050) |   ├─InterpolatedStringExpression
//@[043:0047) |   | └─LambdaVariableReferenceExpression { Variable = name }
//@[027:0033) |   └─AmbientTypeReferenceExpression { Name = string }

output hellos array = map(['Evie', 'Casper'], name => sayHello(name))
//@[000:0069) ├─DeclaredOutputExpression { Name = hellos }
//@[014:0019) | ├─AmbientTypeReferenceExpression { Name = array }
//@[022:0069) | └─FunctionCallExpression { Name = map }
//@[026:0044) |   ├─ArrayExpression
//@[027:0033) |   | ├─StringLiteralExpression { Value = Evie }
//@[035:0043) |   | └─StringLiteralExpression { Value = Casper }
//@[046:0068) |   └─LambdaExpression
//@[054:0068) |     └─UserDefinedFunctionCallExpression { Name = sayHello }
//@[063:0067) |       └─LambdaVariableReferenceExpression { Variable = name }

func objReturnType(name string) object => {
//@[000:0068) ├─DeclaredFunctionExpression { Name = objReturnType }
//@[018:0068) | └─LambdaExpression
//@[024:0030) |   ├─AmbientTypeReferenceExpression { Name = string }
//@[042:0068) |   ├─ObjectExpression
//@[032:0038) |   └─AmbientTypeReferenceExpression { Name = object }
  hello: 'Hi ${name}!'
//@[002:0022) |   | └─ObjectPropertyExpression
//@[002:0007) |   |   ├─StringLiteralExpression { Value = hello }
//@[009:0022) |   |   └─InterpolatedStringExpression
//@[015:0019) |   |     └─LambdaVariableReferenceExpression { Variable = name }
}

func arrayReturnType(name string) array => [
//@[000:0053) ├─DeclaredFunctionExpression { Name = arrayReturnType }
//@[020:0053) | └─LambdaExpression
//@[026:0032) |   ├─AmbientTypeReferenceExpression { Name = string }
//@[043:0053) |   ├─ArrayExpression
//@[034:0039) |   └─AmbientTypeReferenceExpression { Name = array }
  name
//@[002:0006) |   | └─LambdaVariableReferenceExpression { Variable = name }
]

func asdf(name string) array => [
//@[000:0051) ├─DeclaredFunctionExpression { Name = asdf }
//@[009:0051) | └─LambdaExpression
//@[015:0021) |   ├─AmbientTypeReferenceExpression { Name = string }
//@[032:0051) |   ├─ArrayExpression
//@[023:0028) |   └─AmbientTypeReferenceExpression { Name = array }
  'asdf'
//@[002:0008) |   | ├─StringLiteralExpression { Value = asdf }
  name
//@[002:0006) |   | └─LambdaVariableReferenceExpression { Variable = name }
]

func person(name string,
//@[000:0103) ├─DeclaredFunctionExpression { Name = person }
//@[011:0103) | └─LambdaExpression
//@[017:0023) |   ├─AmbientTypeReferenceExpression { Name = string }
  age int,
//@[006:0009) |   ├─AmbientTypeReferenceExpression { Name = int }
weight int,
//@[007:0010) |   ├─AmbientTypeReferenceExpression { Name = int }
height int) array => [
//@[007:0010) |   ├─AmbientTypeReferenceExpression { Name = int }
//@[021:0055) |   ├─ArrayExpression
//@[012:0017) |   └─AmbientTypeReferenceExpression { Name = array }
  name
//@[002:0006) |   | ├─LambdaVariableReferenceExpression { Variable = name }
  age
//@[002:0005) |   | ├─LambdaVariableReferenceExpression { Variable = age }
  weight
//@[002:0008) |   | ├─LambdaVariableReferenceExpression { Variable = weight }
  height
//@[002:0008) |   | └─LambdaVariableReferenceExpression { Variable = height }
]

func longParameterList(one string, two string, three string, /* comment comment comment comment */ four string) array => [
//@[000:0151) ├─DeclaredFunctionExpression { Name = longParameterList }
//@[022:0151) | └─LambdaExpression
//@[027:0033) |   ├─AmbientTypeReferenceExpression { Name = string }
//@[039:0045) |   ├─AmbientTypeReferenceExpression { Name = string }
//@[053:0059) |   ├─AmbientTypeReferenceExpression { Name = string }
//@[104:0110) |   ├─AmbientTypeReferenceExpression { Name = string }
//@[121:0151) |   ├─ArrayExpression
//@[112:0117) |   └─AmbientTypeReferenceExpression { Name = array }
  one
//@[002:0005) |   | ├─LambdaVariableReferenceExpression { Variable = one }
  two
//@[002:0005) |   | ├─LambdaVariableReferenceExpression { Variable = two }
  three
//@[002:0007) |   | ├─LambdaVariableReferenceExpression { Variable = three }
  four
//@[002:0006) |   | └─LambdaVariableReferenceExpression { Variable = four }
]

@minValue(0)
//@[000:0035) ├─DeclaredTypeExpression { Name = positiveInt }
//@[010:0011) | ├─IntegerLiteralExpression { Value = 0 }
type positiveInt = int
//@[019:0022) | └─AmbientTypeReferenceExpression { Name = int }

func typedArg(input string[]) positiveInt => length(input)
//@[000:0058) ├─DeclaredFunctionExpression { Name = typedArg }
//@[013:0058) | └─LambdaExpression
//@[020:0028) |   ├─ArrayTypeExpression { Name = string[] }
//@[020:0026) |   | └─AmbientTypeReferenceExpression { Name = string }
//@[045:0058) |   ├─FunctionCallExpression { Name = length }
//@[052:0057) |   | └─LambdaVariableReferenceExpression { Variable = input }
//@[030:0041) |   └─TypeAliasReferenceExpression { Name = positiveInt }

func barTest() array => ['abc', 'def']
//@[000:0038) ├─DeclaredFunctionExpression { Name = barTest }
//@[012:0038) | └─LambdaExpression
//@[024:0038) |   ├─ArrayExpression
//@[025:0030) |   | ├─StringLiteralExpression { Value = abc }
//@[032:0037) |   | └─StringLiteralExpression { Value = def }
//@[015:0020) |   └─AmbientTypeReferenceExpression { Name = array }
func fooTest() array => map(barTest(), a => 'Hello ${a}!')
//@[000:0058) ├─DeclaredFunctionExpression { Name = fooTest }
//@[012:0058) | └─LambdaExpression
//@[024:0058) |   ├─FunctionCallExpression { Name = map }
//@[028:0037) |   | ├─UserDefinedFunctionCallExpression { Name = barTest }
//@[039:0057) |   | └─LambdaExpression
//@[044:0057) |   |   └─InterpolatedStringExpression
//@[053:0054) |   |     └─LambdaVariableReferenceExpression { Variable = a }
//@[015:0020) |   └─AmbientTypeReferenceExpression { Name = array }

output fooValue array = fooTest()
//@[000:0033) └─DeclaredOutputExpression { Name = fooValue }
//@[016:0021)   ├─AmbientTypeReferenceExpression { Name = array }
//@[024:0033)   └─UserDefinedFunctionCallExpression { Name = fooTest }

func test() object => loadJsonContent('./repro-data.json')
//@[000:0058) ├─DeclaredFunctionExpression { Name = test }
//@[009:0058) | └─LambdaExpression
//@[012:0018) |   └─AmbientTypeReferenceExpression { Name = object }
func test2() string => loadTextContent('./repro-data.json')
//@[000:0059) ├─DeclaredFunctionExpression { Name = test2 }
//@[010:0059) | └─LambdaExpression
//@[023:0059) |   ├─StringLiteralExpression { Value = {} }
//@[013:0019) |   └─AmbientTypeReferenceExpression { Name = string }
func test3() object => loadYamlContent('./repro-data.json')
//@[000:0059) ├─DeclaredFunctionExpression { Name = test3 }
//@[010:0059) | └─LambdaExpression
//@[013:0019) |   └─AmbientTypeReferenceExpression { Name = object }
func test4() string => loadFileAsBase64('./repro-data.json')
//@[000:0060) ├─DeclaredFunctionExpression { Name = test4 }
//@[010:0060) | └─LambdaExpression
//@[023:0060) |   ├─StringLiteralExpression { Value = e30= }
//@[013:0019) |   └─AmbientTypeReferenceExpression { Name = string }

