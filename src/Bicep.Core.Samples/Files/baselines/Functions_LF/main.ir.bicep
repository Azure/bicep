func buildUrl(https bool, hostname string, path string) string => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@[000:975) ProgramExpression
//@[000:141) ├─DeclaredFunctionExpression { Name = buildUrl }
//@[013:141) | └─LambdaExpression
//@[020:024) |   ├─AmbientTypeReferenceExpression { Name = bool }
//@[035:041) |   ├─AmbientTypeReferenceExpression { Name = string }
//@[048:054) |   ├─AmbientTypeReferenceExpression { Name = string }
//@[066:141) |   ├─InterpolatedStringExpression
//@[069:093) |   | ├─TernaryExpression
//@[069:074) |   | | ├─LambdaVariableReferenceExpression { Variable = https }
//@[077:084) |   | | ├─StringLiteralExpression { Value = https }
//@[087:093) |   | | └─StringLiteralExpression { Value = http }
//@[099:107) |   | ├─LambdaVariableReferenceExpression { Variable = hostname }
//@[110:139) |   | └─TernaryExpression
//@[110:121) |   |   ├─FunctionCallExpression { Name = empty }
//@[116:120) |   |   | └─LambdaVariableReferenceExpression { Variable = path }
//@[124:126) |   |   ├─StringLiteralExpression { Value =  }
//@[129:139) |   |   └─InterpolatedStringExpression
//@[133:137) |   |     └─LambdaVariableReferenceExpression { Variable = path }
//@[056:062) |   └─AmbientTypeReferenceExpression { Name = string }
//@[000:000) |   ├─ObjectExpression [UNPARENTED]
//@[000:000) |   ├─ObjectExpression [UNPARENTED]

output foo string = buildUrl(true, 'google.com', 'search')
//@[000:058) ├─DeclaredOutputExpression { Name = foo }
//@[011:017) | ├─AmbientTypeReferenceExpression { Name = string }
//@[020:058) | └─UserDefinedFunctionCallExpression { Name = buildUrl }
//@[029:033) |   ├─BooleanLiteralExpression { Value = True }
//@[035:047) |   ├─StringLiteralExpression { Value = google.com }
//@[049:057) |   └─StringLiteralExpression { Value = search }

func sayHello(name string) string => 'Hi ${name}!'
//@[000:050) ├─DeclaredFunctionExpression { Name = sayHello }
//@[013:050) | └─LambdaExpression
//@[019:025) |   ├─AmbientTypeReferenceExpression { Name = string }
//@[037:050) |   ├─InterpolatedStringExpression
//@[043:047) |   | └─LambdaVariableReferenceExpression { Variable = name }
//@[027:033) |   └─AmbientTypeReferenceExpression { Name = string }

output hellos array = map(['Evie', 'Casper'], name => sayHello(name))
//@[000:069) ├─DeclaredOutputExpression { Name = hellos }
//@[014:019) | ├─AmbientTypeReferenceExpression { Name = array }
//@[022:069) | └─FunctionCallExpression { Name = map }
//@[026:044) |   ├─ArrayExpression
//@[027:033) |   | ├─StringLiteralExpression { Value = Evie }
//@[035:043) |   | └─StringLiteralExpression { Value = Casper }
//@[046:068) |   └─LambdaExpression
//@[054:068) |     └─UserDefinedFunctionCallExpression { Name = sayHello }
//@[063:067) |       └─LambdaVariableReferenceExpression { Variable = name }

func objReturnType(name string) object => {
//@[000:068) ├─DeclaredFunctionExpression { Name = objReturnType }
//@[018:068) | └─LambdaExpression
//@[024:030) |   ├─AmbientTypeReferenceExpression { Name = string }
//@[042:068) |   ├─ObjectExpression
//@[032:038) |   └─AmbientTypeReferenceExpression { Name = object }
  hello: 'Hi ${name}!'
//@[002:022) |   | └─ObjectPropertyExpression
//@[002:007) |   |   ├─StringLiteralExpression { Value = hello }
//@[009:022) |   |   └─InterpolatedStringExpression
//@[015:019) |   |     └─LambdaVariableReferenceExpression { Variable = name }
}

