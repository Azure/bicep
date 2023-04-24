func buildUrl = (bool https, string hostname, string path) => string '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'

output foo string = buildUrl(true, 'google.com', 'search')

func sayHello = (string name) => string 'Hi ${name}!'

output hellos array = map(['Evie', 'Casper'], name => sayHello(name))

func objReturnType = (string name) => object {
  hello: 'Hi ${name}!'
}

func arrayReturnType = (string name) => array ([
  name
])

