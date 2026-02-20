func buildUrl(https bool, hostname string, path string) string => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@[14:019) Local https. Type: bool. Declaration start char: 14, length: 10
//@[26:034) Local hostname. Type: string. Declaration start char: 26, length: 15
//@[43:047) Local path. Type: string. Declaration start char: 43, length: 11
//@[05:013) Function buildUrl. Type: (bool, string, string) => string. Declaration start char: 0, length: 141

output foo string = buildUrl(true, 'google.com', 'search')
//@[07:010) Output foo. Type: string. Declaration start char: 0, length: 58

func sayHello(name string) string => 'Hi ${name}!'
//@[14:018) Local name. Type: string. Declaration start char: 14, length: 11
//@[05:013) Function sayHello. Type: string => string. Declaration start char: 0, length: 50

output hellos array = map(['Evie', 'Casper'], name => sayHello(name))
//@[46:050) Local name. Type: 'Casper' | 'Evie'. Declaration start char: 46, length: 4
//@[07:013) Output hellos. Type: array. Declaration start char: 0, length: 69

func objReturnType(name string) object => {
//@[19:023) Local name. Type: string. Declaration start char: 19, length: 11
//@[05:018) Function objReturnType. Type: string => object. Declaration start char: 0, length: 68
  hello: 'Hi ${name}!'
}

func arrayReturnType(name string) array => [
//@[21:025) Local name. Type: string. Declaration start char: 21, length: 11
//@[05:020) Function arrayReturnType. Type: string => [string]. Declaration start char: 0, length: 53
  name
]

func asdf(name string) array => [
//@[10:014) Local name. Type: string. Declaration start char: 10, length: 11
//@[05:009) Function asdf. Type: string => ['asdf', string]. Declaration start char: 0, length: 51
  'asdf'
  name
]

@minValue(0)
type positiveInt = int
//@[05:016) TypeAlias positiveInt. Type: Type<int>. Declaration start char: 0, length: 35

func typedArg(input string[]) positiveInt => length(input)
//@[14:019) Local input. Type: string[]. Declaration start char: 14, length: 14
//@[05:013) Function typedArg. Type: string[] => int. Declaration start char: 0, length: 58

func barTest() array => ['abc', 'def']
//@[05:012) Function barTest. Type: () => ['abc', 'def']. Declaration start char: 0, length: 38
func fooTest() array => map(barTest(), a => 'Hello ${a}!')
//@[39:040) Local a. Type: any. Declaration start char: 39, length: 1
//@[05:012) Function fooTest. Type: () => string[]. Declaration start char: 0, length: 58

output fooValue array = fooTest()
//@[07:015) Output fooValue. Type: array. Declaration start char: 0, length: 33

func test() object => loadJsonContent('./repro-data.json')
//@[05:009) Function test. Type: () => object. Declaration start char: 0, length: 58
func test2() string => loadTextContent('./repro-data.json')
//@[05:010) Function test2. Type: () => '{}'. Declaration start char: 0, length: 59
func test3() object => loadYamlContent('./repro-data.json')
//@[05:010) Function test3. Type: () => object. Declaration start char: 0, length: 59
func test4() string => loadFileAsBase64('./repro-data.json')
//@[05:010) Function test4. Type: () => repro-data.json. Declaration start char: 0, length: 60

// validate formatter works (https://github.com/Azure/bicep/issues/12913)
func a(____________________________________________________________________________________________ string) string => 'a'
//@[07:099) Local ____________________________________________________________________________________________. Type: string. Declaration start char: 7, length: 99
//@[05:006) Function a. Type: string => 'a'. Declaration start char: 0, length: 121
func b(longParameterName1 string, longParameterName2 string, longParameterName3 string, longParameterName4 string) string => 'b'
//@[07:025) Local longParameterName1. Type: string. Declaration start char: 7, length: 25
//@[34:052) Local longParameterName2. Type: string. Declaration start char: 34, length: 25
//@[61:079) Local longParameterName3. Type: string. Declaration start char: 61, length: 25
//@[88:106) Local longParameterName4. Type: string. Declaration start char: 88, length: 25
//@[05:006) Function b. Type: (string, string, string, string) => 'b'. Declaration start char: 0, length: 128

func buildUrlMultiLine(
//@[05:022) Function buildUrlMultiLine. Type: (bool, string, string) => string. Declaration start char: 0, length: 158
  https bool,
//@[02:007) Local https. Type: bool. Declaration start char: 2, length: 10
  hostname string,
//@[02:010) Local hostname. Type: string. Declaration start char: 2, length: 15
  path string
//@[02:006) Local path. Type: string. Declaration start char: 2, length: 11
) string => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'

output likeExactMatch bool =like('abc', 'abc')
//@[07:021) Output likeExactMatch. Type: bool. Declaration start char: 0, length: 46
output likeWildCardMatch bool= like ('abcdef', 'a*c*')
//@[07:024) Output likeWildCardMatch. Type: bool. Declaration start char: 0, length: 54

