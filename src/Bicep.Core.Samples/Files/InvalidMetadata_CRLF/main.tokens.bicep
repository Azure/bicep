// wrong declaration
//@[20:22) NewLine |\r\n|
bad
//@[00:03) Identifier |bad|
//@[03:07) NewLine |\r\n\r\n|

// blank identifier name
//@[24:26) NewLine |\r\n|
meta 
//@[00:04) Identifier |meta|
//@[05:09) NewLine |\r\n\r\n|

// invalid identifier name
//@[26:28) NewLine |\r\n|
meta 2
//@[00:04) Identifier |meta|
//@[05:06) Integer |2|
//@[06:08) NewLine |\r\n|
meta _2
//@[00:04) Identifier |meta|
//@[05:07) Identifier |_2|
//@[07:11) NewLine |\r\n\r\n|

// missing value
//@[16:18) NewLine |\r\n|
meta missingValueAndType = 
//@[00:04) Identifier |meta|
//@[05:24) Identifier |missingValueAndType|
//@[25:26) Assignment |=|
//@[27:31) NewLine |\r\n\r\n|

meta missingAssignment 'noAssingmentOperator'
//@[00:04) Identifier |meta|
//@[05:22) Identifier |missingAssignment|
//@[23:45) StringComplete |'noAssingmentOperator'|
//@[45:49) NewLine |\r\n\r\n|

// metadata referencing metadata
//@[32:34) NewLine |\r\n|
meta myMeta = 'hello'
//@[00:04) Identifier |meta|
//@[05:11) Identifier |myMeta|
//@[12:13) Assignment |=|
//@[14:21) StringComplete |'hello'|
//@[21:23) NewLine |\r\n|
var attemptToReferenceMetadata = myMeta
//@[00:03) Identifier |var|
//@[04:30) Identifier |attemptToReferenceMetadata|
//@[31:32) Assignment |=|
//@[33:39) Identifier |myMeta|
//@[39:43) NewLine |\r\n\r\n|

// two meta blocks with same identifier name
//@[44:46) NewLine |\r\n|
meta same = 'value1'
//@[00:04) Identifier |meta|
//@[05:09) Identifier |same|
//@[10:11) Assignment |=|
//@[12:20) StringComplete |'value1'|
//@[20:22) NewLine |\r\n|
meta same = 'value2'
//@[00:04) Identifier |meta|
//@[05:09) Identifier |same|
//@[10:11) Assignment |=|
//@[12:20) StringComplete |'value2'|
//@[20:24) NewLine |\r\n\r\n|

// metadata referencing vars
//@[28:30) NewLine |\r\n|
var testSymbol = 42
//@[00:03) Identifier |var|
//@[04:14) Identifier |testSymbol|
//@[15:16) Assignment |=|
//@[17:19) Integer |42|
//@[19:21) NewLine |\r\n|
meta test = testSymbol
//@[00:04) Identifier |meta|
//@[05:09) Identifier |test|
//@[10:11) Assignment |=|
//@[12:22) Identifier |testSymbol|
//@[22:28) NewLine |\r\n\r\n\r\n|


// metadata referencing itself
//@[30:32) NewLine |\r\n|
meta selfRef = selfRef
//@[00:04) Identifier |meta|
//@[05:12) Identifier |selfRef|
//@[13:14) Assignment |=|
//@[15:22) Identifier |selfRef|
//@[22:26) NewLine |\r\n\r\n|

// metadata with decorators
//@[27:29) NewLine |\r\n|
@description('this is a description')
//@[00:01) At |@|
//@[01:12) Identifier |description|
//@[12:13) LeftParen |(|
//@[13:36) StringComplete |'this is a description'|
//@[36:37) RightParen |)|
//@[37:39) NewLine |\r\n|
meta decoratedDescription = 'hasDescription'
//@[00:04) Identifier |meta|
//@[05:25) Identifier |decoratedDescription|
//@[26:27) Assignment |=|
//@[28:44) StringComplete |'hasDescription'|
//@[44:48) NewLine |\r\n\r\n|

@secure()
//@[00:01) At |@|
//@[01:07) Identifier |secure|
//@[07:08) LeftParen |(|
//@[08:09) RightParen |)|
//@[09:11) NewLine |\r\n|
meta secureMeta = 'notSupported'
//@[00:04) Identifier |meta|
//@[05:15) Identifier |secureMeta|
//@[16:17) Assignment |=|
//@[18:32) StringComplete |'notSupported'|
//@[32:36) NewLine |\r\n\r\n|


//@[00:00) EndOfFile ||
