import {foo, fizz} from 'modules/mod.bicep'
//@[00:06) Identifier |import|
//@[07:08) LeftBrace |{|
//@[08:11) Identifier |foo|
//@[11:12) Comma |,|
//@[13:17) Identifier |fizz|
//@[17:18) RightBrace |}|
//@[19:23) Identifier |from|
//@[24:43) StringComplete |'modules/mod.bicep'|
//@[43:44) NewLine |\n|
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
//@[20:22) NewLine |\n\n|

type fizzes = fizz[]
//@[00:04) Identifier |type|
//@[05:11) Identifier |fizzes|
//@[12:13) Assignment |=|
//@[14:18) Identifier |fizz|
//@[18:19) LeftSquare |[|
//@[19:20) RightSquare |]|
//@[20:21) NewLine |\n|

//@[00:00) EndOfFile ||
