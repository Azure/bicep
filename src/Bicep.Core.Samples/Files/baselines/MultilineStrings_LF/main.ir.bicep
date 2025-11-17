var name = 'Anthony'
//@[00:492) ProgramExpression
//@[00:020) ├─DeclaredVariableExpression { Name = name }
//@[11:020) | └─StringLiteralExpression { Value = Anthony }
var multilineInterpolation = $'''
//@[00:063) ├─DeclaredVariableExpression { Name = multilineInterpolation }
//@[29:063) | └─InterpolatedStringExpression
Hello,
my
name is
${name}
//@[02:006) |   └─VariableReferenceExpression { Variable = name }
'''

var complexMultilineInterpolation = $$$'''
//@[00:084) ├─DeclaredVariableExpression { Name = complexMultilineInterpolation }
//@[36:084) | └─InterpolatedStringExpression
${name}
$${name}
$$${name}
//@[04:008) |   ├─VariableReferenceExpression { Variable = name }
$$$${name}
//@[05:009) |   └─VariableReferenceExpression { Variable = name }
'''

var interpMultiEmpty = $''''''
//@[00:030) ├─DeclaredVariableExpression { Name = interpMultiEmpty }
//@[23:030) | └─StringLiteralExpression { Value =  }
var interp1Multi = $'''
//@[00:040) ├─DeclaredVariableExpression { Name = interp1Multi }
//@[19:040) | └─InterpolatedStringExpression
abc${123}def
//@[05:008) |   └─IntegerLiteralExpression { Value = 123 }
'''
var interp2Multi = $'''${123}def'''
//@[00:035) ├─DeclaredVariableExpression { Name = interp2Multi }
//@[19:035) | └─InterpolatedStringExpression
//@[25:028) |   └─IntegerLiteralExpression { Value = 123 }
var interp3Multi = $$'''abc$${123}'''
//@[00:037) ├─DeclaredVariableExpression { Name = interp3Multi }
//@[19:037) | └─InterpolatedStringExpression
//@[30:033) |   └─IntegerLiteralExpression { Value = 123 }
var interp4Multi = $'''abc${123}${456}jk$l${789}p$'''
//@[00:053) ├─DeclaredVariableExpression { Name = interp4Multi }
//@[19:053) | └─InterpolatedStringExpression
//@[28:031) |   ├─IntegerLiteralExpression { Value = 123 }
//@[34:037) |   ├─IntegerLiteralExpression { Value = 456 }
//@[44:047) |   └─IntegerLiteralExpression { Value = 789 }
var doubleInterpMulti = $'''abc${'def${123}'}_${'${456}${789}'}'''
//@[00:066) ├─DeclaredVariableExpression { Name = doubleInterpMulti }
//@[24:066) | └─InterpolatedStringExpression
//@[33:044) |   ├─InterpolatedStringExpression
//@[39:042) |   | └─IntegerLiteralExpression { Value = 123 }
//@[48:062) |   └─InterpolatedStringExpression
//@[51:054) |     ├─IntegerLiteralExpression { Value = 456 }
//@[57:060) |     └─IntegerLiteralExpression { Value = 789 }
var curliesInInterpMulti = $'''{${123}{0}${true}}'''
//@[00:052) └─DeclaredVariableExpression { Name = curliesInInterpMulti }
//@[27:052)   └─InterpolatedStringExpression
//@[34:037)     ├─IntegerLiteralExpression { Value = 123 }
//@[43:047)     └─BooleanLiteralExpression { Value = True }

