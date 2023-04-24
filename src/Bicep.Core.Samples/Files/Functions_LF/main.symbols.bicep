func buildUrl = (bool https, string hostname, string path) => string '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@[22:27) Local https. Type: bool. Declaration start char: 17, length: 10
//@[36:44) Local hostname. Type: string. Declaration start char: 29, length: 15
//@[53:57) Local path. Type: string. Declaration start char: 46, length: 11
//@[05:13) Function buildUrl. Type: (bool, string, string) => string. Declaration start char: 0, length: 144

output foo string = buildUrl(true, 'google.com', 'search')
//@[07:10) Output foo. Type: string. Declaration start char: 0, length: 58

func sayHello = (string name) => string 'Hi ${name}!'
//@[24:28) Local name. Type: string. Declaration start char: 17, length: 11
//@[05:13) Function sayHello. Type: string => string. Declaration start char: 0, length: 53

output hellos array = map(['Evie', 'Casper'], name => sayHello(name))
//@[46:50) Local name. Type: 'Casper' | 'Evie'. Declaration start char: 46, length: 4
//@[07:13) Output hellos. Type: array. Declaration start char: 0, length: 69

func objReturnType = (string name) => object {
//@[29:33) Local name. Type: string. Declaration start char: 22, length: 11
//@[05:18) Function objReturnType. Type: string => object. Declaration start char: 0, length: 71
  hello: 'Hi ${name}!'
}

func arrayReturnType = (string name) => array ([
//@[31:35) Local name. Type: string. Declaration start char: 24, length: 11
//@[05:20) Function arrayReturnType. Type: string => string[]. Declaration start char: 0, length: 58
  name
])

