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

func person(name string,
//@[12:016) Local name. Type: string. Declaration start char: 12, length: 11
//@[05:011) Function person. Type: (string, int, int, int) => [string, int, int, int]. Declaration start char: 0, length: 103
  age int,
//@[02:005) Local age. Type: int. Declaration start char: 2, length: 7
weight int,
//@[00:006) Local weight. Type: int. Declaration start char: 0, length: 10
height int) array => [
//@[00:006) Local height. Type: int. Declaration start char: 0, length: 10
  name
  age
  weight
  height
]

func longParameterList(one string, two string, three string, /* comment comment comment comment */ four string) array => [
//@[23:026) Local one. Type: string. Declaration start char: 23, length: 10
//@[35:038) Local two. Type: string. Declaration start char: 35, length: 10
//@[47:052) Local three. Type: string. Declaration start char: 47, length: 12
//@[99:103) Local four. Type: string. Declaration start char: 99, length: 11
//@[05:022) Function longParameterList. Type: (string, string, string, string) => [string, string, string, string]. Declaration start char: 0, length: 151
  one
  two
  three
  four
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

