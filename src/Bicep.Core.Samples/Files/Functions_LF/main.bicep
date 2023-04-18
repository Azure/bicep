func buildUrl = (bool https, string hostname, string path) => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'

output foo string = buildUrl(true, 'google.com', 'search')

func sayHello = (string name) => 'Hi ${name}!'

output hellos array = map(['Evie', 'Casper'], name => sayHello(name))
