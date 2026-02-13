func buildUrl(https bool, hostname string, path string) string => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'

output foo string = buildUrl(true, 'google.com', 'search')

func sayHello(name string) string => 'Hi ${name}!'

output hellos array = map(['Evie', 'Casper'], name => sayHello(name))
//@[14:19) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|

func objReturnType(name string) object => {
//@[32:38) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |object|
  hello: 'Hi ${name}!'
}

func arrayReturnType(name string) array => [
//@[34:39) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
  name
]

func asdf(name string) array => [
//@[23:28) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
  'asdf'
  name
]

@minValue(0)
type positiveInt = int

func typedArg(input string[]) positiveInt => length(input)

func barTest() array => ['abc', 'def']
//@[15:20) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
func fooTest() array => map(barTest(), a => 'Hello ${a}!')
//@[15:20) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|

output fooValue array = fooTest()
//@[16:21) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|

func test() object => loadJsonContent('./repro-data.json')
//@[12:18) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |object|
func test2() string => loadTextContent('./repro-data.json')
func test3() object => loadYamlContent('./repro-data.json')
//@[13:19) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |object|
func test4() string => loadFileAsBase64('./repro-data.json')

// validate formatter works (https://github.com/Azure/bicep/issues/12913)
func a(____________________________________________________________________________________________ string) string => 'a'
func b(longParameterName1 string, longParameterName2 string, longParameterName3 string, longParameterName4 string) string => 'b'

func buildUrlMultiLine(
  https bool,
  hostname string,
  path string
) string => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'

output likeExactMatch bool =like('abc', 'abc')
//@[33:45) [BCP234 (Warning)] The ARM function "like" failed when invoked on the value [abc, abc]: The template function 'like' is not valid. Please see https://aka.ms/arm-functions for usage details. (bicep https://aka.ms/bicep/core-diagnostics#BCP234) |'abc', 'abc'|
output likeWildCardMatch bool= like ('abcdef', 'a*c*')
//@[37:53) [BCP234 (Warning)] The ARM function "like" failed when invoked on the value [abcdef, a*c*]: The template function 'like' is not valid. Please see https://aka.ms/arm-functions for usage details. (bicep https://aka.ms/bicep/core-diagnostics#BCP234) |'abcdef', 'a*c*'|

