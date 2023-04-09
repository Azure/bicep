func buildUrl = (bool https, string hostname, string path) => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@[000:198) ProgramExpression
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
//@[000:058) └─DeclaredOutputExpression { Name = foo }
//@[020:058)   └─FunctionCallExpression { Name = buildUrl }
//@[029:033)     ├─BooleanLiteralExpression { Value = True }
//@[035:047)     ├─StringLiteralExpression { Value = google.com }
//@[049:057)     └─StringLiteralExpression { Value = search }

