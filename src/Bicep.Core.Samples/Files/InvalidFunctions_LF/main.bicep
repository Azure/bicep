func useRuntimeFunction = () => string reference('foo').bar

func constFunc = () => string 'A'
func funcWithOtherFuncRef = () => string constFunc()

func invalidType = (string input) => string input

output invalidType string = invalidType(true)

func madeUpTypeArgs = (notAType a, alsoNotAType b) => string '${a}-${b}'

func noLambda = ('foo') => string ''

func noLambda2 = ('foo' sdf) => string ''

func noLambda3 = string 'asdf'

func argLengthMismatch = (string a, string b, string c) => array ([a, b, c])
var sdf = argLengthMismatch('asdf')

var asdfwdf = noLambda('asd')

func sayHello = (string name) => string 'Hi ${name}!'
output hellos array = map(['Evie', 'Casper'], sayHello) // this syntax not supported currently, but should it be?

func sayHelloBadNewlines = (
  string name) => string 'Hi ${name}!'
