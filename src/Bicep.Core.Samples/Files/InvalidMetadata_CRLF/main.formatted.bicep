// wrong declaration
bad

// blank identifier name
meta 

// invalid identifier name
meta 2
meta _2

// missing value
meta missingValueAndType = 

meta missingAssignment 'noAssingmentOperator'

// metadata referencing metadata
meta myMeta = 'hello'
var attemptToReferenceMetadata = myMeta

// two meta blocks with same identifier name
meta same = 'value1'
meta same = 'value2'

// metadata referencing vars
var testSymbol = 42
meta test = testSymbol

// metadata referencing itself
meta selfRef = selfRef

// metadata with decorators
@description('this is a description')
meta decoratedDescription = 'hasDescription'

@secure()
meta secureMeta = 'notSupported'
