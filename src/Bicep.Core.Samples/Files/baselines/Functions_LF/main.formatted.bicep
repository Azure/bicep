func buildUrl(https bool, hostname string, path string) string =>
  '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'

output foo string = buildUrl(true, 'google.com', 'search')

func sayHello(name string) string => 'Hi ${name}!'

output hellos array = map(['Evie', 'Casper'], name => sayHello(name))

func objReturnType(name string) object => {
  hello: 'Hi ${name}!'
}

func arrayReturnType(name string) array => [
  name
]

func asdf(name string) array => [
  'asdf'
  name
]

@minValue(0)
type positiveInt = int

func typedArg(input string[]) positiveInt => length(input)

func barTest() array => ['abc', 'def']
func fooTest() array => map(barTest(), a => 'Hello ${a}!')

output fooValue array = fooTest()

func test() object => loadJsonContent('./repro-data.json')
func test2() string => loadTextContent('./repro-data.json')
func test3() object => loadYamlContent('./repro-data.json')
func test4() string => loadFileAsBase64('./repro-data.json')

// validate formatter works (https://github.com/Azure/bicep/issues/12913)
func a(____________________________________________________________________________________________ string) string =>
  'a'
func b(longParameterName1 string, longParameterName2 string, longParameterName3 string, longParameterName4 string) string =>
  'b'

func buildUrlMultiLine(https bool, hostname string, path string) string =>
  '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'

output likeExactMatch bool = like('abc', 'abc')
output likeWildCardMatch bool = like('abcdef', 'a*c*')
