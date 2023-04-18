func buildUrl = (bool https, string hostname, string path) => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@[000:317) ProgramExpression
//@[000:137) ├─DeclaredFunctionExpression { Name = buildUrl }
//@[016:137) | └─LambdaExpression
//@[062:137) |   └─InterpolatedStringExpression
//@[065:089) |     ├─TernaryExpression
//@[065:070) |     | ├─LambdaVariableReferenceExpression { Variable = https }
//@[073:080) |     | ├─StringLiteralExpression { Value = https }
//@[083:089) |     | └─StringLiteralExpression { Value = http }
//@[095:103) |     ├─LambdaVariableReferenceExpression { Variable = hostname }
//@[106:135) |     └─TernaryExpression
//@[106:117) |       ├─FunctionCallExpression { Name = empty }
//@[112:116) |       | └─LambdaVariableReferenceExpression { Variable = path }
//@[120:122) |       ├─StringLiteralExpression { Value =  }
//@[125:135) |       └─InterpolatedStringExpression
//@[129:133) |         └─LambdaVariableReferenceExpression { Variable = path }

output foo string = buildUrl(true, 'google.com', 'search')
//@[000:058) ├─DeclaredOutputExpression { Name = foo }
//@[020:058) | └─UserDefinedFunctionCallExpression { Name = buildUrl }
//@[029:033) |   ├─BooleanLiteralExpression { Value = True }
//@[035:047) |   ├─StringLiteralExpression { Value = google.com }
//@[049:057) |   └─StringLiteralExpression { Value = search }

func sayHello = (string name) => 'Hi ${name}!'
//@[000:046) ├─DeclaredFunctionExpression { Name = sayHello }
//@[016:046) | └─LambdaExpression
//@[033:046) |   └─InterpolatedStringExpression
//@[039:043) |     └─LambdaVariableReferenceExpression { Variable = name }

output hellos array = map(['Evie', 'Casper'], name => sayHello(name))
//@[000:069) └─DeclaredOutputExpression { Name = hellos }
//@[022:069)   └─FunctionCallExpression { Name = map }
//@[026:044)     ├─ArrayExpression
//@[027:033)     | ├─StringLiteralExpression { Value = Evie }
//@[035:043)     | └─StringLiteralExpression { Value = Casper }
//@[046:068)     └─LambdaExpression
//@[054:068)       └─UserDefinedFunctionCallExpression { Name = sayHello }
//@[063:067)         └─LambdaVariableReferenceExpression { Variable = name }

