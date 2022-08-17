// wrong declaration
bad

// blank identifier name
meta 
//@[5:05) Metadata <missing>. Type: error. Declaration start char: 0, length: 5

// invalid identifier name
meta 2
//@[5:06) Metadata <error>. Type: error. Declaration start char: 0, length: 6
meta _2
//@[5:07) Metadata _2. Type: error. Declaration start char: 0, length: 7

// missing value
meta missingValueAndType = 
//@[5:24) Metadata missingValueAndType. Type: error. Declaration start char: 0, length: 27

meta missingAssignment 'noAssingmentOperator'
//@[5:22) Metadata missingAssignment. Type: error. Declaration start char: 0, length: 45

// metadata referencing metadata
meta myMeta = 'hello'
//@[5:11) Metadata myMeta. Type: 'hello'. Declaration start char: 0, length: 21
var attemptToReferenceMetadata = myMeta
//@[4:30) Variable attemptToReferenceMetadata. Type: error. Declaration start char: 0, length: 39

// two meta blocks with same identifier name
meta same = 'value1'
//@[5:09) Metadata same. Type: 'value1'. Declaration start char: 0, length: 20
meta same = 'value2'
//@[5:09) Metadata same. Type: 'value2'. Declaration start char: 0, length: 20

// metadata referencing vars
var testSymbol = 42
//@[4:14) Variable testSymbol. Type: int. Declaration start char: 0, length: 19
meta test = testSymbol
//@[5:09) Metadata test. Type: int. Declaration start char: 0, length: 22


// metadata referencing itself
meta selfRef = selfRef
//@[5:12) Metadata selfRef. Type: error. Declaration start char: 0, length: 22

// metadata with decorators
@description('this is a description')
meta decoratedDescription = 'hasDescription'
//@[5:25) Metadata decoratedDescription. Type: 'hasDescription'. Declaration start char: 0, length: 83

@secure()
meta secureMeta = 'notSupported'
//@[5:15) Metadata secureMeta. Type: 'notSupported'. Declaration start char: 0, length: 43


