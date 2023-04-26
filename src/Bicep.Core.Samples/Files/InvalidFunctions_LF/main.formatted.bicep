func useRuntimeFunction() string => reference('foo').bar

func constFunc() string => 'A'
func funcWithOtherFuncRef() string => constFunc()

func missingArgType(input) string => input

func missingOutputType(input string) => input

func invalidType(input string) string => input

output invalidType string = invalidType(true)

func madeUpTypeArgs(a notAType, b alsoNotAType) string => '${a}-${b}'

func noLambda('foo') string => ''

func noLambda2 = (sdf 'foo') string => ''

func noLambda3 = string 'asdf'

func argLengthMismatch(a string, b string, c string) array => ([ a, b, c ])
var sdf = argLengthMismatch('asdf')

var asdfwdf = noLambda('asd')

func sayHello(name string) string => 'Hi ${name}!'
output hellos array = map([ 'Evie', 'Casper' ], sayHello) // this syntax not supported currently, but should it be?

func sayHelloBadNewlines(
name string) string => 'Hi ${name}!'
