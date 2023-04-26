func buildUrl(https bool, hostname string, path string) string => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@[000:503) ProgramExpression
//@[000:141) ├─DeclaredFunctionExpression { Name = buildUrl }
//@[013:141) | └─LambdaExpression
//@[066:141) |   └─InterpolatedStringExpression
//@[069:093) |     ├─TernaryExpression
//@[069:074) |     | ├─LambdaVariableReferenceExpression { Variable = https }
//@[077:084) |     | ├─StringLiteralExpression { Value = https }
//@[087:093) |     | └─StringLiteralExpression { Value = http }
//@[099:107) |     ├─LambdaVariableReferenceExpression { Variable = hostname }
//@[110:139) |     └─TernaryExpression
//@[110:121) |       ├─FunctionCallExpression { Name = empty }
//@[116:120) |       | └─LambdaVariableReferenceExpression { Variable = path }
//@[124:126) |       ├─StringLiteralExpression { Value =  }
//@[129:139) |       └─InterpolatedStringExpression
//@[133:137) |         └─LambdaVariableReferenceExpression { Variable = path }

output foo string = buildUrl(true, 'google.com', 'search')
//@[000:058) ├─DeclaredOutputExpression { Name = foo }
//@[020:058) | └─UserDefinedFunctionCallExpression { Name = buildUrl }
//@[029:033) |   ├─BooleanLiteralExpression { Value = True }
//@[035:047) |   ├─StringLiteralExpression { Value = google.com }
//@[049:057) |   └─StringLiteralExpression { Value = search }

func sayHello(name string) string => 'Hi ${name}!'
//@[000:050) ├─DeclaredFunctionExpression { Name = sayHello }
//@[013:050) | └─LambdaExpression
//@[037:050) |   └─InterpolatedStringExpression
//@[043:047) |     └─LambdaVariableReferenceExpression { Variable = name }

output hellos array = map(['Evie', 'Casper'], name => sayHello(name))
//@[000:069) └─DeclaredOutputExpression { Name = hellos }
//@[022:069)   └─FunctionCallExpression { Name = map }
//@[026:044)     ├─ArrayExpression
//@[027:033)     | ├─StringLiteralExpression { Value = Evie }
//@[035:043)     | └─StringLiteralExpression { Value = Casper }
//@[046:068)     └─LambdaExpression
//@[054:068)       └─UserDefinedFunctionCallExpression { Name = sayHello }
//@[063:067)         └─LambdaVariableReferenceExpression { Variable = name }

func objReturnType(name string) object => {
//@[000:068) ├─DeclaredFunctionExpression { Name = objReturnType }
//@[018:068) | └─LambdaExpression
//@[042:068) |   └─ObjectExpression
  hello: 'Hi ${name}!'
//@[002:022) |     └─ObjectPropertyExpression
//@[002:007) |       ├─StringLiteralExpression { Value = hello }
//@[009:022) |       └─InterpolatedStringExpression
//@[015:019) |         └─LambdaVariableReferenceExpression { Variable = name }
}

func arrayReturnType(name string) array => [
//@[000:053) ├─DeclaredFunctionExpression { Name = arrayReturnType }
//@[020:053) | └─LambdaExpression
//@[043:053) |   └─ArrayExpression
  name
//@[002:006) |     └─LambdaVariableReferenceExpression { Variable = name }
]

func asdf(name string) array => [
//@[000:051) ├─DeclaredFunctionExpression { Name = asdf }
//@[009:051) | └─LambdaExpression
//@[032:051) |   └─ArrayExpression
  'asdf'
//@[002:008) |     ├─StringLiteralExpression { Value = asdf }
  name
//@[002:006) |     └─LambdaVariableReferenceExpression { Variable = name }
]

