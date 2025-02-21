@export()
var exportedString string = 'foo'
//@[4:18) Variable exportedString. Type: string. Declaration start char: 0, length: 43

@export()
var exporteInlineType {
//@[4:21) Variable exporteInlineType. Type: { foo: string, bar: int }. Declaration start char: 0, length: 90
  foo: string
  bar: int
} = {
  foo: 'abc'
  bar: 123
}

type FooType = {
//@[5:12) TypeAlias FooType. Type: Type<{ foo: string, bar: int }>. Declaration start char: 0, length: 43
  foo: string
  bar: int
}

@export()
var exportedTypeRef FooType = {
//@[4:19) Variable exportedTypeRef. Type: { foo: string, bar: int }. Declaration start char: 0, length: 67
  foo: 'abc'
  bar: 123
}

var unExported FooType = {
//@[4:14) Variable unExported. Type: { foo: string, bar: int }. Declaration start char: 0, length: 52
  foo: 'abc'
  bar: 123
}

