// wrong declaration
metadata
//@[8:08) Metadata <missing>. Type: error. Declaration start char: 0, length: 8

// blank identifier name
metadata 
//@[9:09) Metadata <missing>. Type: error. Declaration start char: 0, length: 9

// invalid identifier name
metadata 2
//@[9:10) Metadata <error>. Type: error. Declaration start char: 0, length: 10
metadata _2
//@[9:11) Metadata _2. Type: error. Declaration start char: 0, length: 11

// missing value
metadata missingValueAndType = 
//@[9:28) Metadata missingValueAndType. Type: error. Declaration start char: 0, length: 31

metadata missingAssignment 'noAssingmentOperator'
//@[9:26) Metadata missingAssignment. Type: error. Declaration start char: 0, length: 49

// metadata referencing metadata
metadata myMetadata = 'hello'
//@[9:19) Metadata myMetadata. Type: 'hello'. Declaration start char: 0, length: 29
var attemptToReferenceMetadata = myMetadata
//@[4:30) Variable attemptToReferenceMetadata. Type: error. Declaration start char: 0, length: 43

// two meta blocks with same identifier name
metadata same = 'value1'
//@[9:13) Metadata same. Type: 'value1'. Declaration start char: 0, length: 24
metadata same = 'value2'
//@[9:13) Metadata same. Type: 'value2'. Declaration start char: 0, length: 24

// metadata referencing vars
var testSymbol = 42
//@[4:14) Variable testSymbol. Type: int. Declaration start char: 0, length: 19
metadata test = testSymbol
//@[9:13) Metadata test. Type: int. Declaration start char: 0, length: 26


// metadata referencing itself
metadata selfRef = selfRef
//@[9:16) Metadata selfRef. Type: error. Declaration start char: 0, length: 26

// metadata with decorators
@description('this is a description')
metadata decoratedDescription = 'hasDescription'
//@[9:29) Metadata decoratedDescription. Type: 'hasDescription'. Declaration start char: 0, length: 87

@secure()
metadata secureMetadata = 'notSupported'
//@[9:23) Metadata secureMetadata. Type: 'notSupported'. Declaration start char: 0, length: 51


