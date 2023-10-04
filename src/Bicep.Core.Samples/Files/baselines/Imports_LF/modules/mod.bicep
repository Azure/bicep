@export()
var foo = bar

var bar = baz

var baz = 'quux'

@export()
type fizz = buzz[]

type buzz = {
  property: pop?
}

@minLength(3)
type pop = string
