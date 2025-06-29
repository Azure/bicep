@export()
var exportedString string = 'foo'

@export()
var exporteInlineType {
  foo: string
  bar: int
} = {
  foo: 'abc'
  bar: 123
}

type FooType = {
  foo: string
  bar: int
}

@export()
var exportedTypeRef FooType = {
  foo: 'abc'
  bar: 123
}

var unExported FooType = {
//@[4:14) [no-unused-vars (Warning)] Variable "unExported" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |unExported|
  foo: 'abc'
  bar: 123
}

