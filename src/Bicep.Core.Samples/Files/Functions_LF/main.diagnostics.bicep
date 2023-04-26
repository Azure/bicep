func buildUrl(https bool, hostname string, path string) string => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'

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

