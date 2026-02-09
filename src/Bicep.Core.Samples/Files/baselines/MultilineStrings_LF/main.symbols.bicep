var name = 'Anthony'
//@[4:08) Variable name. Type: 'Anthony'. Declaration start char: 0, length: 20
var multilineInterpolation = $'''
//@[4:26) Variable multilineInterpolation. Type: 'Hello,\nmy\nname is\nAnthony\n'. Declaration start char: 0, length: 63
Hello,
my
name is
${name}
'''

var complexMultilineInterpolation = $$$'''
//@[4:33) Variable complexMultilineInterpolation. Type: '\${name}\n$\${name}\nAnthony\n$Anthony\n'. Declaration start char: 0, length: 84
${name}
$${name}
$$${name}
$$$${name}
'''

var interpMultiEmpty = $''''''
//@[4:20) Variable interpMultiEmpty. Type: ''. Declaration start char: 0, length: 30
var interp1Multi = $'''
//@[4:16) Variable interp1Multi. Type: 'abc123def\n'. Declaration start char: 0, length: 40
abc${123}def
'''
var interp2Multi = $'''${123}def'''
//@[4:16) Variable interp2Multi. Type: '123def'. Declaration start char: 0, length: 35
var interp3Multi = $$'''abc$${123}'''
//@[4:16) Variable interp3Multi. Type: 'abc123'. Declaration start char: 0, length: 37
var interp4Multi = $'''abc${123}${456}jk$l${789}p$'''
//@[4:16) Variable interp4Multi. Type: 'abc123456jk$l789p$'. Declaration start char: 0, length: 53
var doubleInterpMulti = $'''abc${'def${123}'}_${'${456}${789}'}'''
//@[4:21) Variable doubleInterpMulti. Type: 'abcdef123_456789'. Declaration start char: 0, length: 66
var curliesInInterpMulti = $'''{${123}{0}${true}}'''
//@[4:24) Variable curliesInInterpMulti. Type: '{123{0}True}'. Declaration start char: 0, length: 52

