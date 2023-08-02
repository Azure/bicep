func buildUrl(https bool, hostname string, path string) string => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@[14:19) Local https. Type: bool. Declaration start char: 14, length: 10
//@[26:34) Local hostname. Type: string. Declaration start char: 26, length: 15
//@[43:47) Local path. Type: string. Declaration start char: 43, length: 11
//@[05:13) Function buildUrl. Type: (bool, string, string) => string. Declaration start char: 0, length: 141

output foo string = buildUrl(true, 'google.com', 'search')
//@[07:10) Output foo. Type: string. Declaration start char: 0, length: 58

func sayHello(name string) string => 'Hi ${name}!'
//@[14:18) Local name. Type: string. Declaration start char: 14, length: 11
//@[05:13) Function sayHello. Type: string => string. Declaration start char: 0, length: 50

output hellos array = map(['Evie', 'Casper'], name => sayHello(name))
//@[46:50) Local name. Type: 'Casper' | 'Evie'. Declaration start char: 46, length: 4
//@[07:13) Output hellos. Type: array. Declaration start char: 0, length: 69

func objReturnType(name string) object => {
//@[19:23) Local name. Type: string. Declaration start char: 19, length: 11
//@[05:18) Function objReturnType. Type: string => object. Declaration start char: 0, length: 68
  hello: 'Hi ${name}!'
}

func arrayReturnType(name string) array => [
//@[21:25) Local name. Type: string. Declaration start char: 21, length: 11
//@[05:20) Function arrayReturnType. Type: string => [string]. Declaration start char: 0, length: 53
  name
]

func asdf(name string) array => [
//@[10:14) Local name. Type: string. Declaration start char: 10, length: 11
//@[05:09) Function asdf. Type: string => ['asdf', string]. Declaration start char: 0, length: 51
  'asdf'
  name
]

