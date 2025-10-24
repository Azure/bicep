// evaluates to '\'abc\''
var multilineExtraQuotes = ''''abc''''

// evaluates to '\'\nabc\n\''
var multilineExtraQuotesNewlines = ''''
abc
''''

var multilineSingleLine = '''hello!'''

var name = 'Anthony'
var multilineInterpolation = $'''
hello ${name}!
'''

var multilineInterpolationMultipleEscapes = $$'''
hello $${name}!
${this} is literal
'''