func arrayReturnType(name string) array => [
//@[000:053) ├─DeclaredFunctionExpression { Name = arrayReturnType }
//@[020:053) | └─LambdaExpression
//@[026:032) |   ├─AmbientTypeReferenceExpression { Name = string }
//@[043:053) |   ├─ArrayExpression
//@[034:039) |   └─AmbientTypeReferenceExpression { Name = array }
  name
//@[002:006) |   | └─LambdaVariableReferenceExpression { Variable = name }
]

func asdf(name string) array => [
//@[000:051) ├─DeclaredFunctionExpression { Name = asdf }
//@[009:051) | └─LambdaExpression
//@[015:021) |   ├─AmbientTypeReferenceExpression { Name = string }
//@[032:051) |   ├─ArrayExpression
//@[023:028) |   └─AmbientTypeReferenceExpression { Name = array }
  'asdf'
//@[002:008) |   | ├─StringLiteralExpression { Value = asdf }
  name
//@[002:006) |   | └─LambdaVariableReferenceExpression { Variable = name }
]

@minValue(0)
//@[000:035) ├─DeclaredTypeExpression { Name = positiveInt }
//@[010:011) | ├─IntegerLiteralExpression { Value = 0 }
type positiveInt = int
//@[019:022) | └─AmbientTypeReferenceExpression { Name = int }

func typedArg(input string[]) positiveInt => length(input)
//@[000:058) ├─DeclaredFunctionExpression { Name = typedArg }
//@[013:058) | └─LambdaExpression
//@[020:028) |   ├─ArrayTypeExpression { Name = string[] }
//@[020:026) |   | └─AmbientTypeReferenceExpression { Name = string }
//@[045:058) |   ├─FunctionCallExpression { Name = length }
//@[052:057) |   | └─LambdaVariableReferenceExpression { Variable = input }
//@[030:041) |   └─TypeAliasReferenceExpression { Name = positiveInt }

func barTest() array => ['abc', 'def']
//@[000:038) ├─DeclaredFunctionExpression { Name = barTest }
//@[012:038) | └─LambdaExpression
//@[024:038) |   ├─ArrayExpression
//@[025:030) |   | ├─StringLiteralExpression { Value = abc }
//@[032:037) |   | └─StringLiteralExpression { Value = def }
//@[015:020) |   └─AmbientTypeReferenceExpression { Name = array }
func fooTest() array => map(barTest(), a => 'Hello ${a}!')
//@[000:058) ├─DeclaredFunctionExpression { Name = fooTest }
//@[012:058) | └─LambdaExpression
//@[024:058) |   ├─FunctionCallExpression { Name = map }
//@[028:037) |   | ├─UserDefinedFunctionCallExpression { Name = barTest }
//@[039:057) |   | └─LambdaExpression
//@[044:057) |   |   └─InterpolatedStringExpression
//@[053:054) |   |     └─LambdaVariableReferenceExpression { Variable = a }
//@[015:020) |   └─AmbientTypeReferenceExpression { Name = array }

output fooValue array = fooTest()
//@[000:033) └─DeclaredOutputExpression { Name = fooValue }
//@[016:021)   ├─AmbientTypeReferenceExpression { Name = array }
//@[024:033)   └─UserDefinedFunctionCallExpression { Name = fooTest }

func test() object => loadJsonContent('./repro-data.json')
//@[000:058) ├─DeclaredFunctionExpression { Name = test }
//@[009:058) | └─LambdaExpression
//@[012:018) |   └─AmbientTypeReferenceExpression { Name = object }
func test2() string => loadTextContent('./repro-data.json')
//@[000:059) ├─DeclaredFunctionExpression { Name = test2 }
//@[010:059) | └─LambdaExpression
//@[023:059) |   ├─StringLiteralExpression { Value = {} }
//@[013:019) |   └─AmbientTypeReferenceExpression { Name = string }
func test3() object => loadYamlContent('./repro-data.json')
//@[000:059) ├─DeclaredFunctionExpression { Name = test3 }
//@[010:059) | └─LambdaExpression
//@[013:019) |   └─AmbientTypeReferenceExpression { Name = object }
func test4() string => loadFileAsBase64('./repro-data.json')
//@[000:060) ├─DeclaredFunctionExpression { Name = test4 }
//@[010:060) | └─LambdaExpression
//@[023:060) |   ├─StringLiteralExpression { Value = e30= }
//@[013:019) |   └─AmbientTypeReferenceExpression { Name = string }

