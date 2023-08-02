using 'main.bicep'

@description('blah blah')
var blah = 'blah'

param foo = blah

var abc = 'abc'
var def = {
  abc: toUpper(abc)
}

param fooObj = {
  def: def
  ghi: 'ghi'
}
var list = 'FOO,BAR,BAZ'
param bar = join(map(range(0, 3), i => split(list, ',')[2 - i]), ',')

