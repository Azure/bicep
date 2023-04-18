func useRuntimeFunction = () => reference('foo').bar

func constFunc = () => 'A'
func funcWithOtherFuncRef = () => constFunc()

func invalidType = (inputstring) => input

output invalidType string = invalidType(true)

func madeUpTypeArgs = (anotAType, balsoNotAType) => '${a}-${b}'

func noLambda = ('foo') => ''

func noLambda2 = (sdf'foo') => ''

func noLambda3 = 'asdf'

func argLengthMismatch = (astring, bstring, cstring) => [ a, b, c ]
var sdf = argLengthMismatch('asdf')

var asdfwdf = noLambda('asd')

func buildUrl = (httpsbool, hostnamestring, pathstring) => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'

output foo array = buildUrl(true, 'google.com', 'search')

func sayHello = (namestring) => 'Hi ${name}!'
output hellos array = map([ 'Evie', 'Casper' ], sayHello) // this syntax not supported currently, but should it be?

func sayHelloBadNewlines = (
string name) => 'Hi ${name}!'
