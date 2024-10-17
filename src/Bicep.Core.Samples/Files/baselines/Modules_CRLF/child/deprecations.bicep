@deprecated('deprecated param')
param fooParam string?

@export()
@deprecated('deprecated var')
var fooVar = ''

@export()
@deprecated('deprecated func')
func fooFunc() string => ':('

@export()
@deprecated('deprecated type')
type fooType = {
  bar: string
}

@deprecated('deprecated output')
output fooOutput string = ':('
