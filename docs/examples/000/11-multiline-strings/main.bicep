//Multiline string samples
var pipe = '|'
var myVar = '''
test
'''
var myVar2 = '''
test'''

var myVar3 = '''

test
'''

output out1 string = '${pipe}${myVar}${pipe}'
output out2 string = '${pipe}${myVar2}${pipe}'
output out3 string = '${pipe}${myVar3}${pipe}'
