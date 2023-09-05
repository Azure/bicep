// wrong declaration
//@[20:22) NewLine |\r\n|
metadata
//@[00:08) Identifier |metadata|
//@[08:12) NewLine |\r\n\r\n|

// blank identifier name
//@[24:26) NewLine |\r\n|
metadata 
//@[00:08) Identifier |metadata|
//@[09:13) NewLine |\r\n\r\n|

// invalid identifier name
//@[26:28) NewLine |\r\n|
metadata 2
//@[00:08) Identifier |metadata|
//@[09:10) Integer |2|
//@[10:12) NewLine |\r\n|
metadata _2
//@[00:08) Identifier |metadata|
//@[09:11) Identifier |_2|
//@[11:15) NewLine |\r\n\r\n|

// missing value
//@[16:18) NewLine |\r\n|
metadata missingValueAndType = 
//@[00:08) Identifier |metadata|
//@[09:28) Identifier |missingValueAndType|
//@[29:30) Assignment |=|
//@[31:35) NewLine |\r\n\r\n|

metadata missingAssignment 'noAssingmentOperator'
//@[00:08) Identifier |metadata|
//@[09:26) Identifier |missingAssignment|
//@[27:49) StringComplete |'noAssingmentOperator'|
//@[49:53) NewLine |\r\n\r\n|

// metadata referencing metadata
//@[32:34) NewLine |\r\n|
metadata myMetadata = 'hello'
//@[00:08) Identifier |metadata|
//@[09:19) Identifier |myMetadata|
//@[20:21) Assignment |=|
//@[22:29) StringComplete |'hello'|
//@[29:31) NewLine |\r\n|
var attemptToReferenceMetadata = myMetadata
//@[00:03) Identifier |var|
//@[04:30) Identifier |attemptToReferenceMetadata|
//@[31:32) Assignment |=|
//@[33:43) Identifier |myMetadata|
//@[43:47) NewLine |\r\n\r\n|

// two meta blocks with same identifier name
//@[44:46) NewLine |\r\n|
metadata same = 'value1'
//@[00:08) Identifier |metadata|
//@[09:13) Identifier |same|
//@[14:15) Assignment |=|
//@[16:24) StringComplete |'value1'|
//@[24:26) NewLine |\r\n|
metadata same = 'value2'
//@[00:08) Identifier |metadata|
//@[09:13) Identifier |same|
//@[14:15) Assignment |=|
//@[16:24) StringComplete |'value2'|
//@[24:28) NewLine |\r\n\r\n|

// metadata referencing vars
//@[28:30) NewLine |\r\n|
var testSymbol = 42
//@[00:03) Identifier |var|
//@[04:14) Identifier |testSymbol|
//@[15:16) Assignment |=|
//@[17:19) Integer |42|
//@[19:21) NewLine |\r\n|
metadata test = testSymbol
//@[00:08) Identifier |metadata|
//@[09:13) Identifier |test|
//@[14:15) Assignment |=|
//@[16:26) Identifier |testSymbol|
//@[26:32) NewLine |\r\n\r\n\r\n|


// metadata referencing itself
//@[30:32) NewLine |\r\n|
metadata selfRef = selfRef
//@[00:08) Identifier |metadata|
//@[09:16) Identifier |selfRef|
//@[17:18) Assignment |=|
//@[19:26) Identifier |selfRef|
//@[26:30) NewLine |\r\n\r\n|

// metadata with decorators
//@[27:29) NewLine |\r\n|
@description('this is a description')
//@[00:01) At |@|
//@[01:12) Identifier |description|
//@[12:13) LeftParen |(|
//@[13:36) StringComplete |'this is a description'|
//@[36:37) RightParen |)|
//@[37:39) NewLine |\r\n|
metadata decoratedDescription = 'hasDescription'
//@[00:08) Identifier |metadata|
//@[09:29) Identifier |decoratedDescription|
//@[30:31) Assignment |=|
//@[32:48) StringComplete |'hasDescription'|
//@[48:52) NewLine |\r\n\r\n|

@secure()
//@[00:01) At |@|
//@[01:07) Identifier |secure|
//@[07:08) LeftParen |(|
//@[08:09) RightParen |)|
//@[09:11) NewLine |\r\n|
metadata secureMetadata = 'notSupported'
//@[00:08) Identifier |metadata|
//@[09:23) Identifier |secureMetadata|
//@[24:25) Assignment |=|
//@[26:40) StringComplete |'notSupported'|
//@[40:44) NewLine |\r\n\r\n|


//@[00:00) EndOfFile ||
