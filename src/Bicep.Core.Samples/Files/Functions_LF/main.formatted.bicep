func buildUrl = (boolhttps, stringhostname, stringpath) => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'

output foo string = buildUrl(true, 'google.com', 'search')

func sayHello = (stringname) => 'Hi ${name}!'

output hellos array = map([ 'Evie', 'Casper' ], name => sayHello(name))
