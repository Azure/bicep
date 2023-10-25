import {foo, fizz, pop, greet} from 'modules/mod.bicep'
//@[00:06) Identifier |import|
//@[07:08) LeftBrace |{|
//@[08:11) Identifier |foo|
//@[11:12) Comma |,|
//@[13:17) Identifier |fizz|
//@[17:18) Comma |,|
//@[19:22) Identifier |pop|
//@[22:23) Comma |,|
//@[24:29) Identifier |greet|
//@[29:30) RightBrace |}|
//@[31:35) Identifier |from|
//@[36:55) StringComplete |'modules/mod.bicep'|
//@[55:56) NewLine |\n|
import * as mod2 from 'modules/mod2.bicep'
//@[00:06) Identifier |import|
//@[07:08) Asterisk |*|
//@[09:11) AsKeyword |as|
//@[12:16) Identifier |mod2|
//@[17:21) Identifier |from|
//@[22:42) StringComplete |'modules/mod2.bicep'|
//@[42:43) NewLine |\n|
import {
//@[00:06) Identifier |import|
//@[07:08) LeftBrace |{|
//@[08:09) NewLine |\n|
  'not-a-valid-bicep-identifier' as withInvalidIdentifier
//@[02:32) StringComplete |'not-a-valid-bicep-identifier'|
//@[33:35) AsKeyword |as|
//@[36:57) Identifier |withInvalidIdentifier|
//@[57:58) NewLine |\n|
  refersToCopyVariable
//@[02:22) Identifier |refersToCopyVariable|
//@[22:23) NewLine |\n|
} from 'modules/mod.json'
//@[00:01) RightBrace |}|
//@[02:06) Identifier |from|
//@[07:25) StringComplete |'modules/mod.json'|
//@[25:27) NewLine |\n\n|

var aliasedFoo = foo
//@[00:03) Identifier |var|
//@[04:14) Identifier |aliasedFoo|
//@[15:16) Assignment |=|
//@[17:20) Identifier |foo|
//@[20:21) NewLine |\n|
var aliasedBar = mod2.foo
//@[00:03) Identifier |var|
//@[04:14) Identifier |aliasedBar|
//@[15:16) Assignment |=|
//@[17:21) Identifier |mod2|
//@[21:22) Dot |.|
//@[22:25) Identifier |foo|
//@[25:27) NewLine |\n\n|

type fizzes = fizz[]
//@[00:04) Identifier |type|
//@[05:11) Identifier |fizzes|
//@[12:13) Assignment |=|
//@[14:18) Identifier |fizz|
//@[18:19) LeftSquare |[|
//@[19:20) RightSquare |]|
//@[20:22) NewLine |\n\n|

param fizzParam mod2.fizz
//@[00:05) Identifier |param|
//@[06:15) Identifier |fizzParam|
//@[16:20) Identifier |mod2|
//@[20:21) Dot |.|
//@[21:25) Identifier |fizz|
//@[25:26) NewLine |\n|
output magicWord pop = refersToCopyVariable[3].value
//@[00:06) Identifier |output|
//@[07:16) Identifier |magicWord|
//@[17:20) Identifier |pop|
//@[21:22) Assignment |=|
//@[23:43) Identifier |refersToCopyVariable|
//@[43:44) LeftSquare |[|
//@[44:45) Integer |3|
//@[45:46) RightSquare |]|
//@[46:47) Dot |.|
//@[47:52) Identifier |value|
//@[52:54) NewLine |\n\n|

output greeting string = greet('friend')
//@[00:06) Identifier |output|
//@[07:15) Identifier |greeting|
//@[16:22) Identifier |string|
//@[23:24) Assignment |=|
//@[25:30) Identifier |greet|
//@[30:31) LeftParen |(|
//@[31:39) StringComplete |'friend'|
//@[39:40) RightParen |)|
//@[40:41) NewLine |\n|

//@[00:00) EndOfFile ||
