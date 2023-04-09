func buildUrl = (bool https, string hostname, string path) => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@[22:27) Local https. Type: bool. Declaration start char: 17, length: 10
//@[36:44) Local hostname. Type: string. Declaration start char: 29, length: 15
//@[53:57) Local path. Type: string. Declaration start char: 46, length: 11
//@[05:13) Variable buildUrl. Type: (bool, string, string) => string. Declaration start char: 0, length: 137

output foo string = buildUrl(true, 'google.com', 'search')
//@[07:10) Output foo. Type: string. Declaration start char: 0, length: 58

