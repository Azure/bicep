func useRuntimeFunction = () => reference('foo').bar

func constFunc = () => 'A'
func funcWithOtherFuncRef = () => constFunc()

func invalidType = (string input) => input

output invalidType string = invalidType(true)

func madeUpTypeArgs = (notAType a, alsoNotAType b) => '${a}-${b}'

func noLambda = ('foo') => ''

func noLambda2 = ('foo' sdf) => ''

func noLambda3 = 'asdf'

func argLengthMismatch = (string a, string b, string c) => [ a, b, c ]
var sdf = argLengthMismatch('asdf')

var asdfwdf = noLambda('asd')

func buildUrl = (bool https, string hostname, string path) => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'

output foo array = buildUrl(true, 'google.com', 'search')

func sayHello = (string name) => 'Hi ${name}!'
output hellos array = map([ 'Evie', 'Casper' ], sayHello) // this syntax not supported currently, but should it be?

func sayHelloBadNewlines = (
string name) => 'Hi ${name}!'
