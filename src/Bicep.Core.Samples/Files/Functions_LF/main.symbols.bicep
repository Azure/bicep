func buildUrl = (https bool, hostname string, path string) string => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@[17:22) Local https. Type: bool. Declaration start char: 17, length: 10
//@[29:37) Local hostname. Type: string. Declaration start char: 29, length: 15
//@[46:50) Local path. Type: string. Declaration start char: 46, length: 11
//@[05:13) Function buildUrl. Type: (bool, string, string) => string. Declaration start char: 0, length: 144

output foo string = buildUrl(true, 'google.com', 'search')
//@[07:10) Output foo. Type: string. Declaration start char: 0, length: 58

func sayHello = (name string) string => 'Hi ${name}!'
//@[17:21) Local name. Type: string. Declaration start char: 17, length: 11
//@[05:13) Function sayHello. Type: string => string. Declaration start char: 0, length: 53

output hellos array = map(['Evie', 'Casper'], name => sayHello(name))
//@[46:50) Local name. Type: 'Casper' | 'Evie'. Declaration start char: 46, length: 4
//@[07:13) Output hellos. Type: array. Declaration start char: 0, length: 69

func objReturnType = (name string) object => {
//@[22:26) Local name. Type: string. Declaration start char: 22, length: 11
//@[05:18) Function objReturnType. Type: string => object. Declaration start char: 0, length: 71
  hello: 'Hi ${name}!'
}

func arrayReturnType = (name string) array => [
//@[24:28) Local name. Type: string. Declaration start char: 24, length: 11
//@[05:20) Function arrayReturnType. Type: string => [string]. Declaration start char: 0, length: 56
  name
]

