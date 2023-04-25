func buildUrl = (https bool, hostname string, path string) string => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@[000:462) ProgramExpression
//@[000:144) ├─DeclaredFunctionExpression { Name = buildUrl }
//@[016:144) | └─LambdaExpression
//@[069:144) |   └─InterpolatedStringExpression
//@[072:096) |     ├─TernaryExpression
//@[072:077) |     | ├─LambdaVariableReferenceExpression { Variable = https }
//@[080:087) |     | ├─StringLiteralExpression { Value = https }
//@[090:096) |     | └─StringLiteralExpression { Value = http }
//@[102:110) |     ├─LambdaVariableReferenceExpression { Variable = hostname }
//@[113:142) |     └─TernaryExpression
//@[113:124) |       ├─FunctionCallExpression { Name = empty }
//@[119:123) |       | └─LambdaVariableReferenceExpression { Variable = path }
//@[127:129) |       ├─StringLiteralExpression { Value =  }
//@[132:142) |       └─InterpolatedStringExpression
//@[136:140) |         └─LambdaVariableReferenceExpression { Variable = path }

output foo string = buildUrl(true, 'google.com', 'search')
//@[000:058) ├─DeclaredOutputExpression { Name = foo }
//@[020:058) | └─UserDefinedFunctionCallExpression { Name = buildUrl }
//@[029:033) |   ├─BooleanLiteralExpression { Value = True }
//@[035:047) |   ├─StringLiteralExpression { Value = google.com }
//@[049:057) |   └─StringLiteralExpression { Value = search }

func sayHello = (name string) string => 'Hi ${name}!'
//@[000:053) ├─DeclaredFunctionExpression { Name = sayHello }
//@[016:053) | └─LambdaExpression
//@[040:053) |   └─InterpolatedStringExpression
//@[046:050) |     └─LambdaVariableReferenceExpression { Variable = name }

output hellos array = map(['Evie', 'Casper'], name => sayHello(name))
//@[000:069) └─DeclaredOutputExpression { Name = hellos }
//@[022:069)   └─FunctionCallExpression { Name = map }
//@[026:044)     ├─ArrayExpression
//@[027:033)     | ├─StringLiteralExpression { Value = Evie }
//@[035:043)     | └─StringLiteralExpression { Value = Casper }
//@[046:068)     └─LambdaExpression
//@[054:068)       └─UserDefinedFunctionCallExpression { Name = sayHello }
//@[063:067)         └─LambdaVariableReferenceExpression { Variable = name }

func objReturnType = (name string) object => {
//@[000:071) ├─DeclaredFunctionExpression { Name = objReturnType }
//@[021:071) | └─LambdaExpression
//@[045:071) |   └─ObjectExpression
  hello: 'Hi ${name}!'
//@[002:022) |     └─ObjectPropertyExpression
//@[002:007) |       ├─StringLiteralExpression { Value = hello }
//@[009:022) |       └─InterpolatedStringExpression
//@[015:019) |         └─LambdaVariableReferenceExpression { Variable = name }
}

func arrayReturnType = (name string) array => [
//@[000:056) ├─DeclaredFunctionExpression { Name = arrayReturnType }
//@[023:056) | └─LambdaExpression
//@[046:056) |   └─ArrayExpression
  name
//@[002:006) |     └─LambdaVariableReferenceExpression { Variable = name }
]

