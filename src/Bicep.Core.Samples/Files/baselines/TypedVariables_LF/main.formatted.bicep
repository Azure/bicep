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
  foo: 'abc'
  bar: 123
}
