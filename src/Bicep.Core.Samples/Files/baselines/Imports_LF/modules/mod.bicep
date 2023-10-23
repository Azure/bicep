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
@export()
type pop = string

func echo(input string) string => input

@export()
@description('Say hi to someone!')
func greet(name string) string => 'Hi, ${name}!'
