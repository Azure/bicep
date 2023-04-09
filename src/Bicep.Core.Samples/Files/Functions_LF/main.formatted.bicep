funcbuildUrl=(httpsbool,hostnamestring,pathstring)=>'${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'

output foo string = buildUrl(true, 'google.com', 'search')
