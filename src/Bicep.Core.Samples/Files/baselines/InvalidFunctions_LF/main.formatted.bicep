func useRuntimeFunction() string => reference('foo').bar

func missingArgType(input) string => input

func missingOutputType(input string) => input

func invalidType(input string) string => input

output invalidType string = invalidType(true)

func madeUpTypeArgs(a notAType, b alsoNotAType) string => '${a}-${b}'

func noLambda('foo') string => ''

func noLambda2 = (sdf 'foo') string => ''

func noLambda3 = string 'asdf'

func argLengthMismatch(a string, b string, c string) array => ([a, b, c])
var sdf = argLengthMismatch('asdf')

var asdfwdf = noLambda('asd')

func sayHello(name string) string => 'Hi ${name}!'
output hellos array = map(['Evie', 'Casper'], sayHello) // this syntax not supported currently, but should it be?

func sayHelloBadNewlines(name string) string => 'Hi ${name}!'

type validStringLiteralUnion = 'foo' | 'bar' | 'baz'
func invalidArgs(a validStringLiteralUnion, b string) string => a
func invalidOutput() validStringLiteralUnion => 'foo'

func recursive() string => recursive()

func recursiveA() string => recursiveB()
func recursiveB() string => recursiveA()

func onlyComma(,) string => 'foo'
func trailingCommas(a string,,) string => 'foo'
func multiLineOnly(
  a string
  b string) string => 'foo'

func multiLineTrailingCommas(
  a string,
  ,) string => 'foo'

func lineBeforeComma(a string, b string) string => 'foo'

output likeWrongArgcount bool = like('abc')
output likeWrongArgcount2 bool = like('abcdef', 'a*', 'abcd*')
output likeWrongType bool = like(123, 'a*')
output likeWrongReturnType string = like('abcd', 'a*')
