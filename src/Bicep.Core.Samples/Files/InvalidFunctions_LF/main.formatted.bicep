func useRuntimeFunction = () => reference('foo').bar

func constFunc = () => 'A'
func funcWithOtherFuncRef = () => constFunc()

func invalidType = (stringinput) => input

output invalidType string = invalidType(true)

func madeUpTypeArgs = (notATypea, alsoNotATypeb) => '${a}-${b}'

func noLambda = ('foo') => ''

func noLambda2 = ('foo'sdf) => ''

func noLambda3 = 'asdf'

func argLengthMismatch = (stringa, stringb, stringc) => [ a, b, c ]
var sdf = argLengthMismatch('asdf')

var asdfwdf = noLambda('asd')

func buildUrl = (boolhttps, stringhostname, stringpath) => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'

output foo array = buildUrl(true, 'google.com', 'search')

func sayHello = (stringname) => 'Hi ${name}!'
output hellos array = map([ 'Evie', 'Casper' ], sayHello) // this syntax not supported currently, but should it be?

func sayHelloBadNewlines = (
string name) => 'Hi ${name}!'
