// wrong declaration
metadata

// blank identifier name
metadata 

// invalid identifier name
metadata 2
metadata _2

// missing value
metadata missingValueAndType = 

metadata missingAssignment 'noAssingmentOperator'

// metadata referencing metadata
metadata myMetadata = 'hello'
var attemptToReferenceMetadata = myMetadata

// two meta blocks with same identifier name
metadata same = 'value1'
metadata same = 'value2'

// metadata referencing vars
var testSymbol = 42
metadata test = testSymbol

// metadata referencing itself
metadata selfRef = selfRef

// metadata with decorators
@description('this is a description')
metadata decoratedDescription = 'hasDescription'

@secure()
metadata secureMetadata = 'notSupported'
