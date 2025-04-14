using 'main.bicep'

@description('blah blah')
var blah = 'blah'
//@[04:08) Variable blah. Type: 'blah'. Declaration start char: 0, length: 43

param foo = blah
//@[06:09) ParameterAssignment foo. Type: 'blah'. Declaration start char: 0, length: 16

var abc = 'abc'
//@[04:07) Variable abc. Type: 'abc'. Declaration start char: 0, length: 15
var def = {
//@[04:07) Variable def. Type: object. Declaration start char: 0, length: 33
  abc: toUpper(abc)
}

param fooObj = {
//@[06:12) ParameterAssignment fooObj. Type: object. Declaration start char: 0, length: 42
  def: def
  ghi: 'ghi'
}
var list = 'FOO,BAR,BAZ'
//@[04:08) Variable list. Type: 'FOO,BAR,BAZ'. Declaration start char: 0, length: 24
param bar = join(map(range(0, 3), i => split(list, ',')[2 - i]), ',')
//@[34:35) Local i. Type: int. Declaration start char: 34, length: 1
//@[06:09) ParameterAssignment bar. Type: string. Declaration start char: 0, length: 69

