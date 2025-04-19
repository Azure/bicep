func helpers_buildUrl(https bool, hostname string, path string) string =>
  '${(https?'https':'http')}://${hostname}${(empty(path)?'':'/${path}')}'

func helpers_sayHello(name string) string => 'Hi ${name}!'

func helpers_objReturnType(name string) object => {
  hello: 'Hi ${name}!'
}

func helpers_arrayReturnType(name string) array => [
  name
]

func helpers_asdf(name string) array => [
  'asdf'
  name
]

func helpers_typedArg(input array) int => length(input)

func helpers_barTest() array => [
  'abc'
  'def'
]

func helpers_fooTest() array => map(helpers_barTest(), a => 'Hello ${a}!')

func helpers_test() object => {}

func helpers_test2() string => '{}'

func helpers_test3() object => {}

func helpers_test4() string => 'e30='

func helpers_a(____________________________________________________________________________________________ string) string =>
  'a'

func helpers_b(
  longParameterName1 string,
  longParameterName2 string,
  longParameterName3 string,
  longParameterName4 string
) string => 'b'

func helpers_buildUrlMultiLine(https bool, hostname string, path string) string =>
  '${(https?'https':'http')}://${hostname}${(empty(path)?'':'/${path}')}'

output foo string = helpers_buildUrl(true, 'google.com', 'search')
output hellos array = map(
  [
    'Evie'
    'Casper'
  ],
  name => helpers_sayHello(name)
)
output fooValue array = helpers_fooTest()

