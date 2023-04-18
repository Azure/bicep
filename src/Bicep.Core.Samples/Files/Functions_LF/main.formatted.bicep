func buildUrl = (httpsbool, hostnamestring, pathstring) => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'

output foo string = buildUrl(true, 'google.com', 'search')

func sayHello = (namestring) => 'Hi ${name}!'

output hellos array = map([ 'Evie', 'Casper' ], name => sayHello(name))
