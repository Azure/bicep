func buildUrl(https bool, hostname string, path string) string =>
  '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'

output foo string = buildUrl(true, 'google.com', 'search')

func sayHello(name string) string => 'Hi ${name}!'

output hellos array = map(['Evie', 'Casper'], name => sayHello(name))

func objReturnType(name string) object => {
  hello: 'Hi ${name}!'
}

func arrayReturnType(name string) array => [name]

func asdf(name string) array => ['asdf', name]

func person(name string, age int, weight int, height int) array => [
  name
  age
  weight
  height
]

func longParameterList(
  one string,
  two string,
  three string,
  /* comment comment comment comment */ four string
) array => [one, two, three, four]

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
