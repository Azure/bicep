@export()
//@[00:304) ProgramExpression
//@[00:043) ├─DeclaredVariableExpression { Name = exportedString }
//@[01:009) | ├─FunctionCallExpression { Name = export }
var exportedString string = 'foo'
//@[19:025) | ├─AmbientTypeReferenceExpression { Name = string }
//@[28:033) | └─StringLiteralExpression { Value = foo }

@export()
//@[00:090) ├─DeclaredVariableExpression { Name = exporteInlineType }
//@[01:009) | ├─FunctionCallExpression { Name = export }
var exporteInlineType {
//@[22:050) | ├─ObjectTypeExpression { Name = { foo: string, bar: int } }
  foo: string
//@[02:013) | | ├─ObjectTypePropertyExpression
//@[07:013) | | | └─AmbientTypeReferenceExpression { Name = string }
  bar: int
//@[02:010) | | └─ObjectTypePropertyExpression
//@[07:010) | |   └─AmbientTypeReferenceExpression { Name = int }
} = {
//@[04:031) | └─ObjectExpression
  foo: 'abc'
//@[02:012) |   ├─ObjectPropertyExpression
//@[02:005) |   | ├─StringLiteralExpression { Value = foo }
//@[07:012) |   | └─StringLiteralExpression { Value = abc }
  bar: 123
//@[02:010) |   └─ObjectPropertyExpression
//@[02:005) |     ├─StringLiteralExpression { Value = bar }
//@[07:010) |     └─IntegerLiteralExpression { Value = 123 }
}

type FooType = {
//@[00:043) ├─DeclaredTypeExpression { Name = FooType }
//@[15:043) | └─ObjectTypeExpression { Name = { foo: string, bar: int } }
  foo: string
//@[02:013) |   ├─ObjectTypePropertyExpression
//@[07:013) |   | └─AmbientTypeReferenceExpression { Name = string }
  bar: int
//@[02:010) |   └─ObjectTypePropertyExpression
//@[07:010) |     └─AmbientTypeReferenceExpression { Name = int }
}

@export()
//@[00:067) ├─DeclaredVariableExpression { Name = exportedTypeRef }
//@[01:009) | ├─FunctionCallExpression { Name = export }
var exportedTypeRef FooType = {
//@[20:027) | ├─TypeAliasReferenceExpression { Name = FooType }
//@[30:057) | └─ObjectExpression
  foo: 'abc'
//@[02:012) |   ├─ObjectPropertyExpression
//@[02:005) |   | ├─StringLiteralExpression { Value = foo }
//@[07:012) |   | └─StringLiteralExpression { Value = abc }
  bar: 123
//@[02:010) |   └─ObjectPropertyExpression
//@[02:005) |     ├─StringLiteralExpression { Value = bar }
//@[07:010) |     └─IntegerLiteralExpression { Value = 123 }
}

var unExported FooType = {
//@[00:052) └─DeclaredVariableExpression { Name = unExported }
//@[15:022)   ├─TypeAliasReferenceExpression { Name = FooType }
//@[25:052)   └─ObjectExpression
  foo: 'abc'
//@[02:012)     ├─ObjectPropertyExpression
//@[02:005)     | ├─StringLiteralExpression { Value = foo }
//@[07:012)     | └─StringLiteralExpression { Value = abc }
  bar: 123
//@[02:010)     └─ObjectPropertyExpression
//@[02:005)       ├─StringLiteralExpression { Value = bar }
//@[07:010)       └─IntegerLiteralExpression { Value = 123 }
}

