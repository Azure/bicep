using 'main.bicep'

@description('blah blah')
var blah = 'blah'

param foo = blah
//@[6:09) ParameterAssignment foo. Type: 'blah'. Declaration start char: 0, length: 16

var abc = 'abc'
var def = {
  abc: toUpper(abc)
}

param fooObj = {
//@[6:12) ParameterAssignment fooObj. Type: object. Declaration start char: 0, length: 42
  def: def
  ghi: 'ghi'
}
var list = 'FOO,BAR,BAZ'
param bar = join(map(range(0, 3), i => split(list, ',')[2 - i]), ',')
//@[6:09) ParameterAssignment bar. Type: string. Declaration start char: 0, length: 69

